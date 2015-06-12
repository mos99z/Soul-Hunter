using UnityEngine;
using System.Collections;

public class Stalactite_Controller : MonoBehaviour {

	public float StartingHeight = 30.0f;
	public float FallSpeed = 0.75f;
	public float Damage = 100.0f;

	// Use this for initialization
	void Start () 
	{
		transform.position = new Vector3 (transform.position.x,StartingHeight,transform.position.z);
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		transform.position -= new Vector3(0,FallSpeed,0);
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
		{
			col.SendMessage("TakeDamage",Damage);
			Destroy(gameObject);
		}

		else
		{
			Destroy(gameObject);
		}
	}
}
