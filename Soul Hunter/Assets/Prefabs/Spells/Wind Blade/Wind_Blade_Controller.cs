using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wind_Blade_Controller : MonoBehaviour
{

	public float Damage = 20.2f;
	public float RecoveryCost = 1.5f;
	public float Speed = 0.5f;
	public float Range = 10.0f;
	public float StartHeight = 1.0f;
	public GameObject SpellEffect = null;
	public GameObject ImpactEffectObj = null;
	public AudioSource SFXMoving = null;
	public int MaxTargetsHit = 5;
	private Vector3 StartLocation = Vector3.zero;
	private Vector3 ForwardDirection = Vector3.zero;
	private float killSwitch = 5.0f;
	float pushForce = 7.0f;
	
	void Start ()
	{
		transform.localScale *= 1.0f + ((float)GameBrain.Instance.WindLevel / (float)GameBrain.Instance.NumberOfLevels * 0.5f);
		Range += 2.0f * (float)GameBrain.Instance.WindLevel;
		pushForce += (float)GameBrain.Instance.WindLevel;

		// Start at desired height
		Vector3 newHeight = transform.position;
		newHeight.y = StartHeight;
		transform.position = newHeight;
		StartLocation = transform.position;

		// Face spell in right direction
		Vector3 lookAt = GameObject.FindGameObjectWithTag ("MouseMarker").transform.position;
		lookAt.y = StartHeight;
		transform.LookAt (lookAt);
		GameObject.FindGameObjectWithTag ("Player").SendMessage("SetRecoverTime", RecoveryCost, SendMessageOptions.RequireReceiver);

		// Set spell velocity
		ForwardDirection = lookAt - transform.position;
		ForwardDirection.Normalize ();
		ForwardDirection *= Speed;
	}
	
	// Update is called once per frame
	void Update ()
	{
		killSwitch -= Time.deltaTime;
		// Kill if alive too long, safty net incase no collision happens
		if (killSwitch <= 0.0f)
			Destroy (gameObject);
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
		if (MaxTargetsHit > 0 && _object.tag == "Enemy")
		{
			Vector3 atCollider = _object.transform.position - transform.position;
			atCollider.Normalize();
			float dotProduct = Vector3.Dot(transform.forward.normalized, atCollider);

			// Make sure collidie actually hit spell effect.
			if (dotProduct >= 0.0f)
			{
				// If impact effect is set, play effect at hit loaction
				if (ImpactEffectObj != null)
				{
					GameObject Effect = Instantiate(ImpactEffectObj);
					Effect.transform.parent = _object.transform;
					Effect.transform.localPosition = new Vector3(0,StartHeight,0);
					Destroy(Effect, 0.5f);
				}

				_object.transform.SendMessage ("TakeDamage", Damage);
				MaxTargetsHit--;
//				Knock back hit target
				Vector3 pushDirection = transform.forward;
				pushDirection.y = 0.0f;
				pushDirection.Normalize();
				pushDirection *= pushForce;
				_object.transform.GetComponent<Rigidbody>().velocity = pushDirection;
			}
		}
	}
}
