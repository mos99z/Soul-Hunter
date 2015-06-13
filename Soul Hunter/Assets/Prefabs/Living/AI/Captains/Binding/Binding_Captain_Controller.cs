using UnityEngine;
using System.Collections;

public class Binding_Captain_Controller : MonoBehaviour
{
	//Helper Variables
	public GameObject player;
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
	public GameObject Claw;
	public GameObject stunAn;
	public GameObject pullAn;

	//Debuffs
	public float StunDuration = 0.5f;

	// Use this for initialization
	void Start ()
	{
		player = GameBrain.Instance.Player;
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
		DirectionIndicator.transform.LookAt(player.transform.position, new Vector3(0,1,0));
		Vector3 movementDirection = DirectionIndicator.transform.forward;
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
}
