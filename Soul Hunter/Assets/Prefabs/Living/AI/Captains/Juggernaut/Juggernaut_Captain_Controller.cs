using UnityEngine;
using System.Collections;

public class Juggernaut_Captain_Controller : MonoBehaviour 
{
	public GameObject MeleeAttack;
	public LayerMask WallCheck;
	public GameObject target;
	public float Damage = 100.0f;

	public GameObject boundingWalls;		// use this to summon bounding walls

	GameObject[] safeZones;
	int closestSafeZone;
	NavMeshAgent navigation;
	Vector3 destination;
	float movementTimer = 0.0f;
	float currentRotation = 0.0f;
	float AngularAcceleration = 3.5f;
	public float AttackTimer = 2.0f;
	float currentAttackTimer = 0.0f;
	public float ChargeCooldown = 10.0f;
	public GameObject Carrot;
	float currentChargeCooldown = 0.0f;
	float collisionTimer = 0.0f;
	bool movementState = false;
	bool hasHitPlayer = false;
	bool hasTurnedOffAttackArea = false;
	bool isCharging = false;
	bool hasCollided = false;
	bool hasAttacked = false;
	public Animator Animate = null;
	public GameObject DirectionIndicator = null;
	
	// Use this for initialization
	void Start () 
	{
		if (Animate == null)
			Animate = transform.GetComponentInChildren<Animator> ();
		if (DirectionIndicator == null)
			DirectionIndicator = transform.FindChild ("Direction Indicator").gameObject;
		target = GameObject.FindGameObjectWithTag ("Player");
		safeZones = GameObject.FindGameObjectsWithTag ("SafeZone");
		
		navigation = GetComponent<NavMeshAgent> ();
		currentChargeCooldown = ChargeCooldown;
		movementTimer = 5.0f;
		FindNewPosition ();
		navigation.updateRotation = false;
		FindNewPosition ();
		boundingWalls.SendMessage("ActivateWalls");
	}

	void OnDestroy()
	{
		boundingWalls.SendMessage("DestroyWalls");
	}
	// Update is called once per frame
	void Update () 
	{
		//this.gameObject.transform.position = new Vector3(gameObject.transform.position.x,0,gameObject.transform.position.z);
		currentChargeCooldown -= Time.deltaTime;

		if (isCharging == false) 
		{
			Vector3 playerDistance = target.transform.position - gameObject.transform.position;
			TurnTowardsPlayer ();
			if(currentChargeCooldown <= 0.0f && playerDistance.magnitude > 4.0f)
			{
				isCharging = true;
				navigation.velocity = Vector3.zero;
				DirectionIndicator.transform.forward = playerDistance.normalized;
				
				navigation.speed = 50.0f;
				navigation.acceleration = 50.0f;
				navigation.autoBraking = false;

				return;
			}

			if (movementState) 
			{
				if(navigation.remainingDistance == 0.0f)
				{
					movementState = false;
					FindNewPosition();
					navigation.SetDestination (destination);
					movementTimer = 5.0f;
				}
			}
			else
			{
				movementTimer -= Time.deltaTime;
				
				if(movementTimer <= 0.0f || navigation.remainingDistance == 0.0f)
				{
					movementState = true;
					FindNewPosition();
					navigation.SetDestination (destination);
				}
				
			}
		}

		else if (isCharging == true) 
		{
			if(hasCollided == false)
			{
				navigation.SetDestination(Carrot.transform.position);

			}

			else if (hasCollided == true)
			{

				collisionTimer -= Time.deltaTime;
				if(collisionTimer <= 0.0f)
				{
					FindNewPosition();
					isCharging = false;
					hasCollided = false;
					currentChargeCooldown = ChargeCooldown;
					navigation.Resume();
				}
			}


		}


		CheckForAttack ();
	}

	void FindNewPosition()
	{
		if (hasAttacked) 
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
			movementState = false;
			movementTimer = 1.0f;
		}

		else if (movementState) 
		{
			Vector3 direction = gameObject.transform.position - target.transform.position;
			float currentDistance = direction.magnitude;
			direction.y = 0;
			direction.Normalize ();
			float variance = Random.Range (-5.0f, 5.0f);
			direction = Quaternion.Euler (0, variance, 0) * direction;
			
			Ray randomDirection = new Ray();
			randomDirection.origin = target.transform.position;
			randomDirection.origin = new Vector3 (randomDirection.origin.x, 0.0f, randomDirection.origin.z);
			randomDirection.direction = direction;
			destination = randomDirection.GetPoint(currentDistance);

		}

		else 
		{
			destination = target.transform.position;
		}
	}

	void CheckForAttack()
	{
		if (currentAttackTimer > 0.0f) 
		{
			currentAttackTimer -= Time.deltaTime;

			if(hasTurnedOffAttackArea == false && currentAttackTimer <= (AttackTimer - 0.5f))
			{
				hasTurnedOffAttackArea = true;
				MeleeAttack.SetActive(false);
				hasHitPlayer = false;

			}

			if(currentAttackTimer <= 0.0f)
			{
				hasTurnedOffAttackArea = false;
			}
		}
		Vector3 playerDistance = target.transform.position - gameObject.transform.position;
		if (playerDistance.magnitude < 2.5f && currentAttackTimer <= 0.0f) 
		{
			MeleeAttack.SetActive(true);
			hasAttacked = true;
			currentAttackTimer = AttackTimer;
		}
	}

	void TurnTowardsPlayer()
	{
		Vector3 movementDirection = navigation.velocity.normalized;
		if (movementDirection.magnitude >= 1.0f) {
			DirectionIndicator.transform.forward = navigation.velocity.normalized;
			float dotProd = Vector3.Dot (new Vector3 (0, 0, 1), movementDirection);
			Vector3 crossProd = Vector3.Cross (new Vector3 (0, 0, 1), movementDirection);
			//if (dotProd >= 0.75f)
			//	Animate.Play ("Melee_Idle_Up");
			//else if (dotProd <= -0.75f)
			//	Animate.Play ("Melee_Idle_Down");
			//else if (dotProd > -0.25f && dotProd <= 0.25f)
			//{
			//	if (crossProd.y < 0.0f)
			//		Animate.Play ("Melee_Idle_Left");
			//	else
			//		Animate.Play ("Melee_Idle_Right");
			//}
			//else if (dotProd > 0.25f && dotProd < 0.75f)
			//{
			//	if (crossProd.y < 0.0f)
			//		Animate.Play ("Melee_Idle_UpLeft");
			//	else
			//		Animate.Play ("Melee_Idle_UpRight");
			//}
			//else
			//{
			//	if (crossProd.y < 0.0f)
			//		Animate.Play ("Melee_Idle_DownLeft");
			//	else
			//		Animate.Play ("Melee_Idle_DownRight");
			//}
		}

		//Vector3 Forward = transform.forward;
		//Vector3 PlayerDistance = target.transform.position - transform.position;
		//PlayerDistance.y = 0.0f;
		//float rotation = 0.0f;
		//float angle = Vector3.Angle(PlayerDistance, Forward);
		//
		//// Rotation
		//if (angle > 5.0f)
		//{
		//	if (Vector3.Cross (PlayerDistance, Forward).y > 0)
		//		rotation = -1 * AngularAcceleration;
		//	if (Vector3.Cross (PlayerDistance, Forward).y < 0)
		//		rotation = 1 * AngularAcceleration;
		//	
		//	currentRotation += rotation;
		//	currentRotation = Mathf.Min (currentRotation, AngularAcceleration);
		//	currentRotation = Mathf.Max (currentRotation, -AngularAcceleration);
		//	transform.Rotate (0, currentRotation, 0);
		//	
		//	return false;
		//} 
		//
		//else
		//{
		//	gameObject.transform.LookAt(target.transform.position);
		//	return true;
		//}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player" && hasHitPlayer == false) 
		{
			col.SendMessage("TakeDamage",Damage);
			hasHitPlayer = true;
		}

		if ((col.tag == "Wall" || col.tag == "Player") && isCharging == true) 
		{
			collisionTimer = 1.0f;
			navigation.speed = 3.5f;
			navigation.acceleration = 8.0f;
			hasCollided = true;
			navigation.autoBraking = true;
			transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}

	//void OnCollisionEnter(Collision col)
	//{
	//	if ((col.gameObject.tag == "Wall" || col.gameObject.tag == "Player") && isCharging == true) 
	//	{
	//		collisionTimer = 1.0f;
	//		hasCollided = true;
	//		transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
	//	}
	//}
}
