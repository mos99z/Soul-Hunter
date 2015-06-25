using UnityEngine;
using System.Collections;

public class Mage_Captain_Controller : MonoBehaviour {

	public float MinRange = 5.0f;		// Minimum Attack Range
	public float MaxRange = 18.0f;		// Maximum Attack Range
	public LayerMask Mask;				// Layers to ignore
	
	// Movement Variables
	NavMeshAgent navigation;			// Used to allow the minion to use the NavMesh
	public GameObject target;					// Used to know where the player is at all times
	bool isMoving = false;				// a boolean used to prevent the enemy from continuously trying to update position
	bool isCastingSpellBarrage = false;	// a boolean used to check if the Mage Captain is casting spell barrage 
	bool isCastingAOE = false;
	GameObject[] safeZones;
	Vector3 destination;				// Location to move to using the NavMesh
	int closestSafeZone;// = 0;			// 
	Vector3 playerDistance;
	public GameObject boundingWalls;	// walls to summon when facing captain

	// Attack Variables
	int attackCounter = 0;
	public float MinAttackCooldown = 2.0f;
	public float MaxAttackCooldown = 4.0f;
	public float SpellBarrageCooldown = 10.0f;
	public float AOECooldown = 5.0f;
	public GameObject FelMissile;
	public GameObject SpellBarrage;
	public GameObject AOE;
	public float missileDamage, spellDamage, aoeDamage;		// needed for crippled controller
	float currentAttackTimer = 0.0f;
	float currentSpellBarrageTimer = 0.0f;
	float AOEChargeUp = 0.5f;
	float currentAOETimer = 0.0f;
	public GameObject DirectionIndicator = null;
	GameObject player;

	//animation stuff
	public Animator Animate = null;
	private bool underMelee = false;
	private float attackTicker = 0;
	public float attackLength = 0;
	public GameObject spriteImage;

	public AudioSource RangedSFX;
	public AudioSource SpellBarrageSFX;
	
	// Use this for initialization
	void Start () 
	{
		Fog_Event_Manager.PlayerEntered += LosePlayer;
		Fog_Event_Manager.PlayerLeft += FindPlayer;
		navigation = GetComponent<NavMeshAgent> ();
		navigation.updateRotation = false;
		player = GameBrain.Instance.Player;
		target = player;
		safeZones = GameObject.FindGameObjectsWithTag ("SafeZone");
		currentSpellBarrageTimer = SpellBarrageCooldown;
		missileDamage = FelMissile.GetComponent<Fel_Missile_Controller> ().Damage;
		spellDamage = SpellBarrage.GetComponent<Spell_Barrage_Controller> ().Damage;
		aoeDamage = AOE.GetComponent<AOE_Controller> ().Damage;
		boundingWalls.SendMessage("ActivateWalls");
		if (DirectionIndicator == null)
			DirectionIndicator = transform.FindChild ("Direction Indicator").gameObject;
	}

	void OnDestroy()
	{
		boundingWalls.GetComponent<SummonWall> ().DestroyWalls ();
		Fog_Event_Manager.PlayerEntered -= LosePlayer;
		Fog_Event_Manager.PlayerLeft -= FindPlayer;
		GameBrain.Instance.SendMessage("ChangeMusic",GameBrain.Instance.GameplayMusic);
		GameBrain.Instance.FightingCaptain = false;
	}

	// Update is called once per frame
	// The enemy will attack if the player is within range and is not currently moving towards a waypoint
	// The enemy will move if: the player moves out of his comfort range; the enemy gets too close to a wall
	// and the player approaches; and after attacking.
	// If the player gets too close to the enemy, the enemy will perform an AOE blast around himself to damage the player
	// Every 10 seconds, the enemy will cast a spell barrage, regardless of his current state
	void Update () 
	{
		if (underMelee)
		{
			attackTicker += Time.deltaTime;
			if (attackTicker >= attackLength)
			{
				underMelee = false;
				attackTicker = 0;
			}
		}

		currentAOETimer -= Time.deltaTime;

		if (isCastingSpellBarrage == false) 
		{
			currentSpellBarrageTimer -= Time.deltaTime;
			if(currentSpellBarrageTimer <= 0)
			{
				isCastingSpellBarrage = true;
				underMelee = true;
				return;
			}
			
			if(isCastingAOE == false)
			{
				TurnTowardsPlayer();
				if (CheckPlayerDistance () == true && isMoving == false) 
				{
					currentAttackTimer -= Time.deltaTime;
					Vector3 lookAtPlayer = target.transform.position - transform.position;
					DirectionIndicator.transform.forward = lookAtPlayer.normalized;

					if(currentAttackTimer <= 0.0f)
					{
						underMelee = true;
						Vector3 startLoc = transform.position;
						startLoc.y = 1.5f;
						RangedSFX.Play ();
						GameObject missile = (GameObject)GameObject.Instantiate(FelMissile, startLoc, transform.rotation);
						missile.GetComponent<Fel_Missile_Controller>().Damage = missileDamage; 		// needed for cripple
						//RangedAttack.transform.position = startLoc;0
						//Vector3 newForward = (target.transform.position - transform.position);
						//newForward.y = 0.0f;
						//newForward.Normalize();
						//RangedAttack.transform.forward = newForward;
						
						attackCounter++;
						currentAttackTimer = Random.Range(MinAttackCooldown,MaxAttackCooldown);
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
			
			if(playerDistance.magnitude <= 1.5f && isCastingAOE == false && currentAOETimer <= 0.0f)
			{
				underMelee = true;
				isCastingAOE = true;
				AOE.SetActive(true);
				AOE.GetComponent<AOE_Controller>().Damage = aoeDamage;		// needed for cripple
				currentAOETimer = AOEChargeUp;
				Debug.Log ("AOE Enabled");
			}
			
			if(isCastingAOE == true)
			{
				navigation.SetDestination(gameObject.transform.position);
				
				
				if(currentAOETimer <= 0.0f)
				{
					Debug.Log ("AOE Disabled");
					AOE.SetActive(false);
					currentAOETimer = AOECooldown;
					isCastingAOE = false;
				}
			}
		}
		
		else
		{
			navigation.SetDestination(gameObject.transform.position);
			if(navigation.remainingDistance == 0)
			{
				Vector3 lookAtPlayer = target.transform.position - transform.position;
				DirectionIndicator.transform.forward = lookAtPlayer.normalized;

				Vector3 startLoc = transform.position;
				startLoc.y = 1.5f;
				SpellBarrageSFX.Play();

				GameObject spell = (GameObject)GameObject.Instantiate(SpellBarrage, startLoc, transform.rotation);
				spell.GetComponent<Spell_Barrage_Controller>().Damage = spellDamage;		// needed for cripple
				isCastingSpellBarrage = false;
				currentSpellBarrageTimer = SpellBarrageCooldown;
				
			}
		}
	}
	
	// This function will check the distance of the player. It will return true if
	// the player is between the min and max range. It will return false if the
	// player is outside of that zone
	bool CheckPlayerDistance()
	{
		playerDistance = target.transform.position - gameObject.transform.position;
		if (playerDistance.magnitude > MinRange && playerDistance.magnitude < MaxRange)
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
		if (underMelee)
		{
			movementDirection = player.transform.position - this.transform.position;
			movementDirection = movementDirection.normalized;
		}
		if (movementDirection.magnitude >= 1.0f)
		{
			DirectionIndicator.transform.forward = movementDirection.normalized;
			float dotProd = Vector3.Dot (new Vector3 (0, 0, 1), movementDirection);
			Vector3 crossProd = Vector3.Cross (new Vector3 (0, 0, 1), movementDirection);
			if (underMelee)
			{
				Vector3 tempPos = this.gameObject.transform.position;
				tempPos.y = 4.87f;
				tempPos.z -= 1;
				spriteImage.transform.position = tempPos;
			}
			else
			{
				Vector3 tempPos = this.gameObject.transform.position;
				tempPos.y = 3.89f;
				tempPos.z -= 0.5f;
				spriteImage.transform.position = tempPos;
			}
			if (dotProd >= 0.75f)
			{
				if (underMelee)
				{
					Animate.Play ("Mage_Slash_Up");
				}
				else
				{
					Animate.Play ("Mage_Move_Up");
				}
			}
			else if (dotProd <= -0.75f)
			{
				if (underMelee)
				{
					Animate.Play ("Mage_Slash_Down");
				}
				else
				{
					Animate.Play ("Mage_Move_Down");
				}
			}
			else if (dotProd > -0.25f && dotProd <= 0.25f)
			{
				if (crossProd.y < 0.0f)
				{
					if (underMelee)
					{
						Animate.Play ("Mage_Slash_Left");
					}
					else
					{
						Animate.Play ("Mage_Move_Left");
					}
				}
				else
				{
					if (underMelee)
					{
						Animate.Play ("Mage_Slash_Right");
					}
					else
					{
						Animate.Play ("Mage_Move_Right");
					}
				}
			}
			else if (dotProd > 0.25f && dotProd < 0.75f)
			{
				if (crossProd.y < 0.0f)
				{
					if (underMelee)
					{
						Animate.Play ("Mage_Slash_UpLeft");
					}
					else
					{
						Animate.Play ("Mage_Move_UpLeft");
					}
				}
				else
				{
					if (underMelee)
					{
						Animate.Play ("Mage_Slash_UpRight");
					}
					else
					{
						Animate.Play ("Mage_Move_UpRight");
					}
				}
			}
			else
			{
				if (crossProd.y < 0.0f)
				{
					if (underMelee)
					{
						Animate.Play ("Mage_Slash_DownLeft");
					}
					else
					{
						Animate.Play ("Mage_Move_DownLeft");
					}
				}
				else
				{
					if (underMelee)
					{
						Animate.Play ("Mage_Slash_DownRight");
					}
					else
					{
						Animate.Play ("Mage_Move_DownRight");
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

	// While attacking, if the enemy is still inside of his preferred attack range, instead of
	// moving towards 75% of the max distance, it will strafe to the side instead
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
}
