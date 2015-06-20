using UnityEngine;
using System.Collections;

public class Kamikaze_Minion_Controller : MonoBehaviour {

	NavMeshAgent navigation;
	Vector3 destination;
	public GameObject target;
	bool isCountingDown = false;

	public GameObject player;
	public float CountdownTimer = 1.5f;
	public float KamikazeDistance = 3.0f;
	public float ExplosionDamage = 100.0f;
	public float ExplosionRange = 6.0f;
	public bool isFrozen = false;	// used for frozen debuff

	public GameObject DirectionIndicator = null;

	// Appearance
	public GameObject bombElement;
	public GameObject overCharge;
	public GameObject boutToBlow;
	public GameObject boom;
	private Living_Obj LVObj;

	// Use this for initialization
	void Start () 
	{
		Fog_Event_Manager.PlayerEntered += LosePlayer;
		Fog_Event_Manager.PlayerLeft += FindPlayer;
		player = GameBrain.Instance.Player;
		if (DirectionIndicator == null)
			DirectionIndicator = transform.FindChild ("Direction Indicator").gameObject;
		navigation = GetComponent<NavMeshAgent>();
		target = player;
		navigation.updateRotation = false;

		LVObj = (Living_Obj)this.gameObject.GetComponent<Living_Obj>();
		Element elType = LVObj.ElementType;
		switch (elType)
		{
		case Element.Fire:
		{
			bombElement.GetComponent<ParticleSystem>().startColor = new Color32(255, 0, 0, 255);
			overCharge.GetComponent<ParticleSystem>().startColor = new Color32(239, 0, 0, 255);
			boutToBlow.GetComponent<ParticleSystem>().startColor = new Color32(255, 0, 0, 255);
			boom.GetComponent<ParticleSystem>().startColor = new Color32(255, 16, 16, 255);
			break;
		}
		case Element.Wind:
		{
			bombElement.GetComponent<ParticleSystem>().startColor = new Color32(128, 128, 128, 255);
			overCharge.GetComponent<ParticleSystem>().startColor = new Color32(112, 112, 112, 255);
			boutToBlow.GetComponent<ParticleSystem>().startColor = new Color32(128, 128, 128, 255);
			boom.GetComponent<ParticleSystem>().startColor = new Color32(144, 144, 144, 255);
			break;
		}
		case Element.Earth:
		{
			bombElement.GetComponent<ParticleSystem>().startColor = new Color32(128, 64, 0, 255);
			overCharge.GetComponent<ParticleSystem>().startColor = new Color32(112, 48, 0, 255);
			boutToBlow.GetComponent<ParticleSystem>().startColor = new Color32(128, 64, 0, 255);
			boom.GetComponent<ParticleSystem>().startColor = new Color32(144, 80, 16, 255);
			break;
		}
		case Element.Lightning:
		{
			bombElement.GetComponent<ParticleSystem>().startColor = new Color32(255, 255, 0, 255);
			overCharge.GetComponent<ParticleSystem>().startColor = new Color32(239, 239, 0, 255);
			boutToBlow.GetComponent<ParticleSystem>().startColor = new Color32(255, 255, 0, 255);
			boom.GetComponent<ParticleSystem>().startColor = new Color32(255, 255, 16, 255);
			break;
		}
		case Element.Water:
		{
			bombElement.GetComponent<ParticleSystem>().startColor = new Color32(32, 32, 255, 255);
			overCharge.GetComponent<ParticleSystem>().startColor = new Color32(16, 16, 255, 255);
			boutToBlow.GetComponent<ParticleSystem>().startColor = new Color32(32, 32, 255, 255);
			boom.GetComponent<ParticleSystem>().startColor = new Color32(48, 48, 255, 255);
			break;
		}
		default:
		{
			bombElement.GetComponent<ParticleSystem>().startColor = new Color32(32, 32, 32, 255);
			overCharge.GetComponent<ParticleSystem>().startColor = new Color32(16, 16, 16, 255);
			boutToBlow.GetComponent<ParticleSystem>().startColor = new Color32(32, 32, 32, 255);
			boom.GetComponent<ParticleSystem>().startColor = new Color32(48, 48, 48, 255);
			break;
		}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isFrozen)
			return;

		TurnTowardsPlayer ();

		if (isCountingDown == false) 
		{
			destination = target.transform.position;
			navigation.SetDestination (destination);
			float playerDistance = (target.transform.position - gameObject.transform.position).magnitude;
			if (playerDistance <= KamikazeDistance) 
			{
				isCountingDown = true;
				boutToBlow.SetActive(true);
				navigation.Stop();
			}
		}
		
		if (isCountingDown == true) 
		{
			CountdownTimer -= Time.deltaTime;
			if (CountdownTimer <= 0.1f)
			{
				boom.SetActive(true);
			}
			if(CountdownTimer <= 0)
			{
				Explode();
			}
		}


	}

	void Explode()
	{
		GetComponent<Living_Obj>().SoulValue = SoulType.None;
		float playerDistance = (target.transform.position - gameObject.transform.position).magnitude;
		if (playerDistance < ExplosionRange) 
		{
			target.SendMessage("TakeDamage", ExplosionDamage);
		}
		Destroy (gameObject);
	}

	// This function makes the enemy always face towards the player while rotating around him
	void TurnTowardsPlayer()
	{
		//		DirectionIndicator.transform.LookAt(target.transform, new Vector3(0,1,0));
		Vector3 movementDirection = navigation.velocity.normalized;
		if (movementDirection.magnitude >= 1.0f) {
			DirectionIndicator.transform.forward = navigation.velocity.normalized;
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Trap")
			Explode ();
	}

	void LosePlayer()
	{
		GameObject fakePlayer = player;
		fakePlayer.transform.position += Random.insideUnitSphere * 0.5f;
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
