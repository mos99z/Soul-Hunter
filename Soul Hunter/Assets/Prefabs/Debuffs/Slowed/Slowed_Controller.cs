﻿using UnityEngine;
using System.Collections;

public class Slowed_Controller : MonoBehaviour 
{
	public float duration = 5.0f;			// how long in seconds for the debuff to last
	public float slowSpeedModifier = 0.5f;	// fraction to reduce objects speed by

	float timer;							// keep track of reference to duration
	float origSpeed;						// remember objects original speed
	bool check;								// check if a clone exists already

	void Start () 
	{
		timer = 0.0f;
		check = true;
	}
	
	void Update ()
	{
		if (check && transform.parent != null)
		{
			if (transform.parent.tag == "Player")
			{
				origSpeed = transform.parent.GetComponent<Player_Movement_Controller>().Speed;
				transform.parent.GetComponent<Player_Movement_Controller>().Speed *= slowSpeedModifier;
			}
			else if (transform.parent.tag == "Enemy")
			{
				origSpeed = transform.parent.GetComponent<NavMeshAgent>().speed;
				transform.parent.GetComponent<NavMeshAgent>().speed *= slowSpeedModifier;
			}
			int children = transform.parent.childCount;
			for (int child = 0; child < children; child++)
			{
				if (transform.parent.GetChild(child).name == "Slowed(Clone)" && transform.parent.GetChild(child) != transform)
				{
					Destroy(transform.parent.GetChild(child).gameObject);
				}
			}
			check = false;
		}
		
		timer += Time.deltaTime;
		if (timer >= duration)
		{
			if (transform.parent.tag == "Player")
				transform.parent.GetComponent<Player_Movement_Controller>().Speed = origSpeed;
			else if (transform.parent.tag == "Enemy")
				transform.parent.GetComponent<NavMeshAgent>().speed = origSpeed;
			Destroy(gameObject);
		}
	}
}
