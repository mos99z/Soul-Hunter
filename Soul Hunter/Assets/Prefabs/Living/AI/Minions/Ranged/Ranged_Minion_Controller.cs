using UnityEngine;
using System.Collections;

public class Ranged_Minion_Controller : MonoBehaviour {

	public float MinRange = 5.0f;		// Minimum Attack Range
	public float MaxRange = 18.0f;		// Maximum Attack Range
	public LayerMask Mask;				// Layers to ignore

	// Movement Variables
	NavMeshAgent navigation;			// Used to allow the minion to use the NavMesh
	GameObject player;
	GameObject target;					// Used to know where the player is at all times
	bool isMoving = false;				// a boolean used to prevent the enemy from continuously trying to update position
	GameObject[] safeZones;
	Vector3 destination;				// Location to move to using the NavMesh
	int closestSafeZone;// = 0;			

	// Attack Variables
	int attackCounter = 0;
	public float AttackCooldown = 2.0f;
	public GameObject FelMissile = null;
	public float missleDamage;			// This is used for the cripple debuff
	float currentAttackTimer = 0.0f;
	public bool isFrozen = false;		// used for frozen debuff
	public GameObject DirectionIndicator = null;

	//animation stuff
	public Animator Animate = null;
	private bool underMelee = false;
	private float attackTicker = 0;
	public float attackLength = 0;
	public GameObject spriteImage;

	public AudioSource RangedSFX;

	// Use this for initialization
	void Start () 
	{
		if (Animate == null)
			Animate = transform.GetComponentInChildren<Animator>();

		Fog_Event_Manager.PlayerEntered += LosePlayer;
		Fog_Event_Manager.PlayerLeft += FindPlayer;
		navigation = GetComponent<NavMeshAgent> ();
		navigation.updateRotation = false;
		player = GameBrain.Instance.Player;
		if (GameBrain.Instance.PlayerInFog == true)
		{
			LosePlayer();
		}
		else
			target = player;

		safeZones = GameObject.FindGameObjectsWithTag ("SafeZone");

		if (FelMissile)
			missleDamage = FelMissile.GetComponent<Fel_Missile_Controller>().Damage;
		if (DirectionIndicator == null)
			DirectionIndicator = transform.FindChild ("Direction Indicator").gameObject;
	}

	// Update is called once per frame
	void Update () 
	{
		TurnTowardsPlayer();

		if (underMelee)
		{
			attackTicker += Time.deltaTime;
			if (attackTicker >= attackLength)
			{
				underMelee = false;
			}
		}

		if (isFrozen)
			return;

		if (CheckPlayerDistance () == true && isMoving == false && attackCounter < 3) 
		{
			currentAttackTimer -= Time.deltaTime;
			Vector3 lookAtPlayer = target.transform.position - transform.position;
			DirectionIndicator.transform.forward = lookAtPlayer.normalized;


			if(currentAttackTimer <= 0.0f)
			{
				//Debug.Log("Enemy Attacked");
				Vector3 startLoc = transform.position;
				startLoc.y = 1.5f;
				RangedSFX.Play();
				GameObject RangedAttack = GameObject.Instantiate(FelMissile);
				RangedAttack.GetComponent<Fel_Missile_Controller>().Damage = missleDamage;	// this assignment is necessary for cripple
				RangedAttack.transform.position = startLoc;
				Vector3 newForward = (target.transform.position - transform.position);
				newForward.y = 0.0f;
				newForward.Normalize();
				RangedAttack.transform.forward = newForward;
				attackCounter++;
				currentAttackTimer = AttackCooldown;
				underMelee = true;
				attackTicker = 0;
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
	void TurnTowardsPlayer()
	{
		Vector3 movementDirection = navigation.velocity.normalized;
		if (movementDirection.magnitude >= 1)
		{
			DirectionIndicator.transform.forward = navigation.velocity.normalized;
			float dotProd = Vector3.Dot (new Vector3 (0, 0, 1), movementDirection);
			Vector3 crossProd = Vector3.Cross (new Vector3 (0, 0, 1), movementDirection);
			if (navigation.speed > 0.25f)
			{
				Vector3 tempPos = this.gameObject.transform.position;
				tempPos.y = 1.19f;
				spriteImage.transform.position = tempPos;
			}
			if (underMelee)
			{
				Vector3 tempPos = this.gameObject.transform.position;
				tempPos.y = 1.01f;
				spriteImage.transform.position = tempPos;
			}
			if (dotProd >= 0.75f)
			{
				if (underMelee)
				{
					Animate.Play ("Range_Slash_Up");
				}
				else
				{
					if (navigation.remainingDistance <= 0.05f)
					{
						Animate.Play ("Range_Idle_Up");
					}
					else
					{
						Animate.Play ("Range_Move_Up");
					}
				}
			}
			else if (dotProd <= -0.75f)
			{
				if (underMelee)
				{
					Animate.Play ("Range_Slash_Down");
				}
				else
				{
					if (navigation.remainingDistance <= 0.05f)
					{
						Animate.Play ("Range_Idle_Down");
					}
					else
					{
						Animate.Play ("Range_Move_Down");
					}
				}
			}
			else if (dotProd > -0.25f && dotProd <= 0.25f)
			{
				if (crossProd.y < 0.0f)
				{
					if (underMelee)
					{
						Animate.Play ("Range_Slash_Left");
					}
					else
					{
						if (navigation.remainingDistance <= 0.05f)
						{
							Animate.Play ("Range_Idle_Left");
						}
						else
						{
							Animate.Play ("Range_Move_Left");
						}
					}
				}
				else
				{
					if (underMelee)
					{
						Animate.Play ("Range_Slash_Right");
					}
					else
					{
						if (navigation.remainingDistance <= 0.05f)
						{
							Animate.Play ("Range_Idle_Right");
						}
						else
						{
							Animate.Play ("Range_Move_Right");
						}
					}
				}
			}
			else if (dotProd > 0.25f && dotProd < 0.75f)
			{
				if (crossProd.y < 0.0f)
				{
					if (underMelee)
					{
						Animate.Play ("Range_Slash_UpLeft");
					}
					else
					{
						if (navigation.remainingDistance <= 0.05f)
						{
							Animate.Play ("Range_Idle_UpLeft");
						}
						else
						{
							Animate.Play ("Range_Move_UpLeft");
						}
					}
				}
				else
				{
					if (underMelee)
					{
						Animate.Play ("Range_Slash_UpRight");
					}
					else
					{
						if (navigation.remainingDistance <= 0.05f)
						{
							Animate.Play ("Range_Idle_UpRight");
						}
						else
						{
							Animate.Play ("Range_Move_UpRight");
						}
					}
				}
			}
			else
			{
				if (crossProd.y < 0.0f)
				{
					if (underMelee)
					{
						Animate.Play ("Range_Slash_DownLeft");
					}
					else
					{
						if (navigation.remainingDistance <= 0.05f)
						{
							Animate.Play ("Range_Idle_DownLeft");
						}
						else
						{
							Animate.Play ("Range_Move_DownLeft");
						}
					}
				}
				else
				{
					if (underMelee)
					{
						Animate.Play ("Range_Slash_DownRight");
					}
					else
					{
						if (navigation.remainingDistance <= 0.05f)
						{
							Animate.Play ("Range_Idle_DownRight");
						}
						else
						{
							Animate.Play ("Range_Move_DownRight");
						}
					}
				}
			}
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

	void LosePlayer()
	{
		GameObject fakePlayer = player;
		fakePlayer.transform.position += Random.insideUnitSphere * 3.5f;
		fakePlayer.transform.position = new Vector3(fakePlayer.transform.position.x,0,fakePlayer.transform.position.z);
		target = fakePlayer;
	}

	void FindPlayer()
	{
		target = player;
	}

	void OnDestroy()
	{
		Fog_Event_Manager.PlayerEntered -= LosePlayer;
		Fog_Event_Manager.PlayerLeft -= FindPlayer;
	}
}
