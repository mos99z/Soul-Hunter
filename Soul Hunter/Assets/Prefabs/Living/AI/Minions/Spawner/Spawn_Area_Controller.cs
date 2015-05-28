using UnityEngine;
using System.Collections;

public class Spawn_Area_Controller : MonoBehaviour {

	public GameObject[] Spawners = null;
	public bool AreaContainsCaptain = false;
	public GameObject Captain;
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (AreaContainsCaptain) 
		{
			if (Captain == null)
			{
				Destroy(gameObject);
			}
		}

		bool stillNeeded= false;
		for (int i = 0; i < Spawners.Length; i++) {
			if(Spawners[i] != null)
			{
				stillNeeded = true;
				break;
			}
		}

		if (stillNeeded == false)
			Destroy (gameObject);
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") 
		{
			for (int i = 0; i < Spawners.Length; i++)
			{
				Spawners[i].SetActive(true);
			}
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.tag == "ddPlayer") 
		{
			for (int i = 0; i < Spawners.Length; i++)
			{
				if(Spawners[i] != null)
					Spawners[i].SetActive(false);
			}
		}
	}
}
