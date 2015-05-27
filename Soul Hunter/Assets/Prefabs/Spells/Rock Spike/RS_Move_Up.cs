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
		if (_obj.tag == "Enemy" && this.isActiveAndEnabled)
		{
			_obj.SendMessage ("TakeDamage", Damage);
			Vector3 crossProd = Vector3.Cross(_obj.transform.position - transform.position.normalized, transform.forward.normalized);
			GameObject pushDirection = new GameObject();
			pushDirection.transform.forward  = transform.parent.forward;
			Vector3 pushBack;
			if (crossProd.y < 0)
				pushDirection.transform.RotateAround(pushDirection.transform.position, new Vector3(0,1,0), 90.0f);
			else
				pushDirection.transform.RotateAround(pushDirection.transform.position, new Vector3(0,1,0), -90.0f);
			pushBack = pushDirection.transform.forward.normalized * 10.0f;
			_obj.transform.GetComponent<Rigidbody>().velocity = pushBack;
		}
	}
}
