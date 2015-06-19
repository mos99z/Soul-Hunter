using UnityEngine;
using System.Collections;

public class Blinded_Controller : MonoBehaviour {

	bool checkonce = true;
	bool blinded = false;
	MonoBehaviour[] Scripts;
	NavMeshAgent navigation;
	Vector3 destination;
	public float Duration = 0.0f;
	// Use this for initialization
	void Start ()
	{
		Scripts = transform.parent.GetComponents<MonoBehaviour>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (checkonce) {
			if (transform.parent.GetComponent<Living_Obj> ().entType == Living_Obj.EntityType.Boss) {
				Destroy (gameObject);
				return;
			}

			//Scripts = transform.parent.GetComponents<MonoBehaviour>();
			for (int i = 0; i < Scripts.Length; i++)
			{
				Scripts[i].enabled = false;
			}

			navigation = transform.parent.GetComponent<NavMeshAgent>();
			navigation.autoBraking = true;
			destination = Random.insideUnitSphere * 7;
			destination.y = 0;
			destination += transform.position;

			checkonce = false;
		}

		Duration -= Time.deltaTime;
		navigation.SetDestination (destination);
		
		
		if (navigation.remainingDistance == 0) 
		{
			destination = Random.insideUnitSphere * 7;
			destination.y = 0;
			destination += transform.position;
		}

		if (Duration <= 0.0f)
			Destroy (gameObject);
	}

	void OnDestroy()
	{
		for (int i = 0; i < Scripts.Length; i++)
		{
			Scripts[i].enabled = true;
		}
	}
}
