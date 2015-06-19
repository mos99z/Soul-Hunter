using UnityEngine;
using System.Collections;

public class Blinded_Controller : MonoBehaviour {

	bool checkonce = true;
	MonoBehaviour[] Scripts;
	NavMeshAgent navigation;
	Vector3 destination;
	public float Duration = 0.0f;
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (checkonce)
		{
			if (transform.parent.GetComponent<Living_Obj> ().entType == Living_Obj.EntityType.Boss)
			{
				Destroy (gameObject);
				return;
			}

			Scripts = transform.parent.GetComponents<MonoBehaviour>();

			for (int i = 0; i < Scripts.Length; i++)
			{
				if (i != 0)
					Scripts[i].enabled = false;
			}

			navigation = transform.parent.GetComponent<NavMeshAgent>();
			destination = Random.insideUnitSphere * 7;
			destination.y = 0;
			destination += transform.position;

			checkonce = false;
		}

		transform.parent.GetComponent<NavMeshAgent>().SetDestination (destination);
		
		
		if (navigation.remainingDistance < 0.1f) 
		{
			destination = Random.insideUnitSphere * 7;
			destination.y = 0;
			destination += transform.position;
		}

		Duration -= Time.deltaTime;
		if (Duration <= 0.0f)
			Destroy (gameObject);
	}

	void OnDestroy()
	{
		if (transform.parent != null && Scripts != null)
			for (int i = 0; i < Scripts.Length; i++)
			{
				if (Scripts[i] != null)
					Scripts[i].enabled = true;
			}
	}
}
