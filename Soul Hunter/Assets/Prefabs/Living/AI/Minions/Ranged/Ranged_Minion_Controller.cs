using UnityEngine;
using System.Collections;

public class Ranged_Minion_Controller : MonoBehaviour {

	public float MinRange = 5.0f;		// Minimum Attack Range
	public float MaxRange = 18.0f;		// Maximum Attack Range
	public LayerMask Mask;				// Layers to ignore

	// Movement Variables
	NavMeshAgent navigation;			// Used to allow the minion to use the NavMesh
	GameObject target;					// Used to know where the player is at all times
	bool isMoving = false;				// a boolean used to prevent the enemy from continuously trying to update position
	GameObject[] safeZones;
	Vector3 destination;				// Location to move to using the NavMesh
	int closestSafeZone;// = 0;			// 
	float currentRotation = 0.0f;
	float AngularAcceleration = 3.5f;

	// Attack Variables
	int attackCounter = 0;
	public float AttackCooldown = 2.0f;
	float currentAttackTimer = 0.0f;

	
	// Use this for initialization
	void Start () 
	{
		navigation = GetComponent<NavMeshAgent> ();
		target = GameObject.FindGameObjectWithTag ("Player");
		safeZones = GameObject.FindGameObjectsWithTag ("SafeZone");
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (CheckPlayerDistance () == true && isMoving == false && attackCounter < 3) 
		{
			TurnTowardsPlayer();
			currentAttackTimer -= Time.deltaTime;

			if(currentAttackTimer <= 0.0f)
			{
				Debug.Log("Enemy Attacked");
				attackCounter++;
				currentAttackTimer = AttackCooldown;
			}

			if(attackCounter >= 3)
			{
				SideStrafe();
				isMoving = true;
			}
		}
		
		else
		{
			if(isMoving == false)
			{
				Reposition();
				isMoving = true;
			}
			
			else if(isMoving == true)
			{

				navigation.SetDestination(destination);
				if(navigation.remainingDistance == 0)
				{
					isMoving = false;
					navigation.updateRotation = false;
				}
				
			}
		}


	}

	// This function will check the distance of the player. It will return true if
	// the player is between the min and max range. It will return false if the
	// player is outside of that zone
	bool CheckPlayerDistance()
	{
		Vector3 distance = target.transform.position - gameObject.transform.position;
		if (distance.magnitude > MinRange && distance.magnitude < MaxRange)
			return true;
		
		navigation.updateRotation = true;
		return false;
	}

	// Search for a new viable position. If no viable position, will search for the nearest safe zone
	void Reposition()
	{
		attackCounter = 0;
		Vector3 direction = gameObject.transform.position - target.transform.position;
		direction.y = 0;
		direction.Normalize ();
		float variance = Random.Range (-5.0f, 5.0f);
		direction = Quaternion.Euler (0, variance, 0) * direction;

		Ray randomDirection = new Ray();
		randomDirection.origin = target.transform.position;
		randomDirection.origin = new Vector3 (randomDirection.origin.x, 0.5f, randomDirection.origin.z);
		randomDirection.direction = direction;
		//Debug.Log (randomDirection.origin);
		//Debug.Log (randomDirection.direction);

		RaycastHit debugData;

		if (Physics.Raycast (randomDirection, out debugData, MaxRange * 0.5f, Mask)) 
		{
			FindRandomLocation();
			return;
		}

		destination = randomDirection.GetPoint(MaxRange *0.75f);

	}

	// This function turns the enemy towards the player
	// It will return a true if the player is in front of the enemy
	// It will return false if the player is not in front of the enemy
	bool TurnTowardsPlayer()
	{
		Vector3 Forward = transform.forward;
		Vector3 PlayerDistance = target.transform.position - transform.position;
		PlayerDistance.y = 0.0f;
		float rotation = 0.0f;
		float angle = Vector3.Angle(PlayerDistance, Forward);
		
		// Rotation
		if (angle > 5.0f)
		{
			if (Vector3.Cross (PlayerDistance, Forward).y > 0)
				rotation = -1 * AngularAcceleration;
			if (Vector3.Cross (PlayerDistance, Forward).y < 0)
				rotation = 1 * AngularAcceleration;
			
			currentRotation += rotation;
			currentRotation = Mathf.Min (currentRotation, AngularAcceleration);
			currentRotation = Mathf.Max (currentRotation, -AngularAcceleration);
			transform.Rotate (0, currentRotation, 0);
		
			return false;
		} 
		
		else
		{
			gameObject.transform.LookAt(target.transform.position);
			return true;
		}
	}

	// This function will search for the nearest safe zone. When found, it will
	// pick a random point inside of the safe zone and set that as the new destination
	void FindRandomLocation()
	{
		closestSafeZone = 0;
		attackCounter = 0;
		
		for (int i = 0; i < safeZones.Length; i++) 
		{
			Vector3 currWaypoint = safeZones[closestSafeZone].transform.position - gameObject.transform.position;
			Vector3 nextWaypoint = safeZones[i].transform.position - gameObject.transform.position;
			
			if(nextWaypoint.magnitude < currWaypoint.magnitude)
			{
				closestSafeZone = i;
			}
		}

		float safeZoneRadius = safeZones[closestSafeZone].GetComponent<SphereCollider>().radius;
		Vector3 randomPosition = Random.insideUnitSphere * safeZoneRadius;
		randomPosition.y = 0;

		destination = safeZones [closestSafeZone].transform.position + randomPosition;
		navigation.updateRotation = true;
		isMoving = true;
	}

	// This function will check if there is any Ranged Minion enemy too close.
	// If there is, then the enemy will find a new location.
	void OnTriggerStay(Collider col)
	{
		if (isMoving == false && col.name.Contains ("Ranged")) 
		{
			FindRandomLocation ();

		}
	}

	void SideStrafe()
	{
		attackCounter = 0;
		Vector3 direction = gameObject.transform.position - target.transform.position;
		float currentDistance = direction.magnitude;
		direction.y = 0;
		direction.Normalize ();
		float variance = Random.Range (-40.0f, 40.0f);
		direction = Quaternion.Euler (0, variance, 0) * direction;
		
		Ray randomDirection = new Ray();
		randomDirection.origin = target.transform.position;
		randomDirection.origin = new Vector3 (randomDirection.origin.x, 0.5f, randomDirection.origin.z);
		randomDirection.direction = direction;
		destination = randomDirection.GetPoint(currentDistance);
	}
}
