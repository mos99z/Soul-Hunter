using UnityEngine;
using System.Collections;

public class Spinning_Orbs_Controller : MonoBehaviour {

	private Vector3 RotationPoint = Vector3.zero;
	private Vector3 RotationAxis = new Vector3(0, 0, 1.0f);
	private float Angle = 360.0f;
	
	void Update ()
	{
		RotationPoint = transform.parent.transform.position;
		transform.RotateAround(RotationPoint, RotationAxis, Angle * Time.deltaTime);
	}
}
