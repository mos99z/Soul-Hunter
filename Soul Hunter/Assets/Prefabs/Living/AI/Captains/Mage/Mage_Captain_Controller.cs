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
	float currentRotation = 0.0f;
	float AngularAcceleration = 3.5f;
	Vector3 playerDistance;
	
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
	
	
	// Use this for initialization
	void Start () 
	{
		navigation = GetComponent<NavMeshAgent> ();
		target = GameObject.FindGameObjectWithTag ("Player");
		safeZones = GameObject.FindGameObjectsWithTag ("SafeZone");
		currentSpellBarrageTimer = SpellBarrageCooldown;
		missileDamage = FelMissile.GetComponent<Fel_Missile_Controller> ().Damage;
		spellDamage = SpellBarrage.GetComponent<Spell_Barrage_Controller> ().Damage;
		aoeDamage = AOE.GetComponent<AOE_Controller> ().Damage;
	}
	
	// Update is called once per frame
	// The enemy will attack if the player is within range and is not currently moving towards a waypoint
	// The enemy will move if: the player moves out of his comfort range; the enemy gets too close to a wall
	// and the player approaches; and after attacking.
	// If the player gets too close to the enemy, the enemy will perform an AOE blast around himself to damage the player
	// Every 10 seconds, the enemy will cast a spell barrage, regardless of his current state
	void Update () 
	{
		currentAOETimer -= Time.deltaTime;

		if (target == null) {
		}

		else 
		{
			if (isCastingSpellBarrage == false) 
			{
				currentSpellBarrageTimer -= Time.deltaTime;
				if(currentSpellBarrageTimer <= 0)
				{
					isCastingSpellBarrage = true;
					navigation.updateRotation = false;
					return;
				}
				
				if(isCastingAOE == false)
				{
					if (CheckPlayerDistance () == true && isMoving == false) 
					{
						TurnTowardsPlayer();
						currentAttackTimer -= Time.deltaTime;
						
						if(currentAttackTimer <= 0.0f)
						{
							Vector3 startLoc = transform.position;
							startLoc.y = 1.5f;
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
								navigation.updateRotation = false;
							}
							
						}
					}
				}
				
				if(playerDistance.magnitude <= 1.5f && isCastingAOE == false && currentAOETimer <= 0.0f)
				{
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
					if(TurnTowardsPlayer())
					{
						Vector3 startLoc = transform.position;
						startLoc.y = 1.5f;
						GameObject spell = (GameObject)GameObject.Instantiate(SpellBarrage, startLoc, transform.rotation);
						spell.GetComponent<Spell_Barrage_Controller>().Damage = spellDamage;		// needed for cripple
						isCastingSpellBarrage = false;
						currentSpellBarrageTimer = SpellBarrageCooldown;
						navigation.updateRotation = true;
					}
				}
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

	void PlayerDead()
	{
		target = null;
	}
}
