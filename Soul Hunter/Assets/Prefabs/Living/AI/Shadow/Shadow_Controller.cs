using UnityEngine;
using System.Collections;

public class Shadow_Controller : MonoBehaviour {

	GameObject Target = null;
	Ray newLocation;
	public float maintainedDistance = 6.0f;
	float orientation;
	float angle;
	// Use this for initialization
	void Start () 
	{
		Target = GameObject.FindGameObjectWithTag ("Player");
		Vector3 distance = gameObject.transform.position - Target.transform.position;
		Vector3 forward = Target.transform.forward;

		angle = Vector3.Angle (forward, distance);
		if (Vector3.Cross (forward, distance).y < 0)
			angle = 360 - angle;
		newLocation.direction = distance;
		newLocation.origin = Target.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		newLocation.origin = Target.transform.position;
		Vector3 newPosition = newLocation.GetPoint (maintainedDistance);
		transform.position = newPosition;
	}

}
