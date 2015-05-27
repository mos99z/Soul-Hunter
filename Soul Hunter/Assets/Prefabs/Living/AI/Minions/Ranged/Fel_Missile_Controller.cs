using UnityEngine;
using System.Collections;

public class Fel_Missile_Controller : MonoBehaviour {

	public float Damage = 500.0f;
	public float Speed = 0.25f;
	public float TurnSpeed = 360.0f;
	public float StartHeight = 1.5f;
	public GameObject SpellEffect = null;
	public GameObject ImpactEffect = null;
	public AudioSource SFXMoving = null;
	public AudioSource SFXImpact = null;
	private Vector3 ForwardDirection = Vector3.zero;
	private float killSwitch = 30.0f;
	private bool dieing = false;
	private GameObject player = null;
	
	void Start () {
		
		// Start at desired height
		Vector3 newHeight = transform.position;
		newHeight.y = StartHeight;
		transform.position = newHeight;
		
		// Face spell in right direction
		player = GameObject.FindGameObjectWithTag ("Player");
		Vector3 lookAt = player.transform.position;
		lookAt.y = StartHeight;
		transform.LookAt (lookAt);
		
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
			
			ForwardDirection = transform.forward;
			ForwardDirection.Normalize ();
			ForwardDirection *= Speed;
			transform.position += ForwardDirection;
			
			Vector3 RotateAt = player.transform.position;
			RotateAt.y = StartHeight;
			Vector3 crossProd = Vector3.Cross(transform.forward.normalized, (RotateAt - transform.position).normalized);
			if (crossProd.y > 0)
			{
				if (TurnSpeed < 0)
					TurnSpeed = -TurnSpeed;
			}
			else if (TurnSpeed > 0)
				TurnSpeed = -TurnSpeed;
			
			transform.RotateAround(transform.position, new Vector3(0, 1.0f, 0), TurnSpeed * Time.fixedDeltaTime);
			
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
			if (_object.tag == "Player")
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
			if (SFXMoving != null)
				SFXMoving.Stop();
			if (SpellEffect != null)
				SpellEffect.GetComponent<ParticleSystem>().Stop();
			else
				GetComponent<ParticleSystem>().Stop();
			if (SFXImpact != null)
				SFXImpact.Play();
			if (ImpactEffect != null)
				ImpactEffect.SetActive (true);
			Destroy (gameObject, 1.5f);
			GetComponent<BoxCollider> ().enabled = false;
		}
	}
}
