using UnityEngine;
using System.Collections;

public class Kamikaze_Minion_Controller : MonoBehaviour {

	NavMeshAgent navigation;
	Vector3 destination;
	public GameObject target;
	bool isCountingDown = false;

	public GameObject player;
	public float CountdownTimer = 1.5f;
	public float KamikazeDistance = 2.0f;
	public float ExplosionDamage = 100.0f;
	public float ExplosionRange = 3.0f;
	public bool isFrozen = false;	// used for frozen debuff

	public GameObject DirectionIndicator = null;
	public Animator Animate = null;

	// Use this for initialization
	void Start () 
	{
		Fog_Event_Manager.PlayerEntered += LosePlayer;
		Fog_Event_Manager.PlayerLeft += FindPlayer;
		player = GameBrain.Instance.Player;
		if (Animate == null)
			Animate = transform.GetComponentInChildren<Animator> ();
		if (DirectionIndicator == null)
			DirectionIndicator = transform.FindChild ("Direction Indicator").gameObject;
		navigation = GetComponent<NavMeshAgent>();
		target = player;
		navigation.updateRotation = false;
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
				navigation.Stop();
			}
		}
		
		if (isCountingDown == true) 
		{
			CountdownTimer -= Time.deltaTime;
			
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
			float dotProd = Vector3.Dot (new Vector3 (0, 0, 1), movementDirection);
			Vector3 crossProd = Vector3.Cross (new Vector3 (0, 0, 1), movementDirection);
			if (dotProd >= 0.75f)
			{
				Animate.Play ("Kamikaze_Up");
			}
			else if (dotProd <= -0.75f)
			{
				Animate.Play ("Kamikaze_Down");
			}
			else if (dotProd > -0.25f && dotProd <= 0.25f)
			{
				if (crossProd.y < 0.0f)
					Animate.Play ("Kamikaze_Left");
				else
					Animate.Play ("Kamikaze_Right");
			}
			else if (dotProd > 0.25f && dotProd < 0.75f)
			{
				if (crossProd.y < 0.0f)
					Animate.Play ("Kamikaze_UpLeft");
				else
					Animate.Play ("Kamikaze_UpRight");
			}
			else
			{
				if (crossProd.y < 0.0f)
					Animate.Play ("Kamikaze_DownLeft");
				else
					Animate.Play ("Kamikaze_DownRight");
			}
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
