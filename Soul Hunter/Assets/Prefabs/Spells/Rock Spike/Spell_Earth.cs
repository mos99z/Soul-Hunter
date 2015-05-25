using UnityEngine;
using System.Collections;

public class Spell_Earth : MonoBehaviour {
	public float CoolDownCost = 0.0f;
	public float ImpactRate = 0.0f;
	public int NumImpacts = 10;
	public float Range = 10.0f;
	public GameObject ImpactEffect = null;
	public Vector3 Scaler = new Vector3 (1, 6, 1);
	
	// PRIVATES
	private Vector3 StartLoc = Vector3.zero;
	private Vector3 ForwardDirection = Vector3.zero;
//	private GameObject Owner = null;
	private GameObject MouseMarker = null;
	private GameObject Player = null;
	private float impactPoints = 0.0f;
	private float TimePassed = 0.0f;
	
	// Use this for initialization
	void Start () {
		Player = GameObject.FindGameObjectWithTag ("Player");
		MouseMarker = GameObject.FindGameObjectWithTag ("MouseMarker");
//		Owner = GameObject.FindGameObjectWithTag ("SpellCaster");
		Vector3 lookat = MouseMarker.transform.position;
		lookat.y = Player.transform.position.y;
		Player.transform.LookAt (lookat);

//		GameObject.FindGameObjectWithTag("SpellCaster").GetComponent<SpellCaster>().SendMessage("SetCoolDown", CoolDownCost, SendMessageOptions.RequireReceiver);
//		GameObject.Find("Main").BroadcastMessage("SpellCasted", SendMessageOptions.DontRequireReceiver);
		transform.FindChild("Ground Spike").transform.GetComponent<AudioSource>().Play();
//		StartLoc = Owner.transform.position;
		StartLoc.y = -0.1f;
		transform.position = StartLoc;
		Vector3 TargetLoc = MouseMarker.transform.position;
		TargetLoc.y = -0.1f;
		ForwardDirection = TargetLoc - StartLoc;
		ForwardDirection.Normalize ();
		transform.position += ForwardDirection * 2.0f;
		StartLoc = transform.position;
		TargetLoc += ForwardDirection * 2.0f;
		ForwardDirection *= Range;
	}
	
	// Update is called once per frame
	void Update ()
	{
		TimePassed += Time.deltaTime;
		if (TimePassed >= ImpactRate) {
			TimePassed = 0.0f;
			transform.FindChild("Ground Spike").transform.GetComponent<AudioSource>().Play();
			impactPoints += 1.0f / (float)NumImpacts;
		}
	}

	void FixedUpdate () {
		transform.position = StartLoc + ForwardDirection * impactPoints;
//		float distance = (transform.position - StartLoc).magnitude;
		Vector3 Scale = Scaler;
//		if (impactPoints <= 0.5f) {
//			Scale.y = 10.0f * impactPoints;
//			Scale.x = 2.0f * impactPoints;
//			Scale.z = 2.0f * impactPoints;
//		} else {
//			Scale.y = 10.0f - 10.0f * (1 - impactPoints);
//			Scale.x = 2.0f - 2.0f * (1 - impactPoints);
//			Scale.z = 2.0f - 2.0f * (1 - impactPoints);
//		}

		//Vector3 correctY = transform.position;
		if (TimePassed <= 0.5f * ImpactRate) {
			Scale.y = Scaler.y * (TimePassed / ImpactRate);
		} else {
			Scale.y = Scaler.y * (1 - (TimePassed / ImpactRate));
		}
		//transform.position = correctY;
		transform.localScale = Scale;
		if (impactPoints >= 1.0f) {
			Destroy (gameObject);
		}
	}
}