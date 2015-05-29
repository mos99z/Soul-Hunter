using UnityEngine;
using System.Collections;
using System.Linq;

public class Melee_Minion_Controller : MonoBehaviour {

	NavMeshAgent navigation;			// Used to allow the minion to use the NavMesh
	GameObject target = null;			// When the minion notices the player, this will be used to store his location
	Vector3 destination;				// Location to move to using the NavMesh
	float aggroDistance = 10.0f;		// Distance the player must be within to be noticed by the minion
	public GameObject[] attackingWaypoints;	// Used to rotate around the player

	float waypointTimer = 0.0f;
	float currentAttackTimer = 0.0f;
	float lungeTimer = 0.0f;
	GameObject player = null;			// 
	int closestShadow = 0;				// 
	float currentRotation = 0.0f;
	float AngularAcceleration = 3.5f;
	public bool isAttacking = false;
	public bool isSwarming = false;
	public float WaitMinimum = 5;
	public float WaitMaximum = 7;
	public float AttackTimer = 5.0f;
	public float Damage = 100.0f;
	public SphereCollider AttackCollider;

	public GameObject DirectionIndicator = null;
	public Animator Animate = null;

	// Use this for initialization
	void Start ()
	{
		if (Animate == null)
			Animate = transform.GetComponentInChildren<Animator> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		navigation = GetComponent<NavMeshAgent> ();
		destination = Random.insideUnitSphere * 7;
		destination.y = 0;
		destination += transform.position;
		attackingWaypoints = GameObject.FindGameObjectsWithTag ("Shadow").OrderBy(waypoint => waypoint.name).ToArray<GameObject>();
	}
	
	// Update is called once per frame
	// If there is no target, the enemy will move to a random waypoint in the navmesh in a patrol-like fashion
	// If there is a target, the enemy will rotate around the player and lunge in after a certain amount of time has passed
	void FixedUpdate () 
	{
		if (target == null) 
		{
			waypointTimer -= Time.deltaTime;
			if(waypointTimer <= 0.0f)
				navigation.SetDestination (destination);


			if (navigation.remainingDistance == 0 && waypointTimer <= 0.0f) 
			{
				waypointTimer = Random.Range(WaitMinimum,WaitMaximum);
				destination = Random.insideUnitSphere * 7;
				destination.y = 0;
				destination += transform.position;
			}
			SearchForPlayer ();
		}

		else if (target != null) 
		{
			//destination = attackingWaypoints[closestShadow].transform.position;
			TurnTowardsPlayer();

			if(isAttacking == false)
			{
				navigation.SetDestination (destination);
				currentAttackTimer -= Time.deltaTime;
				waypointTimer -= Time.deltaTime;
				if (navigation.remainingDistance <= 0.3f && waypointTimer <= 0.0f) 
				{
					waypointTimer = 0.3f;
					closestShadow++;
					if(closestShadow >= attackingWaypoints.Length)
						closestShadow = 0;
					
					
					destination = attackingWaypoints[closestShadow].transform.position;
				}

				if(currentAttackTimer <= 0.0f)
				{
					isAttacking = true;
					lungeTimer = 1.5f;
					currentAttackTimer = AttackTimer;
					navigation.autoBraking = true;
					navigation.updateRotation = true;
					navigation.stoppingDistance = 2.0f;
					destination = gameObject.transform.position;
				}
			}

			if(isAttacking == true)
			{
				lungeTimer -= Time.deltaTime;
				//gameObject.transform.LookAt(target.transform.position,Vector3.up);
				navigation.SetDestination (target.transform.position);
				
				if(navigation.remainingDistance < 2.5f)
					AttackCollider.enabled = true;

				if(lungeTimer <= 0.0f)
				{
					isAttacking = false;
					AttackCollider.enabled = false;
					navigation.stoppingDistance = 0.0f;
					navigation.updateRotation = false;
					//SearchForNearestNode();
					destination = attackingWaypoints[closestShadow].transform.position;
				}

				
			}

		}
	}

	// Check distance to player. Will target the player if he is within distance
	void SearchForPlayer()
	{
		if (CheckPlayerAvailability () == false)
			return;
		
		Vector3 playerDistance = player.transform.position - gameObject.transform.position;
		if (playerDistance.magnitude < aggroDistance) 
		{
			target = player;

			SearchForNearestNode();

			navigation.updateRotation = false;
			navigation.autoBraking = false;
			waypointTimer = 0.0f;
			currentAttackTimer = AttackTimer;
			isSwarming = true;
			destination = attackingWaypoints[closestShadow].transform.position;
		}
	}


	// This function makes the enemy always face towards the player while rotating around him
	void TurnTowardsPlayer()
	{
//		DirectionIndicator.transform.LookAt(target.transform, new Vector3(0,1,0));
		Vector3 movementDirection = navigation.velocity.normalized;
		if (movementDirection.magnitude >= 1.0f) {
			DirectionIndicator.transform.forward = navigation.velocity.normalized;
			float dotProd = Vector3.Dot (new Vector3 (0, 0, 1), movementDirection);
			Vector3 crossProd = Vector3.Cross (new Vector3 (0, 0, 1), movementDirection);
			if (dotProd >= 0.75f)
				Animate.Play ("Melee_Idle_Up");
			else if (dotProd <= -0.75f)
				Animate.Play ("Melee_Idle_Down");
			else if (dotProd > -0.25f && dotProd <= 0.25f)
			{
				if (crossProd.y < 0.0f)
					Animate.Play ("Melee_Idle_Left");
				else
					Animate.Play ("Melee_Idle_Right");
			}
			else if (dotProd > 0.25f && dotProd < 0.75f)
			{
				if (crossProd.y < 0.0f)
					Animate.Play ("Melee_Idle_UpLeft");
				else
					Animate.Play ("Melee_Idle_UpRight");
			}
			else
			{
				if (crossProd.y < 0.0f)
					Animate.Play ("Melee_Idle_DownLeft");
				else
					Animate.Play ("Melee_Idle_DownRight");
			}
		}
//		Vector3 Forward = currentRotationtransform.forward;
//		Vector3 PlayerDistance = target.transform.position - transform.position;
//		PlayerDistance.y = 0.0f;
//		float rotation = 0.0f;
//		float angle = Vector3.Angle(PlayerDistance, Forward);
//
//		// Rotation
//		if (angle > 5.0f)
//		{
//			if(Vector3.Cross(PlayerDistance, Forward).y > 0)
//				rotation = -1 * AngularAcceleration;
//			if(Vector3.Cross(PlayerDistance, Forward).y < 0)
//				rotation = 1 * AngularAcceleration;
//
//			currentRotation += rotation;
//			currentRotation = Mathf.Min (currentRotation, AngularAcceleration);
//			currentRotation = Mathf.Max (currentRotation, -AngularAcceleration);
//			DirectionIndicator.transform.Rotate (0, 0, currentRotation);
//		}
	}


	// This function will check to see if the player has too many enemies circling him already
	// Will return true if there is space available to circle the player or if there are 5 or less enemies remaining
	// Will return false if there are too many other enemies swarming
	bool CheckPlayerAvailability()
	{
		GameObject[] enemyList = GameObject.FindGameObjectsWithTag ("Enemy");

		if(enemyList.Length <= 5)
		{
			aggroDistance = 100;
			return true;
		}

		int swarmingCounter = 0;

		for (int i = 0; i < enemyList.Length; i++) 
		{
			if(enemyList[i] == gameObject)
				continue;
			if(enemyList[i].name.Contains("Melee"))
			{
				Melee_Minion_Controller enemyScript = enemyList[i].GetComponent<Melee_Minion_Controller>();

				if(enemyScript.isSwarming == true)
					swarmingCounter++;
				
				if(swarmingCounter >= 5)
					return false;
			}

			else
				continue;
		}

		return true;
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
}