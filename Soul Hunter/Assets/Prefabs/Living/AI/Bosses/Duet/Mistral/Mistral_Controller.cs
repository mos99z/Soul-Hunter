using UnityEngine;
using System.Collections;
using System.Linq;

public class Mistral_Controller : MonoBehaviour 
{

	public GameObject Perendi;
	public float MinRange = 5.0f;		// Minimum Attack Range
	public float MaxRange = 18.0f;		// Maximum Attack Range
	public LayerMask Mask;				// Layers to ignore

	public GameObject boundingWalls;

	// Movement Variables
	NavMeshAgent navigation;			// Used to allow the minion to use the NavMesh
	public GameObject target;					// Used to know where the player is at all times
	bool isMoving = false;				// a boolean used to prevent the enemy from continuously trying to update position
	GameObject[] safeZones;
	Vector3 destination;				// Location to move to using the NavMesh
	int closestSafeZone;// = 0;			
	public float MinWaitTime = 5.0f;
	public float MaxWaitTime = 7.0f;
	bool perendiAlive = true;
	
	// Attack Variables
	public float AttackCooldown = 2.0f;
	public GameObject Whirlwind = null;
	float currentAttackTimer = 0.0f;
	float currentWhirlwindTimer = 0.0f;

	public GameObject[] attackingWaypoints;	// Used to rotate around the player
	
	float waypointTimer = 0.0f;
	float lungeTimer = 0.0f;
	int closestShadow = 0;				
	public bool isAttacking = false;
	public float WaitMinimum = 5;
	public float WaitMaximum = 7;
	public float AttackTimer = 5.0f;
	public float Damage = 100.0f;
	public SphereCollider AttackCollider;
	public GameObject DirectionIndicator = null;
	
//	private Living_Obj healthScript = null;

	//animation stuff
	public Animator Animate = null;
	public GameObject spriteImage;
	
	// Use this for initialization
	void Start () 
	{
		navigation = GetComponent<NavMeshAgent> ();
		navigation.updateRotation = false;
		target = GameObject.FindGameObjectWithTag ("Player");
		safeZones = GameObject.FindGameObjectsWithTag ("SafeZone");
		boundingWalls.SendMessage("ActivateWalls");
		if (DirectionIndicator == null)
			DirectionIndicator = transform.FindChild ("Direction Indicator").gameObject;
//		healthScript = gameObject.GetComponent<Living_Obj> ();
	}

	// Update is called once per frame
	void Update () 
	{

		if (perendiAlive) 
		{
			if (CheckPlayerDistance () == true && isMoving == false) 
			{
				TurnTowardsPlayer ();
				currentAttackTimer -= Time.deltaTime;
				
				if (currentAttackTimer <= 0.0f) 
				{
					/*GameObject Hurricane = */GameObject.Instantiate (Whirlwind, target.transform.position,target.transform.rotation);
					//Hurricane.transform.position = target.transform.position;
					currentAttackTimer = AttackCooldown;
					SideStrafe ();
				}
				isMoving = true;
			} 
			else if (isMoving == false)
			{
				Reposition ();
				isMoving = true;
				
			}
			else if (isMoving == true) 
			{
				navigation.SetDestination (destination);
				if (navigation.remainingDistance == 0) 
				{
					isMoving = false;
				}
			}
			
		}
		
		
		else 
		{
			TurnTowardsPlayer ();

			currentWhirlwindTimer -= Time.deltaTime;
			
			if (currentWhirlwindTimer <= 0.0f) 
			{
				GameObject.Instantiate (Whirlwind, target.transform.position,target.transform.rotation);
				currentWhirlwindTimer = AttackCooldown;
			}

			if (isAttacking == false) 
			{
				navigation.SetDestination (destination);
				currentAttackTimer -= Time.deltaTime;
				waypointTimer -= Time.deltaTime;
				if (navigation.remainingDistance <= 0.3f && waypointTimer <= 0.0f) {
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
					currentAttackTimer = AttackTimer;
					navigation.autoBraking = true;
					navigation.stoppingDistance = 4.0f;
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
					//SearchForNearestNode();
					destination = attackingWaypoints[closestShadow].transform.position;
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

		return false;
	}
	
	// Search for a new viable position. If no viable position, will search for the nearest safe zone
	void Reposition()
	{
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
	void TurnTowardsPlayer()
	{
		Vector3 movementDirection = target.transform.position - this.transform.position;
		movementDirection = movementDirection.normalized;
		if (movementDirection.magnitude >= 1.0f)
		{
			DirectionIndicator.transform.forward = movementDirection.normalized;
			float dotProd = Vector3.Dot (new Vector3 (0, 0, 1), movementDirection);
			Vector3 crossProd = Vector3.Cross (new Vector3 (0, 0, 1), movementDirection);
			if (dotProd >= 0.75f)
			{

				Animate.Play ("Duel_Move_Up");
			}
			else if (dotProd <= -0.75f)
			{

				Animate.Play ("Duel_Move_Down");
			}
			else if (dotProd > -0.25f && dotProd <= 0.25f)
			{
				if (crossProd.y < 0.0f)
				{
					Animate.Play ("Duel_Move_Left");
				}
				else
				{
					Animate.Play ("Duel_Move_Right");
				}
			}
			else if (dotProd > 0.25f && dotProd < 0.75f)
			{
				if (crossProd.y < 0.0f)
				{
					Animate.Play ("Duel_Move_UpLeft");
				}
				else
				{
					Animate.Play ("Duel_Move_UpRight");
				}
			}
			else
			{
				if (crossProd.y < 0.0f)
				{
					Animate.Play ("Duel_Move_DownLeft");
				}
				else
				{
					Animate.Play ("Duel_Move_DownRight");
				}
			}
		}
	}
	
	// This function will search for the nearest safe zone. When found, it will
	// pick a random point inside of the safe zone and set that as the new destination
	void FindRandomLocation()
	{
		closestSafeZone = 0;
		
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
		isMoving = true;
	}
	

	
	void SideStrafe()
	{
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

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") 
		{
			col.SendMessage("TakeDamage",Damage);
		}
	}

	void OtherBossDead()
	{
		perendiAlive = false;
		currentWhirlwindTimer = AttackCooldown;
		attackingWaypoints = GameObject.FindGameObjectsWithTag ("Shadow").OrderBy(waypoint => waypoint.name).ToArray<GameObject>();
		navigation.speed = 8.0f;
		gameObject.GetComponent<Living_Obj>().SoulValue = SoulType.Red;
	}

	void OnDestroy()
	{
		if(perendiAlive)
			Perendi.SendMessage ("OtherBossDead");
		if (!perendiAlive) 
		{
			boundingWalls.SendMessage("DestroyWalls");
			GameBrain.Instance.SendMessage("ChangeMusic",GameBrain.Instance.GameplayMusic);
		}
	}

	void PlayerDead()
	{
		target = null;
	}
}
