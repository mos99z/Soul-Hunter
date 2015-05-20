using UnityEngine;
using System.Collections;

public class Bolt_Controller : MonoBehaviour {

	public float Damage = 50.4f;
	public float RecoveryCost = 1.0f;
	public float StartHeight = 1.5f;
	public float Speed = 1.5f;
	public float Range = 10.0f;
	public float MaxSpreadAngle = 0.0f;
	public GameObject SpellEffect = null;
	public GameObject ImpactEffect = null;
	public GameObject Stunned = null;
	public AudioSource SFXMoving = null;
	public AudioSource SFXImpact = null;
	public float StunDuration = 0.5f;

	private Vector3 StartLocation = Vector3.zero;
	private Vector3 ForwardDirection = Vector3.zero;
	private bool dieing = false;
	private float killSwitch = 5.0f;
	void Start ()
	{
		// Start at desired height
		Vector3 newHeight = transform.position;
		newHeight.y = StartHeight;
		transform.position = newHeight;

		StartLocation = transform.position;

		Vector3 Target = GameObject.FindGameObjectWithTag ("MouseMarker").transform.position;
		Target.y = transform.position.y;
		transform.LookAt(Target);
		transform.RotateAround (transform.position, new Vector3 (0, 1, 0), Random.Range(-MaxSpreadAngle * 0.5f, MaxSpreadAngle * 0.5f));
		ForwardDirection = transform.forward.normalized * Speed;

		GameObject.FindGameObjectWithTag ("Player").SendMessage("SetRecoverTime", RecoveryCost, SendMessageOptions.RequireReceiver);
	}

	void Update ()
	{
		killSwitch -= Time.deltaTime;
		// Kill if alive too long, safty net incase no collision happens
		if (killSwitch <= 0.0f)
			Destroy (gameObject);
	}

	void FixedUpdate ()
	{
		if (!dieing)
		{
			transform.position += ForwardDirection;
			float distance = (transform.position - StartLocation).magnitude;
			
			if (distance >= Range)
				Destroy (gameObject);
		}	
	}
	
	void OnTriggerEnter(Collider _object)
	{
		if (!dieing)
		{
			float damageMod = 1.0f;
			if (_object.tag == "Enemy")
			{
				if (_object.transform.Find ("Wet(Clone)"))
				{
					GameObject stun = Instantiate (Stunned);
					stun.transform.parent = _object.transform;
					stun.transform.localPosition = new Vector3 (0, -_object.transform.position.y, 0);
					stun.GetComponent<StunnedDebuffController> ().Duration = StunDuration;
					damageMod = 2.0f;
				}

				_object.transform.SendMessage ("TakeDamage", Damage * damageMod);

				DestroyMe();
			}
			else if (_object.tag == "Solid")
				DestroyMe();
		}
	}

	void DestroyMe()
	{
		if (!dieing)
		{
			dieing = true;
			if (SFXMoving != null)
				SFXMoving.Stop();
			if (SpellEffect != null)
				SpellEffect.SetActive (false);
			if (SFXImpact != null)
				SFXImpact.Play();
			if (ImpactEffect != null)
				ImpactEffect.SetActive (true);
			Destroy (gameObject, 0.4f);
			GetComponent<BoxCollider> ().enabled = false;
		}
	}
}
