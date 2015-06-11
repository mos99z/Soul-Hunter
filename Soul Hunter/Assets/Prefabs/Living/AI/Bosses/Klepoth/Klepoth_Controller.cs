using UnityEngine;
using System.Collections;



public class Klepoth_Controller : MonoBehaviour {

	bool isAttacking = false;
	bool movementState = false;
	bool startedCharging = false;
	Living_Obj healthCheck;
	GameObject target;
	public GameObject TailWhip;
	public float TailDamage = 100.0f;
	public GameObject GroundPound;
	public float GroundPoundDamage = 100.0f;
	public float ChargeDamage = 100.0f;
	public GameObject Carrot;
	Vector3 destination;
	int AttackForm = 0;

	public float AttackCooldown = 10.0f;
	float currentAttackCooldown = 0.0f;
	public float StunTimer = 2.0f;
	float currentStunTimer = 0.0f;
	public float GroundPoundTimer = 1.0f;
	float currentGroundPoundTimer = 0.0f;
	float movementTimer = 0.0f;
	float chargeChargeUp = 1.0f;
	NavMeshAgent navigation;
	
	public Animator Animate = null;
	public GameObject DirectionIndicator = null;
	public GameObject StalactiteDrop = null;
	float debugTimer = 0.0f;
	float groundPoundChecker = 0.5f;
	Vector3 stunnedPosition;
	
	

	enum Attack_State {NULL = -1, CHARGE = 0, GROUND_POUND = 1, TAIL_SWING = 2, SLOWING_DOWN = 3};

	// Use this for initialization
	void Start () 
	{
		if (Animate == null)
			Animate = transform.GetComponentInChildren<Animator> ();
		if (DirectionIndicator == null)
			DirectionIndicator = transform.FindChild ("Direction Indicator").gameObject;

		target = GameObject.FindGameObjectWithTag ("Player");
		healthCheck = GetComponent<Living_Obj> ();
		currentAttackCooldown = AttackCooldown;
		currentStunTimer = StunTimer;
		navigation = GetComponent<NavMeshAgent> ();
		navigation.updateRotation = false;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (isAttacking == false) 
		{
			currentAttackCooldown -= Time.deltaTime;
			TurnTowardsPlayer ();
			if(currentAttackCooldown <= 0.0f)
			{
				Vector3 playerDistance = target.transform.position - gameObject.transform.position;

				if(playerDistance.magnitude < 4.0f)
				{
					AttackForm = (int)Attack_State.GROUND_POUND;
					Debug.Log("Ground Pound");
				}
				else if(playerDistance.magnitude > 4.0f)
				{
					Debug.Log("Charging");
					AttackForm = (int)Attack_State.CHARGE;
					navigation.Stop();
					navigation.autoBraking = false;
				}

				//navigation.SetDestination(gameObject.transform.position);
				isAttacking = true;

				
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
				
				if(movementTimer < 0.0f || navigation.remainingDistance == 0.0f)
				{
					movementState = true;
					FindNewPosition();
					navigation.SetDestination (destination);
				}
				
			}
		}
		
		else if (isAttacking == true) 
		{
			//Debug.Log("Attack");
			isAttacking = true;
			currentAttackCooldown = AttackCooldown;
			FindNewPosition();
			navigation.Resume();
			navigation.speed = 3.5f;
			Attack();
			
			if(isAttacking == false)
			{
				currentAttackCooldown = AttackCooldown;
				FindNewPosition();
				//navigation.updateRotation = true;
			}
				
		}
	}

	void Attack()
	{
		switch (AttackForm) 
		{
		case (int)Attack_State.CHARGE:
		{
			chargeChargeUp -= Time.deltaTime;
			if(chargeChargeUp > 0.0f)
			{
				navigation.SetDestination(target.transform.position);
				TurnTowardsPlayer();
				return;
			}

			if(chargeChargeUp <= 0.0f)
			{
				navigation.Resume();
				navigation.acceleration = 20;
				navigation.speed = 20;
				startedCharging = true;
			}

			if(navigation.remainingDistance <= 0.5f && chargeChargeUp < 0.0f)
			{
				Vector3 newLocation = DirectionIndicator.transform.forward * 7.0f;
				newLocation += gameObject.transform.position;
				AttackForm = (int)Attack_State.SLOWING_DOWN;
				navigation.SetDestination(newLocation);
				navigation.autoBraking = true;
				Debug.Log("Slowing Down");
				navigation.acceleration = 20.0f;
				return;
			}
			break;
		}
		case (int)Attack_State.GROUND_POUND:
		{
			currentGroundPoundTimer -= Time.deltaTime;
			if(currentGroundPoundTimer <= 0.0f)
			{
				GroundPound.SetActive(true);
				currentGroundPoundTimer = GroundPoundTimer;
				AttackForm = (int)Attack_State.NULL;

				groundPoundChecker -= Time.deltaTime;
				if(groundPoundChecker <= 0.0f)
				{
					GroundPound.SetActive(false);
					AttackForm = (int)Attack_State.NULL;
				}
			}
			break;
		}
		case (int)Attack_State.TAIL_SWING:
		{
			currentStunTimer -= Time.deltaTime;
			transform.position = stunnedPosition;
			if(currentStunTimer > StunTimer * 0.75f)
			{
				TailWhip.SetActive(true);
			}
			if(currentStunTimer > StunTimer * 0.5f)
			{
				TailWhip.SetActive(false);
			}
			if(currentStunTimer > StunTimer * 0.25f)
			{
				TailWhip.SetActive(true);
			}
			if(currentStunTimer > StunTimer * 0.0f)
			{
				TailWhip.SetActive(false);
			}
			if(currentStunTimer <= 0.0f)
			{
				AttackForm = (int)Attack_State.NULL;
			}
			break;
		}
		case (int)Attack_State.SLOWING_DOWN:
		{
			if(navigation.velocity == Vector3.zero)
			{
				AttackForm = (int)Attack_State.NULL;
			}
			break;
		}
		case (int)Attack_State.NULL:
		{
			groundPoundChecker = 0.5f;
			currentStunTimer = StunTimer;
			navigation.acceleration = 8;
			Debug.Log("Stopped");
			chargeChargeUp = 1.0f;
			navigation.speed = 3.5f;
			navigation.autoBraking = true;
			navigation.Resume();
			isAttacking = false;
			startedCharging = false;
			Carrot.transform.position = Vector3.zero;
			break;
		}
		}
	}

	void FindNewPosition()
	{
		if (movementState) 
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

	void TurnTowardsPlayer()
	{
		Vector3 movementDirection = navigation.velocity.normalized;
		if (movementDirection.magnitude >= 1.0f) {
			DirectionIndicator.transform.forward = navigation.velocity.normalized;
			float dotProd = Vector3.Dot (new Vector3 (0, 0, 1), movementDirection);
			Vector3 crossProd = Vector3.Cross (new Vector3 (0, 0, 1), movementDirection);
			if (dotProd >= 0.75f)
				Animate.Play ("Klepoth_Idle_Up");
			else if (dotProd <= -0.75f)
				Animate.Play ("Klepoth_Idle_Down");
			else if (dotProd > -0.25f && dotProd <= 0.25f)
			{
				if (crossProd.y < 0.0f)
					Animate.Play ("Klepoth_Idle_Left");
				else
					Animate.Play ("Klepoth_Idle_Right");
			}
			else if (dotProd > 0.25f && dotProd < 0.75f)
			{
				if (crossProd.y < 0.0f)
					Animate.Play ("Klepoth_Idle_UpLeft");
				else
					Animate.Play ("Klepoth_Idle_UpRight");
			}
			else
			{
				if (crossProd.y < 0.0f)
					Animate.Play ("Klepoth_Idle_DownLeft");
				else
					Animate.Play ("Klepoth_Idle_DownRight");
			}
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player" && TailWhip.activeSelf == true) 
		{
			col.SendMessage ("TakeDamage", TailDamage);
		}

		else if (GroundPound.activeSelf == true) 
		{
			if (col.tag == "Player")
				col.SendMessage ("TakeDamage", GroundPoundDamage);

			if(healthCheck.CurrHealth < healthCheck.MaxHealth * 0.5f)
			{
				col.SendMessage ("TakeDamage", GroundPoundDamage);
			}
		} 
		else if (col.tag == "Player" && AttackForm == (int)Attack_State.CHARGE) 
		{
			col.SendMessage ("TakeDamage", ChargeDamage);
			transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
			AttackForm = (int)Attack_State.NULL;
			navigation.speed = 3.5f;
			isAttacking = false;

			if(healthCheck.CurrHealth < healthCheck.MaxHealth * 0.5f)
			{
				col.SendMessage ("TakeDamage", ChargeDamage);
				isAttacking = true;
				AttackForm = (int)Attack_State.GROUND_POUND;
			}
		}
		
		if ((col.tag == "Wall") && (AttackForm == (int)Attack_State.CHARGE || AttackForm == (int)Attack_State.SLOWING_DOWN) && startedCharging) 
		{
			Debug.Log("Swinging Tail");
			AttackForm = (int)Attack_State.TAIL_SWING;
			navigation.Stop();
			stunnedPosition = transform.position;
			transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
			navigation.velocity = Vector3.zero;
			
			Debug.Log("Hit Wall");
			
			if(healthCheck.CurrHealth < healthCheck.MaxHealth * 0.5f)
			{
				Debug.Log("Stalactite Drop");
			}
		}
	}
}
