using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spell_Wind : MonoBehaviour {
	public float CoolDownCost = 0.0f;
	public float PushBackDistance = 0.0f;
	public float Speed = 0.0f;
	public float Range = 10.0f;
	public float Angle = 90.0f;
	public float GrowthRate = 0.25f;
	public GameObject ImpactEffect = null;
	public GameObject SoundImpact = null;

	private float Rotated = 0.0f;
	private Vector3 Target = Vector3.zero;
	private Vector3 ForwardDirection = Vector3.zero;
	private GameObject Owner = null;
	private GameObject MouseMarker = null;
	private GameObject Player = null;
	private bool Once = true;
	private Vector3 StartLoc = Vector3.zero;

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Damage_Message>().CollidableTag = "Enemy";
		Player = GameObject.FindGameObjectWithTag ("Player");
		MouseMarker = GameObject.FindGameObjectWithTag ("MouseMarker");
		Owner = GameObject.FindGameObjectWithTag ("SpellCaster");

		GameObject.FindGameObjectWithTag("SpellCaster").GetComponent<SpellCaster>().SendMessage("SetCoolDown", CoolDownCost, SendMessageOptions.RequireReceiver);
		GameObject.Find("Main").BroadcastMessage("SpellCasted", SendMessageOptions.DontRequireReceiver);
		Target = MouseMarker.transform.position;
		Target.y = Player.transform.position.y;
		Player.transform.LookAt (Target);
		StartLoc = Player.transform.position;
		
		Target.y = Owner.transform.position.y;
		ForwardDirection = Target - Owner.transform.position;
		Target.y = Player.transform.position.y;
		ForwardDirection.Normalize ();
		transform.forward = ForwardDirection;
		transform.RotateAround(Owner.transform.position, new Vector3(0,1,0), -Angle * 0.5f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Once)
		{
			Player.transform.position = StartLoc;
			Player.transform.LookAt(Target);
			Vector3 newScale = transform.localScale;
			newScale.z += GrowthRate;
			if (newScale.z >= Range){
				newScale.z = Range;
				Once = false;
				GetComponent<BoxCollider>().enabled = true;
			}
			transform.localScale = newScale;
			transform.position = Owner.transform.position + transform.forward.normalized * 0.5f * transform.localScale.z;
		}
	}

	void FixedUpdate ()
	{
		if (!Once) {
			Player.transform.position = StartLoc;
			Player.transform.LookAt(Target);
			transform.RotateAround(Owner.transform.position, new Vector3(0,1,0), Speed);
			transform.position = Owner.transform.position + transform.forward.normalized * 0.5f * transform.localScale.z;
			Rotated += Speed;

			if (Rotated >= Angle)
				Destroy (gameObject);
		}
	}

	void OnTriggerEnter(Collider _object){
		if (_object.tag == "Enemy")
		{
			GameObject impactsound = Instantiate(SoundImpact);
			impactsound.transform.parent = _object.transform;
			impactsound.transform.localPosition = new Vector3(0,0,0);
			Destroy(impactsound, 0.5f);


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