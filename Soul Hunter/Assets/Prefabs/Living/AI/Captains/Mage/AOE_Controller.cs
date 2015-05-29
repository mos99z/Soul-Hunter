using UnityEngine;
using System.Collections;

public class AOE_Controller : MonoBehaviour {

	public float Damage = 100.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
			col.SendMessage ("TakeDamage", Damage);
	}
}
