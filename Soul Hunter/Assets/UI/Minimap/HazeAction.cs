using UnityEngine;
using System.Collections;

public class HazeAction : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter(Collider _coll)
	{
		if (_coll.tag == "Player")
		{
			Destroy(gameObject);
		}
	}
}
