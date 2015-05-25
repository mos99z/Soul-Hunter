using UnityEngine;
using System.Collections;

public class Fel_Missile_Controller : MonoBehaviour {

	public float Speed = 1.0f;
	public float Damage = 100.0f;
	Vector3 currentVelocity;

	// Use this for initialization
	void Start () 
	{
		currentVelocity = gameObject.transform.forward;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//currentVelocity.z = 1;
		currentVelocity *= Speed;
		gameObject.transform.position += currentVelocity;
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Player") 
		{
			col.gameObject.SendMessage("TakeDamage",Damage);
		}
	}
}
