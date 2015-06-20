using UnityEngine;
using System.Collections;

public class Juggernaut_Captain_Controller : MonoBehaviour 
{
	public GameObject MeleeAttack;
	public LayerMask WallCheck;
	public GameObject target;
	public float Damage = 100.0f;

	public GameObject boundingWalls;		// use this to summon bounding walls

	NavMeshAgent navigation;
	Vector3 destination;
	public float AttackTimer = 2.0f;
	float currentAttackTimer = 0.0f;
	public float ChargeCooldown = 10.0f;
	public float ChargeChargeUp = 1.0f;
	float currentChargeChargeUp = 0.0f;
	public GameObject Carrot;
	float currentChargeCooldown = 0.0f;
	float collisionTimer = 0.0f;
	bool hasHitPlayer = false;
	bool hasTurnedOffAttackArea = false;
	public bool isCharging = false;
	bool hasChargedUp = false;
	bool hasCollided = false;
	public Animator Animate = null;
	public GameObject DirectionIndicator = null;
	GameObject player;

	//animation stuff
	private bool underMelee = false;
	private float attackTicker = 0;
	public float attackLength = 0;
	public GameObject spriteImage;
	
	// Use this for initialization
	void Start () 
	{
		Fog_Event_Manager.PlayerEntered += LosePlayer;
		Fog_Event_Manager.PlayerLeft += FindPlayer;
		if (Animate == null)
			Animate = transform.GetComponentInChildren<Animator> ();
		if (DirectionIndicator == null)
			DirectionIndicator = transform.FindChild ("Direction Indicator").gameObject;
		player = GameBrain.Instance.Player;
		target = player;
		
		navigation = GetComponent<NavMeshAgent> ();
		currentChargeCooldown = ChargeCooldown;
		currentChargeChargeUp = ChargeChargeUp;
		navigation.updateRotation = false;
		boundingWalls.SendMessage("ActivateWalls");
	}

	void OnDestroy()
	{
		boundingWalls.SendMessage("DestroyWalls");
		Fog_Event_Manager.PlayerEntered -= LosePlayer;
		Fog_Event_Manager.PlayerLeft -= FindPlayer;
	}
	// Update is called once per frame
	void Update () 
	{
		if (underMelee)
		{
			attackTicker += Time.deltaTime;
			if (attackTicker >= attackLength)
			{
				underMelee = false;
			}
		}

		//this.gameObject.transform.position = new Vector3(gameObject.transform.position.x,0,gameObject.transform.position.z);
		currentChargeCooldown -= Time.deltaTime;

		if (isCharging == false) 
		{
			Vector3 playerDistance = target.transform.position - gameObject.transform.position;
			TurnTowardsPlayer ();
			if(hasChargedUp == false && currentChargeCooldown <= 0.0f && playerDistance.magnitude > 4.0f)
			{
				currentChargeChargeUp -= Time.deltaTime;
				navigation.SetDestination(gameObject.transform.position);
				DirectionIndicator.transform.forward = playerDistance.normalized;
				if(currentChargeChargeUp <= 0.0f)
					hasChargedUp = true;
				return;
			}
			if(hasChargedUp)
			{
				isCharging = true;
				hasChargedUp = false;
				navigation.velocity = Vector3.zero;
				DirectionIndicator.transform.forward = playerDistance.normalized;
				currentChargeChargeUp = ChargeChargeUp;
				
				navigation.speed = 50.0f;
				navigation.acceleration = 50.0f;
				navigation.autoBraking = false;

				return;
			}

			navigation.SetDestination (target.transform.position);
		}

		else if (isCharging == true) 
		{
			if(hasCollided == false)
			{
				navigation.SetDestination(Carrot.transform.position);
				Debug.Log(DirectionIndicator.transform.forward);
			}

			else if (hasCollided == true)
			{

				collisionTimer -= Time.deltaTime;
				if(collisionTimer <= 0.0f)
				{
					isCharging = false;
					hasCollided = false;
					currentChargeCooldown = ChargeCooldown;
					navigation.Resume();
				}
			}
		}
		CheckForAttack ();
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
			underMelee = true;
			attackTicker = 0;
			MeleeAttack.SetActive(true);
			currentAttackTimer = AttackTimer;
		}
	}

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
				tempPos.y = 2.21f;

				spriteImage.transform.position = tempPos;
			}
			else
			{
				Vector3 tempPos = this.gameObject.transform.position;
				tempPos.y = 2.01f;
				tempPos.z -= 0.5f;
				spriteImage.transform.position = tempPos;
			}
			if (dotProd >= 0.75f)
			{
				if (underMelee)
				{
					Animate.Play ("Jugs_Slash_Up");
				}
				else
				{
					Animate.Play ("Jugs_Move_Up");
				}
			}
			else if (dotProd <= -0.75f)
			{
				if (underMelee)
				{
					Animate.Play ("Jugs_Slash_Down");
				}
				else
				{
					Animate.Play ("Jugs_Move_Down");
				}
			}
			else if (dotProd > -0.25f && dotProd <= 0.25f)
			{
				if (crossProd.y < 0.0f)
				{
					if (underMelee)
					{
						Animate.Play ("Jugs_Slash_Left");
					}
					else
					{
						Animate.Play ("Jugs_Move_Left");
					}
				}
				else
				{
					if (underMelee)
					{
						Animate.Play ("Jugs_Slash_Right");
					}
					else
					{
						Animate.Play ("Jugs_Move_Right");
					}
				}
			}
			else if (dotProd > 0.25f && dotProd < 0.75f)
			{
				if (crossProd.y < 0.0f)
				{
					if (underMelee)
					{
						Animate.Play ("Jugs_Slash_UpLeft");
					}
					else
					{
						Animate.Play ("Jugs_Move_UpLeft");
					}
				}
				else
				{
					if (underMelee)
					{
						Animate.Play ("Jugs_Slash_UpRight");
					}
					else
					{
						Animate.Play ("Jugs_Move_UpRight");
					}
				}
			}
			else
			{
				if (crossProd.y < 0.0f)
				{
					if (underMelee)
					{
						Animate.Play ("Jugs_Slash_DownLeft");
					}
					else
					{
						Animate.Play ("Jugs_Move_DownLeft");
					}
				}
				else
				{
					if (underMelee)
					{
						Animate.Play ("Jugs_Slash_DownRight");
					}
					else
					{
						Animate.Play ("Jugs_Move_DownRight");
					}
				}
			}
		}
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
