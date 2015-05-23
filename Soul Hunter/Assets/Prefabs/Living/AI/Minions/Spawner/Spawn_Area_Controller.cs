using UnityEngine;
using System.Collections;

public class Spawn_Area_Controller : MonoBehaviour {

	public GameObject[] Spawners = null;
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
			for (int i = 0; i < Spawners.Length; i++)
			{
				Spawners[i].SetActive(true);
			}
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player") 
		{
			for (int i = 0; i < Spawners.Length; i++)
			{
				Spawners[i].SetActive(false);
			}
		}
	}
}
