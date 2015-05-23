﻿using UnityEngine;
using System.Collections;

public class HazeMaker : MonoBehaviour
{
	public GameObject Haze;

	// Use this for initialization
	void Start ()
	{
		for (int i = -150; i <= 150; i += 7)
		{
			for (int j = -50; j <= 400; j += 7)
			{
				Vector3 newPosition = new Vector3(i, 50, j);
				GameObject newHaze = (GameObject)Instantiate(Haze, newPosition, Quaternion.identity);
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}