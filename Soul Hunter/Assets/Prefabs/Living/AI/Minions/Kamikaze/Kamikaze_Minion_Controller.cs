﻿using UnityEngine;
using System.Collections;

public class Kamikaze_Minion_Controller : MonoBehaviour {

	NavMeshAgent navigation;
	Vector3 destination;
	GameObject target;
	bool isCountingDown = false;

	public float CountdownTimer = 1.5f;
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
		if (isCountingDown == false) 
		{
			destination = target.transform.position;
			navigation.SetDestination (destination);
			
			if (navigation.remainingDistance <= KamikazeDistance) 
			{
				isCountingDown = true;
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
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Trap")
			Explode ();
	}
}
