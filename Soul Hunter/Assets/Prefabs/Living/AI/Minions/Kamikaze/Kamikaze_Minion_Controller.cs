using UnityEngine;
using System.Collections;

public class Kamikaze_Minion_Controller : MonoBehaviour {

	NavMeshAgent navigation;
	Vector3 destination;
	public GameObject target;
	bool isCountingDown = false;

	public float CountdownTimer = 1.5f;
	public float KamikazeDistance = 2.0f;
	public float ExplosionDamage = 100.0f;
	public float ExplosionRange = 3.0f;

	// Use this for initialization
	void Start () 
	{
		navigation = GetComponent<NavMeshAgent>();
		target = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (target == null) {
		}

		else 
		{
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

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Trap")
			Explode ();
	}

	void PlayerDead()
	{
		target = null;
	}
}
