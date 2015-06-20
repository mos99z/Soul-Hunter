using UnityEngine;
using System.Collections;

public class Fire_Ball_Controller : MonoBehaviour {

	public float Damage = 15.1f;
	public float RecoveryCost = 0.0f;
	public float Speed = 0.0f;
	public float Range = 12.0f;
	public float StartHeight = 1.5f;
	public GameObject SpellEffect = null;
	public GameObject ImpactEffect = null;
	public AudioSource SFXMoving = null;
	public AudioSource SFXImpact = null;

	private Vector3 StartLocation = Vector3.zero;
	private Vector3 ForwardDirection = Vector3.zero;
	private float killSwitch = 5.0f;
	private bool dieing = false;

	private float _50PercentDropRate = 0.0f;
	private bool Reached50 = false;
	private float _75PercentDropRate = 0.0f;
	private bool Reached75 = false;
	
	void Start () {
		_50PercentDropRate = 0.01f * StartHeight;
		_75PercentDropRate = 0.03f * StartHeight;

		SFXMoving.ignoreListenerVolume = false;
		SFXImpact.ignoreListenerVolume = false;

		// Start at desired height
		Vector3 newHeight = transform.position;
		newHeight.y = StartHeight;
		transform.position = newHeight;

		StartLocation = transform.position;

		// Face spell in right direction
		Vector3 lookAt = GameObject.FindGameObjectWithTag ("MouseMarker").transform.position;
		lookAt.y = StartHeight;
		transform.LookAt (lookAt);

		GameObject.FindGameObjectWithTag ("Player").transform.SendMessage("SetRecoverTime", RecoveryCost, SendMessageOptions.RequireReceiver);

		// Set spell velocity
		ForwardDirection = lookAt - transform.position;
		ForwardDirection.Normalize ();
		ForwardDirection *= Speed;
	}

	void Update () {
		killSwitch -= Time.deltaTime;
		// Kill if alive too long, safty net incase no collision happens
		if (killSwitch <= 0.0f)
			Destroy (gameObject);
	}
	
	void FixedUpdate () {
		if (!dieing) {
			if (!Reached75 && Reached50 && (StartLocation - transform.position).magnitude >= 0.75f * Range)
			{
				Reached75 = true;
				ForwardDirection.y = -_75PercentDropRate;
			}
			else if (!Reached50 && (StartLocation - transform.position).magnitude >= 0.5f * Range)
			{
				Reached50 = true;
				ForwardDirection.y = -_50PercentDropRate;
			}

			transform.position += ForwardDirection;

			// Set the box collider from the floor up 6 so spell wont go over or under targets.
			Vector3 temp = GetComponent<BoxCollider> ().center;
			temp.y = GetComponent<BoxCollider> ().size.y * 0.5f + 0.1f - transform.position.y;
			GetComponent<BoxCollider> ().center = temp;
		}
	}
	
	void OnTriggerEnter(Collider _object)
	{
		if (!dieing)
		{
			if (_object.tag == "Enemy")
			{
				DestroyMe ();
				_object.transform.SendMessage ("TakeDamage", Damage);
			}
			else if (_object.tag == "Solid")
			{
				DestroyMe ();
			}
		}
	}
	
	void DestroyMe()
	{
		if (!dieing)
		{
			dieing = true;
			SFXMoving.Stop();
			SpellEffect.SetActive (false);
			SFXImpact.Play();
			ImpactEffect.SetActive (true);
			Destroy (gameObject, 1.5f);
			GetComponent<BoxCollider> ().enabled = false;
		}
	}
}
