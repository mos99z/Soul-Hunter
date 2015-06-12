using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Laser_Controller : MonoBehaviour 
{
	void Start()
	{
		Vector3 spawn = Vector3.zero;
		spawn.y += 1.0f;
		transform.position = spawn;
	}
}
