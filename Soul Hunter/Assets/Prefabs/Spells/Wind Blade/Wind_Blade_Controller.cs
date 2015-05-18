using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wind_Blade_Controller : MonoBehaviour {

	public float RecoveryCost = 0.0f;
	public float PushBackDistance = 0.0f;
	public float Speed = 0.0f;
	public float Range = 10.0f;
	public float StartHeight = 1.0f;
	public GameObject EffectImpact = null;
	public GameObject SoundImpact = null;
	
	private Vector3 ForwardDirection = Vector3.zero;
	private GameObject MouseMarker = null;
	private GameObject Player = null;
	private Vector3 StartLocation = Vector3.zero;
	
	// Use this for initialization
	void Start () {
		// Start at desired height
		Vector3 newHeight = transform.position;
		newHeight.y = StartHeight;
		transform.position = newHeight;
		StartLocation = transform.position;

		MouseMarker = GameObject.FindGameObjectWithTag ("MouseMarker");
		Vector3 lookAt = MouseMarker.transform.position;
		Player = GameObject.FindGameObjectWithTag ("Player");
		
		//		// Face player at target direction
		//		lookAt.y = Player.transform.position.y;
		//		Player.transform.LookAt (lookAt);
		
		Player.SendMessage("SetRecoverTime", RecoveryCost, SendMessageOptions.RequireReceiver);
		GameObject.Find("GameBrain").BroadcastMessage("SpellCasted",gameObject.name, SendMessageOptions.DontRequireReceiver);
		// Play movement sound
		GetComponent<AudioSource>().Play();
		// Face spell in right direction
		lookAt.y = StartHeight;
		transform.LookAt (lookAt);
		// Set spell velocity
		ForwardDirection = lookAt - transform.position;
		ForwardDirection.Normalize ();
		ForwardDirection *= Speed;
	}
	
	// Update is called once per frame
	void Update ()
	{

	}
	
	void FixedUpdate ()
	{
		float distanceTravled = (StartLocation - transform.position).magnitude;
		// Reached range, Destroy self
		if (distanceTravled >= Range)
			Destroy (gameObject);

		// Move Spell forward
		transform.position += ForwardDirection;
	}
	
	void OnTriggerEnter(Collider _object){
		if (_object.tag == "Enemy")
		{
			Vector3 atCollider = _object.transform.position - transform.position;
			atCollider.Normalize();
			float dotProduct = Vector3.Dot(transform.forward.normalized, atCollider);

			// Make sure collided actually hit spell effect.
			if (dotProduct >= 0.0f)
			{
				// If impact sound effect is set, play sound at hit location.
				if (SoundImpact != null)
				{
					GameObject Sound = Instantiate(SoundImpact);
					Sound.transform.parent = _object.transform;
					Sound.transform.localPosition = new Vector3(0,0,0);
					Destroy(Sound, 0.5f);
				}
				// If impact effect is set, play effect at hit loaction
				if (EffectImpact != null)
				{
					GameObject Effect = Instantiate(EffectImpact);
					Effect.transform.parent = _object.transform;
					Effect.transform.localPosition = new Vector3(0,StartHeight,0);
					Destroy(Effect, 0.5f);
				}
				// Knock back hit target
				//			Vector3 pushDirection = Owner.transform.position;
				//			pushDirection.y = _object.transform.position.y;
				//			pushDirection = _object.transform.position - pushDirection;
				//			pushDirection.Normalize();
				//			pushDirection.z *= PushBackDistance;
				//			_object.transform.position += pushDirection;
				//pushDirection = _object.transform.position - pushDirection;
				//_object.transform.GetComponent<Rigidbody>().AddExplosionForce(PushBackDistance, pushDirection, 1.0f);
			}
		}
	}
}
