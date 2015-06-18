using UnityEngine;
using System.Collections;

public class Ragnarok_Phase_2_Controller : MonoBehaviour 
{
	//Helper Variables
	public GameObject player;
	public GameObject sprite;
	private int behaviorState; //0-normal, 1-second state
	private Vector3 destination;
	private NavMeshAgent navigation;
	private Living_Obj LVObj;
	private Transform playerShadow;
	
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

	// Phase 2 variables
	bool healthAt80 = false;
	bool healthAt60 = false;
	bool healthAt40 = false;
	bool healthAt20 = false;
	public GameObject WindBreath;
	public GameObject EarthBreath;
	public GameObject ElectricBreath;
	public GameObject WaterBreath;
	public GameObject WindMeteor;
	public GameObject EarthMeteor;
	public GameObject ElectricMeteor;
	public GameObject WaterMeteor;
	public float StunDuration = 0.5f;
	public float WetDuration = 10.0f;
	public float MaxElementChangeTimer = 10.0f;
	float currentMaxTimer;
	float currentElementTimer;
	private GameObject WetDebuff = null;
	private GameObject StunnedDebuff = null;
	
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
		
		if (Animate == null)
		{
			Animate = transform.GetComponentInChildren<Animator> ();
		}
		if (DirectionIndicator == null)
		{
			DirectionIndicator = transform.FindChild ("Direction Indicator").gameObject;
		}
		BurningDebuff = GameBrain.Instance.GetComponent<DebuffMasterList>().burning;
		WetDebuff = GameBrain.Instance.GetComponent<DebuffMasterList>().wet;
		StunnedDebuff = GameBrain.Instance.GetComponent<DebuffMasterList>().stunned;
		
		//		player = GameBrain.Instance.Player;
		navigation = GetComponent<NavMeshAgent>();
		navigation.updateRotation = false;
		meleeCollider.enabled = false;
		flameBreathCollider.enabled = false;
		LVObj = (Living_Obj)this.gameObject.GetComponent("Living_Obj");
		playerShadow = player.transform;
		playerShadow.position = player.transform.position;
		currentMaxTimer = MaxElementChangeTimer;
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
			CheckHealth ();
			ElementCheck();
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
		currentElementTimer -= Time.deltaTime;
		slashTicker -= Time.deltaTime;
		if (slashTicker <= 0)
		{
			slashing = false;
		}
	}

	private void CheckHealth ()
	{
		if (LVObj.CurrHealth < LVObj.MaxHealth * 0.8f && healthAt80 == false) 
		{
			healthAt80 = true;
			currentMaxTimer = MaxElementChangeTimer * 0.8f;
		}

		if (LVObj.CurrHealth < LVObj.MaxHealth * 0.6f && healthAt60 == false) 
		{
			healthAt60 = true;
			currentMaxTimer = MaxElementChangeTimer * 0.6f;
		}
		if (LVObj.CurrHealth < LVObj.MaxHealth * 0.4f && healthAt40 == false) 
		{
			healthAt40 = true; 
			currentMaxTimer = MaxElementChangeTimer * 0.4f;
		}
		if (LVObj.CurrHealth < LVObj.MaxHealth * 0.2f && healthAt20 == false) 
		{
			healthAt20 = true;
			currentMaxTimer = MaxElementChangeTimer * 0.2f;
		}
	}

	void ElementCheck()
	{
		if(currentElementTimer <= 0.0f)
		{
			ElementalBreathStop();
			Element tempElement = LVObj.ElementType;
			currentElementTimer = currentMaxTimer;
			int elementCount = 0;
			while(tempElement == LVObj.ElementType)
			{
				tempElement = (Element)Random.Range((int)Element.Fire,(int)Element.Water + 1);
				elementCount++;
				if(elementCount == 100)
				{
					Debug.Log("While loop set up incorrectly");
					elementCount = 0;
					break;
				}
			}
			LVObj.ElementType = tempElement;
			currentElementTimer = currentMaxTimer;
		}
	}

	void ElementalBreathCast()
	{
		switch (LVObj.ElementType) 
		{
		case Element.Fire:
		{
			flameBreathDamge = ((float)((int)flameBreathDamge) + 0.1f);
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
			break;
		}
		case Element.Wind:
		{
			flameBreathDamge = ((float)((int)flameBreathDamge) + 0.2f);
			WindBreath.SetActive(true);
			break;
		}
		case Element.Earth:
		{
			flameBreathDamge = ((float)((int)flameBreathDamge) + 0.3f);
			EarthBreath.SetActive(true);
			break;
		}
		case Element.Lightning:
		{
			flameBreathDamge = ((float)((int)flameBreathDamge) + 0.4f);
			ElectricBreath.SetActive(true);
			//float roll = UnityEngine.Random.Range(0.0f, 1.0f);
			//if (roll <= IgniteChance)
			//{
			if (player.transform.FindChild ("Wet(Clone)"))
			{
				GameObject debuff = Instantiate(StunnedDebuff);
				debuff.transform.parent = player.transform;
				debuff.transform.localPosition = new Vector3 (0, -player.transform.position.y, 0);
				debuff.GetComponent<Stunned_Controller>().Duration = StunDuration;
			}
			//}
			break;
		}
		case Element.Water:
		{
			flameBreathDamge = ((float)((int)flameBreathDamge) + 0.5f);
			WaterBreath.SetActive(true);
			//float roll = UnityEngine.Random.Range(0.0f, 1.0f);
			//if (roll <= IgniteChance)
			//{
				GameObject debuff = Instantiate(WetDebuff);
				debuff.transform.parent = player.transform;
				debuff.transform.localPosition = Vector3.zero;
				debuff.GetComponent<Wet_Controller>().Duration = WetDuration;
			//}
			break;
		}
		default:
		{
			Debug.Log("Improper Element Swapping");
			break;
		}
		}
	}

	void ElementalBreathStop()
	{
		switch (LVObj.ElementType) 
		{
		case Element.Fire:
		{
			Firbreath.SetActive(false);
			break;
		}
		case Element.Wind:
		{
			WindBreath.SetActive(false);
			break;
		}
		case Element.Earth:
		{
			EarthBreath.SetActive(false);
			break;
		}
		case Element.Lightning:
		{
			ElectricBreath.SetActive(false);
			break;
		}
		case Element.Water:
		{
			WaterBreath.SetActive(false);
			break;
		}
		default:
		{
			Debug.Log("Improper Element Swapping");
			break;
		}
		}
	}

	void ElementalAOE()
	{
		switch (LVObj.ElementType) 
		{
		case Element.Fire:
		{
			Instantiate(meteor);
			break;
		}
		case Element.Wind:
		{
			Instantiate(WindMeteor,player.transform.position,WindMeteor.transform.rotation);
			break;
		}
		case Element.Earth:
		{
			Instantiate(EarthMeteor,player.transform.position,EarthMeteor.transform.rotation);
			break;
		}
		case Element.Lightning:
		{
			Instantiate(ElectricMeteor,player.transform.position,ElectricMeteor.transform.rotation);
			break;
		}
		case Element.Water:
		{
			Vector3 correctPosition = player.transform.position;
			correctPosition.y = 25;
			Instantiate(WaterMeteor,correctPosition,WaterMeteor.transform.rotation);
			break;
		}
		default:
		{
			Debug.Log("Improper Element Swapping");
			break;
		}
		}
	}

	private void Over50HP()
	{
		Vector3 distanceToPlayer = player.transform.position - this.gameObject.transform.position;
		if (wayPointTicker <= 0)
		{
			//Firbreath.SetActive(false);
			ElementalBreathStop();
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
				//Firbreath.SetActive(false);
				ElementalBreathStop();
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
				//Firbreath.SetActive(true);
				ElementalBreathCast();

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
				ElementalAOE();
			}
		}
	}
	
	private void Under50HP()
	{
		Vector3 distanceToPlayer = player.transform.position - this.gameObject.transform.position;
		if (wayPointTicker <= 0)
		{
			Vector3 tempPos = this.gameObject.transform.position;
			//Firbreath.SetActive(false);
			ElementalBreathStop();
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
				sprite.transform.position = tempPos;
				if (jumpHieght >= 30)
				{
					jumpHieght = 30;
					tempPos.y = jumpHieght;
					sprite.transform.position = tempPos;
					navigation.Warp(playerShadow.position);
					goingUp = false;
				}
			}
			else
			{
				jumpHieght -= Time.deltaTime * 15;
				tempPos.y = jumpHieght;
				sprite.transform.position = tempPos;
				if (jumpHieght <= 0)
				{
					jumpHieght = 0;
					tempPos.y = jumpHieght;
					sprite.transform.position = tempPos;
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
				//Firbreath.SetActive(false);
				ElementalBreathStop();
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
				//Firbreath.SetActive(true);
				ElementalBreathCast();
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
			ElementalAOE();
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

				if(LVObj.ElementType == Element.Lightning && col.transform.FindChild ("Wet(Clone)"))
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
