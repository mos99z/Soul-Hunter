﻿using UnityEngine;
using System.Collections;

public class Boss_Room_Controller : MonoBehaviour {

	public GameObject[] numBosses;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") 
		{
			for (int i = 0; i < numBosses.Length; i++)
			{
				numBosses[i].SetActive(true);
			}
			Destroy (gameObject);
		}

	}
}