using UnityEngine;
using System.Collections;

public class RS_Move_Up : MonoBehaviour {

	public float Damage = 0.3f;
	private Vector3 Velocity = new Vector3(0, 0.15f, 0);

	void Start ()
	{
		Velocity *= -transform.localPosition.y;
	}

	void FixedUpdate ()
	{
		transform.localPosition += Velocity;
		if (transform.localPosition.y >= 0.0f)
			this.enabled = false;
	}

	void OnTriggerEnter(Collider _obj)
	{
		if (_obj.tag == "Enemy")
		{
			_obj.SendMessage("TakeDamage", Damage);
			_obj.GetComponent<Rigidbody>().AddExplosionForce(10.0f, transform.position, 1.0f);
		}
	}
}
