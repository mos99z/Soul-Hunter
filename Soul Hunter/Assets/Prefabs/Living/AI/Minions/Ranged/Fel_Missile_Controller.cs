using UnityEngine;
using System.Collections;

public class Fel_Missile_Controller : MonoBehaviour {

	public float Speed = 0.15f;
	public float Damage = 100.0f;
	private bool dieing = false;

	void Start () 
	{
	}

	void FixedUpdate () 
	{
		gameObject.transform.position += transform.forward.normalized * Speed;
	}

	void OnTriggerEnter(Collider col)
	{
		if (!dieing) {
			if (col.tag == "Player" || col.tag == "Solid")
			{
				if (col.tag == "Player")
					col.SendMessage ("TakeDamage", Damage);
				dieing = true;
				Destroy (gameObject, 0.5f);
				Speed = 0.0f;
			}
		}
	}
}
