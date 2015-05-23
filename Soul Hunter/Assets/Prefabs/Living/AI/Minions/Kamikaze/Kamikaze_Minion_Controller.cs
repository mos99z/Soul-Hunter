using UnityEngine;
using System.Collections;

public class Kamikaze_Minion_Controller : MonoBehaviour {

	NavMeshAgent navigation;
	Vector3 destination;
	GameObject target;

	public float KamikazeDistance = 1.0f;

	// Use this for initialization
	void Start () 
	{
		navigation = GetComponent<NavMeshAgent>();
		target = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update ()
	{
		destination = target.transform.position;
		navigation.SetDestination (destination);

		if (navigation.remainingDistance <= KamikazeDistance) 
		{
			gameObject.GetComponent<Kamikaze_Attack_Controller>().enabled = true;
		}
	}
}
