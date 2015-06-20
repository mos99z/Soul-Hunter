using UnityEngine;
using System.Collections;

public class Ragnorak_Controller : MonoBehaviour
{
	//Helper Variables
	public GameObject player;
	public GameObject sprite;
	private int behaviorState; //0-normal, 1-second state
	private Vector3 destination;
	private NavMeshAgent navigation;
	private Living_Obj LVObj;
	private Transform playerShadow;

	//public GameObject boundingWalls;		// this will lock the player in a room

	//Behavior vars
	public float wayPointMinTicker = 3;
	public float wayPointMaxTicker = 3;
	public float wayPointTicker = 3;
	public int currWayPoint = 0;
	private float animationTicker = 1;
	public float meleeMinTicker = 1;
	public float meleeMaxTicker = 1;
	private float meleeTicker;
	public float meleeDamge;
	public float meleeRange = 5;
	public SphereCollider meleeCollider;
	public float flameBreathMinTicker = 1;
	public float flameBreathMaxTicker = 1;
	private float flameBreathTicker;
	public float flameBreathLength = 1;
	public float FBLengthTicker = 0;
	public float flameBreathDamge;
	public float flameBreathRange = 5;
	public CapsuleCollider flameBreathCollider;
	public float meteorMinTicker = 1;
	public float meteorMaxTicker = 1;
	private float meteorTicker;
	public float meteorDamge;
	public float meteorRange = 5;
	private float jumpHieght;
	private bool goingUp;
	private bool doOnce;
	public SphereCollider SockWaveCollider;
	public float shockDamge = 1;

	//Needed components
	private GameObject DirectionIndicator = null;
	private Animator Animate = null;
	public GameObject Firbreath;
	public GameObject AOEJump;
	public GameObject Claw;
	public GameObject WarpDirection;
	public GameObject Reapear;
	public GameObject meteor;
	public GameObject meteor2;
	//waypoints
	public Transform[] waypoints = new Transform[5];
	public SphereCollider bossCollider;

	//Debuffs
	public float ImpactDamage = 10.1f;
	public float IgniteChance = 1.0f;
	public float BurningDOTDuration = 10.0f;
	public float BurningDOTTickCycle = 0.5f;
	public float BurningDOTTickDamage = 5.1f;
	private GameObject BurningDebuff = null;

	//sprite anmation
	private bool slashing;
	private float slashTicker;

	// Use this for initialization
	void Start ()
	{
		player = GameBrain.Instance.Player;

		behaviorState = 0;
		jumpHieght = 0;
		goingUp = true;
		doOnce = false;
		slashing = false;

		if (Animate == null)
		{
			Animate = transform.GetComponentInChildren<Animator> ();
		}
		if (DirectionIndicator == null)
		{
			DirectionIndicator = transform.FindChild ("Direction Indicator").gameObject;
		}
		BurningDebuff = GameBrain.Instance.GetComponent<DebuffMasterList>().burning;

//		player = GameBrain.Instance.Player;
		navigation = GetComponent<NavMeshAgent>();
		navigation.updateRotation = false;
		meleeCollider.enabled = false;
		flameBreathCollider.enabled = false;
		//boundingWalls.SendMessage("ActivateWalls");
		LVObj = (Living_Obj)this.gameObject.GetComponent("Living_Obj");
		playerShadow = player.transform;
		playerShadow.position = player.transform.position;
	}

	void OnDestroy()
	{
		//boundingWalls.SendMessage("DestroyWalls");
		GameBrain.Instance.SendMessage("ChangeMusic",GameBrain.Instance.GameplayMusic);
	}
	// Update is called once per frame
	void Update ()
	{
		if (LVObj.CurrHealth <= LVObj.MaxHealth * 0.5)
		{
			behaviorState = 1;
			animationTicker = 1;
		}
		if (player != null)
		{
			if (animationTicker == 1)
			{
				TurnTowardsPlayer();
			}
			else
			{
				DirectionIndicator.transform.LookAt(waypoints[currWayPoint].position, new Vector3(0,0,0));
			}
			UpdateTickers();
			switch (behaviorState)
			{
			case 0:
			{
				Over50HP();
				break;
			}
			case 1:
			{
				Under50HP();
				break;
			}
			default:
				break;
			}
		}
	}

	private void UpdateTickers()
	{
		wayPointTicker -= Time.deltaTime;
		meleeTicker -= Time.deltaTime;
		flameBreathTicker -= Time.deltaTime;
		meteorTicker -= Time.deltaTime;
		SockWaveCollider.enabled = false;
		slashTicker -= Time.deltaTime;
		if (slashTicker <= 0)
		{
			slashing = false;
		}
	}

	private void Over50HP()
	{
		Vector3 distanceToPlayer = player.transform.position - this.gameObject.transform.position;
		if (wayPointTicker <= 0)
		{
			Firbreath.SetActive(false);
			Reapear.SetActive(false);
			if (animationTicker == 1.0f)
			{
				navigation.SetDestination(waypoints[currWayPoint].position);
				WarpDirection.SetActive(true);
			}
			animationTicker -= Time.deltaTime;
			if (animationTicker <= 0)
			{
				WarpDirection.SetActive(false);
				navigation.Warp(waypoints[currWayPoint].position);
				Reapear.SetActive(true);
				wayPointTicker = Random.Range(wayPointMinTicker, wayPointMaxTicker);
				int tempWayPoint;
				while (true)
				{
					tempWayPoint = Random.Range(0, 4);
					if (tempWayPoint == currWayPoint)
					{
						continue;
					}
					currWayPoint = tempWayPoint;
					break;
				}
				animationTicker = 1.0f;
			}
		}
		else if (distanceToPlayer.magnitude < meleeRange)
		{
			if (meleeTicker <= 0)
			{
				Firbreath.SetActive(false);
				Claw.SetActive(false);
				Claw.SetActive(true);
				meleeTicker = Random.Range(meleeMinTicker, meleeMaxTicker);
				meleeCollider.enabled = true;
				slashing = true;
				slashTicker = 1.75f;
			}
		}
		else if (distanceToPlayer.magnitude < flameBreathRange)
		{
			if (flameBreathTicker <= 0)
			{
				Firbreath.SetActive(true);
				float roll = UnityEngine.Random.Range(0.0f, 1.0f);
				if (roll <= IgniteChance)
				{
					GameObject debuff = Instantiate(BurningDebuff);
					debuff.transform.parent = player.transform;
					debuff.transform.localPosition = Vector3.zero;
					debuff.GetComponent<Burning_Controller>().Damage = BurningDOTTickDamage;
					debuff.GetComponent<Burning_Controller>().Duration = BurningDOTDuration;
					debuff.GetComponent<Burning_Controller>().TickCycle = BurningDOTTickCycle;
				}
				FBLengthTicker += Time.deltaTime;
				flameBreathTicker = 0.5f;
				if (FBLengthTicker >= flameBreathLength)
				{
					FBLengthTicker = 0;
					Firbreath.SetActive(false);
					flameBreathTicker = Random.Range(flameBreathMinTicker, flameBreathMaxTicker);
				}
				flameBreathCollider.enabled = true;
			}
		}
		else
		{
			if (meteorTicker <= 0)
			{
				meteorTicker = Random.Range(meteorMinTicker, meteorMaxTicker);
				Instantiate(meteor);
			}
		}
	}

	private void Under50HP()
	{
		Vector3 distanceToPlayer = player.transform.position - this.gameObject.transform.position;
		if (wayPointTicker <= 0)
		{
			Vector3 tempPos = new Vector3(-0.5f, 0, -3);
			Firbreath.SetActive(false);
			if (!doOnce)
			{
				playerShadow.position = player.transform.position;
				bossCollider.enabled = false;
				doOnce = true;
			}
			if (goingUp)
			{
				jumpHieght += Time.deltaTime * 15;
				tempPos.y = jumpHieght;
				sprite.transform.localPosition = tempPos;
				if (jumpHieght >= 30)
				{
					jumpHieght = 30;
					tempPos.y = jumpHieght;
					sprite.transform.localPosition = tempPos;
					navigation.Warp(playerShadow.position);
					goingUp = false;
				}
			}
			else
			{
				jumpHieght -= Time.deltaTime * 15;
				tempPos.y = jumpHieght;
				sprite.transform.localPosition = tempPos;
				if (jumpHieght <= 0)
				{
					jumpHieght = 0;
					tempPos.y = jumpHieght;
					sprite.transform.localPosition = tempPos;
					AOEJump.SetActive(false);
					AOEJump.SetActive(true);
					goingUp = true;
					wayPointTicker = Random.Range(wayPointMinTicker, wayPointMaxTicker);
					doOnce = false;
					bossCollider.enabled = true;
					SockWaveCollider.enabled = true;
				}
			}
		}
		else if (distanceToPlayer.magnitude < meleeRange)
		{
			if (meleeTicker <= 0)
			{
				Firbreath.SetActive(false);
				Claw.SetActive(false);
				Claw.SetActive(true);
				meleeTicker = Random.Range(meleeMinTicker, meleeMaxTicker);
				meleeCollider.enabled = true;
				slashing = true;
				slashTicker = 1.75f;
			}
		}
		else if (distanceToPlayer.magnitude < flameBreathRange)
		{
			if (flameBreathTicker <= 0)
			{
				Firbreath.SetActive(true);
				float roll = UnityEngine.Random.Range(0.0f, 1.0f);
				if (roll <= IgniteChance)
				{
					GameObject debuff = Instantiate(BurningDebuff);
					debuff.transform.parent = player.transform;
					debuff.transform.localPosition = Vector3.zero;
					debuff.GetComponent<Burning_Controller>().Damage = BurningDOTTickDamage;
					debuff.GetComponent<Burning_Controller>().Duration = BurningDOTDuration;
					debuff.GetComponent<Burning_Controller>().TickCycle = BurningDOTTickCycle;
				}
				FBLengthTicker += Time.deltaTime;
				flameBreathTicker = 0.5f;
				if (FBLengthTicker >= flameBreathLength)
				{
					FBLengthTicker = 0;
					Firbreath.SetActive(false);
					flameBreathTicker = Random.Range(flameBreathMinTicker, flameBreathMaxTicker);
				}
				flameBreathCollider.enabled = true;
			}
		}
		if (meteorTicker <= 0)
		{
			meteorTicker = 1;
			Instantiate(meteor2);
		}
	}

	private void TurnTowardsPlayer()
	{
		DirectionIndicator.transform.LookAt(player.transform.position, new Vector3(0,1,0));
		Vector3 movementDirection = DirectionIndicator.transform.forward;
		if (movementDirection.magnitude >= 1.0f)
		{
			float dotProd = Vector3.Dot (new Vector3 (0, 0, 1), movementDirection);
			Vector3 crossProd = Vector3.Cross (new Vector3 (0, 0, 1), movementDirection);
			if (slashing)
			{
				Vector3 tempPos = new Vector3(0, 5, -1);
				sprite.transform.localPosition = tempPos;
			}
			else if (wayPointTicker > 0)
			{
				Vector3 tempPos = new Vector3(-0.5f, 0, -3);
				sprite.transform.localPosition = tempPos;
			}
			if (dotProd >= 0.75f)
			{
				if (!slashing)
				{
					Animate.Play ("Rag_Idle_Up");
				}
				else
				{
					Animate.Play ("Rag_Slash_Up");
				}
			}
			else if (dotProd <= -0.75f)
			{
				if (!slashing)
				{
					Animate.Play ("Rag_Idle_Down");
				}
				else
				{
					Animate.Play ("Rag_Slash_Down");
				}
			}
			else if (dotProd > -0.25f && dotProd <= 0.25f)
			{
				if (!slashing)
				{
					if (crossProd.y < 0.0f)
						Animate.Play ("Rag_Idle_Left");
					else
						Animate.Play ("Rag_Idle_Right");
				}
				else
				{
					if (crossProd.y < 0.0f)
						Animate.Play ("Rag_Slash_Left");
					else
						Animate.Play ("Rag_Slash_Right");
				}
			}
			else if (dotProd > 0.25f && dotProd < 0.75f)
			{
				if (!slashing)
				{
					if (crossProd.y < 0.0f)
						Animate.Play ("Rag_Idle_Up_Left");
					else
						Animate.Play ("Rag_Idle_Up_Right");
				}
				else
				{
					if (crossProd.y < 0.0f)
						Animate.Play ("Rag_Slash_Up_Left");
					else
						Animate.Play ("Rag_Slash_Up_Right");
				}
			}
			else
			{
				if (!slashing)
				{
					if (crossProd.y < 0.0f)
						Animate.Play ("Rag_Idle_Down_Left");
					else
						Animate.Play ("Rag_Idle_Down_Right");
				}
				else
				{
					if (crossProd.y < 0.0f)
						Animate.Play ("Rag_Slash_Down_Left");
					else
						Animate.Play ("Rag_Slash_Down_Right");
				}
			}
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") 
		{
			if (meleeCollider.enabled == true)
			{
				col.SendMessage("TakeDamage", meleeDamge);
				meleeCollider.enabled = false;
			}
			if (flameBreathCollider.enabled == true)
			{
				col.SendMessage("TakeDamage", flameBreathDamge);
				flameBreathCollider.enabled = false;
			}
			if (SockWaveCollider.enabled)
			{
				col.SendMessage("TakeDamage", shockDamge);
				SockWaveCollider.enabled = false;
			}
		}
	}
}
