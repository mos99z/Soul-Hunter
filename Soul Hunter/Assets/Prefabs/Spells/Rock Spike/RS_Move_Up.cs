using UnityEngine;
using System.Collections;

public class RS_Move_Up : MonoBehaviour {

	public float Damage = 0.3f;
	private Vector3 Velocity = new Vector3(0, 0.15f, 0);
	private int CollisionCheck = 0;

	public LayerMask Colliders;
	void Start ()
	{
		Vector3 startPosition = transform.position;
		startPosition.y = 10.0f;
		Ray rayUp = new Ray (startPosition, new Vector3 (0, -1.0f, 0));
		if (!Physics.Raycast (rayUp, 50.0f, Colliders)) {
			GetComponentInParent<Rock_Spike_Controller>().MaxChildren = GetComponentInParent<Rock_Spike_Controller>().Child;
			transform.gameObject.SetActive(false);
			return;
		}
		Velocity *= -transform.localPosition.y;
		Destroy(transform.GetChild (0).gameObject, 1.25f);
		transform.GetChild (0).SetParent (null);
	}

	void FixedUpdate ()
	{
		transform.localPosition += Velocity;
		if (transform.localPosition.y >= 0.0f)
			this.enabled = false;
	}

	void OnTriggerEnter(Collider _obj)
	{
		++CollisionCheck;
		if (_obj.tag == "Enemy")
		{
			_obj.SendMessage ("TakeDamage", Damage);
		}
	}
}
