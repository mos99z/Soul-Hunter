using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Binding_Captain_Controller : MonoBehaviour
{
	//Helper Variables
	public GameObject target;
	GameObject player;
	private NavMeshAgent navigation;
	private Player_Caster_Controller PCC;
	private Player_Movement_Controller PMC;
	public bool isSurrounded;
	public GameObject boundingWalls;	// for setting walls to lock room

	//behavior vars
	public float meleeMinTicker = 1;
	public float meleeMaxTicker = 1;
	private float meleeTicker = 0;
	public float meleeDamge;
	public float meleeRange = 5;
	public SphereCollider meleeCollider;

	public float abilityMinTicker = 1;
	public float abilityMaxTicker = 1;
	public float abilityTicker = 0;
	public float abilityLength = 5;
	public float aLengthTicker = 0;
	public float abilityRange = 5;
	public bool abilityStart = false;

	//Needed components
	private GameObject DirectionIndicator = null;
	public Animator Animate = null;
	public GameObject Claw;
	public GameObject stunAn;
	public GameObject pullAn;

	//Debuffs
	public float StunDuration = 0.5f;

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
		target = player = GameBrain.Instance.Player;
		navigation = GetComponent<NavMeshAgent>();
		navigation.updateRotation = false;
		isSurrounded = false;

		if (DirectionIndicator == null)
		{
			DirectionIndicator = transform.FindChild ("Direction Indicator").gameObject;
		}
		PCC = (Player_Caster_Controller)player.GetComponent("Player_Caster_Controller");
		PMC = (Player_Movement_Controller)player.GetComponent("Player_Movement_Controller");
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
		TurnTowardsPlayer();
		UpdateTickers();
		DoStuff();
	}

	private void DoStuff()
	{
		Vector3 distanceToPlayer = player.transform.position - this.gameObject.transform.position;
		if (distanceToPlayer.magnitude < meleeRange)
		{
			if (meleeTicker <= 0)
			{
				Claw.SetActive(false);
				Claw.SetActive(true);
				meleeTicker = Random.Range(meleeMinTicker, meleeMaxTicker);
				meleeCollider.enabled = true;
				abilityTicker = Random.Range(abilityMinTicker, abilityMaxTicker);
			}
		}
		if (abilityStart)
		{
			aLengthTicker += Time.deltaTime;
			if (aLengthTicker <= abilityLength)
			{
				if (!isSurrounded)
				{
					Vector3 tempVec = player.transform.position - this.transform.position;
					tempVec = new Vector3(tempVec.x * -300, 0, tempVec.z * -300);
					player.GetComponent<Rigidbody>().AddForce(tempVec);
				}
			}
			else
			{
				stunAn.SetActive(false);
				pullAn.SetActive(false);
				PCC.enabled = true;
				PMC.enabled = true;
				abilityStart = false;
				aLengthTicker = 0;
				abilityTicker = Random.Range(abilityMinTicker, abilityMaxTicker);
			}
		}
		else if (distanceToPlayer.magnitude < abilityRange)
		{
			if (abilityTicker <= 0)
			{
				if (!abilityStart)
				{
					underMelee = true;
					attackTicker = 0;
					navigation.SetDestination(this.transform.position);
					if (isSurrounded)
					{
						PCC.enabled = false;
						PMC.enabled = false;
						stunAn.SetActive(true);
					}
					else
					{
						pullAn.SetActive(true);
					}
					abilityStart = true;
				}
			}
		}
	}

	private void UpdateTickers()
	{
		meleeTicker -= Time.deltaTime;
		abilityTicker -= Time.deltaTime;
	}

	private void TurnTowardsPlayer()
	{
		if (!abilityStart)
		{
			navigation.SetDestination (player.transform.position);
		}

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
			if (dotProd >= 0.75f)
			{
				if (underMelee)
				{
					Animate.Play ("Bind_Slash_Up");
				}
				else
				{
					Animate.Play ("Bind_Move_Up");
				}
			}
			else if (dotProd <= -0.75f)
			{
				if (underMelee)
				{
					Animate.Play ("Bind_Slash_Down");
				}
				else
				{
					Animate.Play ("Bind_Move_Down");
				}
			}
			else if (dotProd > -0.25f && dotProd <= 0.25f)
			{
				if (crossProd.y < 0.0f)
				{
					if (underMelee)
					{
						Animate.Play ("Bind_Slash_Left");
					}
					else
					{
						Animate.Play ("Bind_Move_Left");
					}
				}
				else
				{
					if (underMelee)
					{
						Animate.Play ("Bind_Slash_Right");
					}
					else
					{
						Animate.Play ("Bind_Move_Right");
					}
				}
			}
			else if (dotProd > 0.25f && dotProd < 0.75f)
			{
				if (crossProd.y < 0.0f)
				{
					if (underMelee)
					{
						Animate.Play ("Bind_Slash_UpLeft");
					}
					else
					{
						Animate.Play ("Bind_Move_UpLeft");
					}
				}
				else
				{
					if (underMelee)
					{
						Animate.Play ("Bind_Slash_UpRight");
					}
					else
					{
						Animate.Play ("Bind_Move_UpRight");
					}
				}
			}
			else
			{
				if (crossProd.y < 0.0f)
				{
					if (underMelee)
					{
						Animate.Play ("Bind_Slash_DownLeft");
					}
					else
					{
						Animate.Play ("Bind_Move_DownLeft");
					}
				}
				else
				{
					if (underMelee)
					{
						Animate.Play ("Bind_Slash_DownRight");
					}
					else
					{
						Animate.Play ("Bind_Move_DownRight");
					}
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

	void OnDestroy()
	{
		Fog_Event_Manager.PlayerEntered -= LosePlayer;
		Fog_Event_Manager.PlayerLeft -= FindPlayer;
		GameBrain.Instance.SendMessage("ChangeMusic",GameBrain.Instance.GameplayMusic);
		GameBrain.Instance.FightingCaptain = false;
	}
}
