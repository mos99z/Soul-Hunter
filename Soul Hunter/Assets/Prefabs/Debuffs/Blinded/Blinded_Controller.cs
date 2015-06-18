using UnityEngine;
using System.Collections;

public class Blinded_Controller : MonoBehaviour {

	bool checkonce = true;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (checkonce)
		{
			if (transform.parent.GetComponent<Living_Obj>().entType == Living_Obj.EntityType.Boss)
			{
				Destroy(gameObject);
				return;
			}
			checkonce = false;
		}
	}
}
