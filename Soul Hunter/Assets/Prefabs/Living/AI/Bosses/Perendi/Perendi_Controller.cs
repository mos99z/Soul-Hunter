using UnityEngine;
using System.Collections;
using System.Linq;

public class Perendi_Controller : MonoBehaviour {

	public GameObject Mistral;
	NavMeshAgent navigation;			// Used to allow the minion to use the NavMesh
	public GameObject target = null;
	Vector3 destination;				// Location to move to using the NavMesh
	public GameObject[] attackingWaypoints;	// Used to rotate around the player
	
	float waypointTimer = 0.0f;
	float currentAttackTimer = 0.0f;
	float lungeTimer = 0.0f;
	int closestShadow = 0;				// 
	float currentRotation = 0.0f;
	float AngularAcceleration = 3.5f;
	bool mistralAlive = true;
	public bool isAttacking = false;
	public float AttackMinimum = 5;
	public float AttackMaximum = 10;
	public float Damage = 100.0f;
	public SphereCollider AttackCollider;
	public GameObject ShockDischarge;
	public float SDCooldown = 6.0f;
	public float SDDistance = 2.0f;
	float currentSDCooldown = 0.0f;
	public float SDChargeUp = 1.0f;
	bool DischargingStatic = false;
	bool CurrentlyDischarging = false;

	// Use this for initialization
	void Start ()
	{
		target = GameObject.FindGameObjectWithTag ("Player");
		navigation = GetComponent<NavMeshAgent> ();
		destination = Random.insideUnitSphere * 7;
		destination.y = 0;
		destination += transform.position;
		attackingWaypoints = GameObject.FindGameObjectsWithTag ("Shadow").OrderBy(waypoint => waypoint.name).ToArray<GameObject>();
	}
	
	// Update is called once per frame
	// The enemy will rotate around the player and lunge in after a certain amount of time has passed
	// After another amount of time has passed, the enemy has the ability to use a Shock Discharge, but only if the player is within range
	void FixedUpdate () 
	{
		if (target == null) {
		} 

		else 
		{
			if (DischargingStatic == false) 
			{
				currentSDCooldown -= Time.deltaTime;
				
				//destination = attackingWaypoints[closestShadow].transform.position;
				TurnTowardsPlayer ();
				
				if (isAttacking == false) 
				{	
					destination = attackingWaypoints [closestShadow].transform.position;
					navigation.SetDestination (destination);
					currentAttackTimer -= Time.deltaTime;
					waypointTimer -= Time.deltaTime;
					if (navigation.remainingDistance <= 0.3f && waypointTimer <= 0.0f) 
					{
						waypointTimer = 0.3f;
						closestShadow++;
						if (closestShadow >= attackingWaypoints.Length)
							closestShadow = 0;
						
						
						destination = attackingWaypoints [closestShadow].transform.position;
					}
					
					if (currentAttackTimer <= 0.0f) 
					{
						isAttacking = true;
						lungeTimer = 1.5f;
						float AttackTimer = Random.Range(AttackMinimum,AttackMaximum);
						currentAttackTimer = AttackTimer;
						navigation.autoBraking = true;
						navigation.updateRotation = true;
						navigation.stoppingDistance = 2.0f;
						destination = gameObject.transform.position;
					}
				}
				
				if (isAttacking == true) {
					lungeTimer -= Time.deltaTime;
					//gameObject.transform.LookAt(target.transform.position,Vector3.up);
					navigation.SetDestination (target.transform.position);
					
					if (navigation.remainingDistance < 2.5f)
						AttackCollider.enabled = true;
					
					if (lungeTimer <= 0.0f)
					{
						isAttacking = false;
						AttackCollider.enabled = false;
						navigation.stoppingDistance = 0.0f;
						navigation.updateRotation = false;
						//SearchForNearestNode();
						destination = attackingWaypoints [closestShadow].transform.position;
					}	
				}
				
				float playerDistance = (target.transform.position - gameObject.transform.position).magnitude;
				
				if (currentSDCooldown <= 0.0f && playerDistance < SDDistance) 
				{
					navigation.Stop();
					Debug.Log("Stopped Movement");
					currentSDCooldown = SDChargeUp;
					DischargingStatic = true;
				}
			}
			
			else if (DischargingStatic == true) 
			{
				currentSDCooldown -= Time.deltaTime;
				if(currentSDCooldown <= 0.0f && CurrentlyDischarging == false)
				{
					ShockDischarge.SetActive(true);
					CurrentlyDischarging = true;
					currentSDCooldown = Random.Range(2.0f,3.0f);
				}
				if(currentSDCooldown <= 0.0f && CurrentlyDischarging == true)
				{
					ShockDischarge.SetActive(false);
					CurrentlyDischarging = false;
					currentSDCooldown = SDCooldown;
					DischargingStatic = false;
					navigation.Resume();
					Debug.Log("Resumed Movement");
				}
				
				
			}
		}
	}
	
	
	// This function makes the enemy always face towards the player while rotating around him
	void TurnTowardsPlayer()
	{
		Vector3 Forward = transform.forward;
		Vector3 PlayerDistance = target.transform.position - transform.position;
		PlayerDistance.y = 0.0f;
		float rotation = 0.0f;
		float angle = Vector3.Angle(PlayerDistance, Forward);
		
		// Rotation
		if (angle > 5.0f)
		{
			if(Vector3.Cross(PlayerDistance, Forward).y > 0)
				rotation = -1 * AngularAcceleration;
			if(Vector3.Cross(PlayerDistance, Forward).y < 0)
				rotation = 1 * AngularAcceleration;
			
			currentRotation += rotation;
			currentRotation = Mathf.Min (currentRotation, AngularAcceleration);
			currentRotation = Mathf.Max (currentRotation, -AngularAcceleration);
			transform.Rotate (0, currentRotation, 0);
		}
	}
	
	void SearchForNearestNode()
	{
		closestShadow = 0;
		
		for (int i = 0; i < attackingWaypoints.Length; i++) 
		{
			Vector3 currWaypoint = attackingWaypoints[closestShadow].transform.position - gameObject.transform.position;
			Vector3 nextWaypoint = attackingWaypoints[i].transform.position - gameObject.transform.position;
			
			if(nextWaypoint.magnitude < currWaypoint.magnitude)
			{
				closestShadow = i;
			}
		}
	}
	
	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") 
		{
			col.SendMessage("TakeDamage",Damage);
		}
	}

	void OtherBossDead()
	{
		mistralAlive = false;
		navigation.speed *= 1.5f;
		AttackMinimum *= 0.5f;
		AttackMaximum *= 0.5f;
		gameObject.GetComponent<Living_Obj>().SoulValue = SoulType.Red;
	}

	void OnDestroy()
	{
		if (mistralAlive)
		Mistral.SendMessage ("OtherBossDead");
	}

	void PlayerDead()
	{
		target = null;
	}
}
