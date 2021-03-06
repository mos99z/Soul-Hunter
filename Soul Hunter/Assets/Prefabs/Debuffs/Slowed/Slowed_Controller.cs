﻿using UnityEngine;
using System.Collections;

public class Slowed_Controller : MonoBehaviour 
{
	public float duration = 5.0f;			// how long in seconds for the debuff to last
	public float slowSpeedModifier = 0.5f;	// fraction to reduce objects speed by
	public bool fromConcrete = false;		// used to cast stun after slow wears off from concrete spell

	float origSpeed;						// remember objects original speed
	bool check;								// check if a clone exists already

	void Start () 
	{
		check = true;
	}
	
	void Update ()
	{
		if (check && transform.parent != null)
		{
			if (transform.parent.GetComponent<Living_Obj>().entType == Living_Obj.EntityType.Boss)
			{
				slowSpeedModifier = 0.75f;
			}
			int children = transform.parent.childCount;
			for (int child = 0; child < children; child++)
			{
				if (transform.parent.GetChild(child).name == "Slowed(Clone)" && transform.parent.GetChild(child) != transform)
				{
					transform.parent.GetChild(child).GetComponent<Slowed_Controller>().duration = duration;
					Destroy(gameObject);
					return;
				}
			}

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
			check = false;
		}
		
		duration -= Time.deltaTime;
		if (duration <= 0.0f)
		{
			if (transform.parent.tag == "Player")
				transform.parent.GetComponent<Player_Movement_Controller>().Speed = origSpeed;
			else if (transform.parent.tag == "Enemy")
				transform.parent.GetComponent<NavMeshAgent>().speed = origSpeed;

			if (fromConcrete)
			{
				GameObject stun = Instantiate(GameBrain.Instance.GetComponent<DebuffMasterList>().stunned);
				stun.transform.parent = transform.parent;
				stun.transform.localPosition = Vector3.zero;
				stun.GetComponent<Stunned_Controller>().Duration = 4.0f;
			}

			Destroy(gameObject);
		}
	}

	void OnDestroy()
	{
		if (transform.parent.tag == "Player")
			transform.parent.GetComponent<Player_Movement_Controller>().Speed = origSpeed;
	}
}
