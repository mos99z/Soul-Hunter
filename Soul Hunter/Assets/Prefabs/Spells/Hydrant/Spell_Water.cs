using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spell_Water : MonoBehaviour {
	public float CoolDownCost = 0.0f;
	public float Duration = -1.11f;
	public float PushBackPower = 0.0f;
	public float Range = 10.0f;
	public GameObject ImpactEffect = null;
	public float DamageTickSpeed = 0.2f;
	public LayerMask CollideWIth;
	
	// PRIVATES
	private Vector3 Target = Vector3.zero;
	private bool Once = false;
	private GameObject Owner = null;
	private GameObject MouseMarker = null;
	private GameObject Player = null;
	private Vector3 StartLoc = Vector3.zero;
	private float TimePasted = 0.0f;


	// Use this for initialization
	void Start () {
		TimePasted = DamageTickSpeed;
		gameObject.GetComponent<Damage_Message>().CollidableTag = "Enemy";
		Player = GameObject.FindGameObjectWithTag ("Player");
		MouseMarker = GameObject.FindGameObjectWithTag ("MouseMarker");
		Owner = GameObject.FindGameObjectWithTag ("SpellCaster");
		Destroy(gameObject, Duration);
		StartLoc = Player.transform.position;

		GameObject.FindGameObjectWithTag("SpellCaster").GetComponent<SpellCaster>().SendMessage("SetCoolDown", CoolDownCost, SendMessageOptions.RequireReceiver);
		GameObject.Find("Main").BroadcastMessage("SpellCasted", SendMessageOptions.DontRequireReceiver);
	}
	
	// Update is called once per frame
	void Update () {
		TimePasted += Time.deltaTime;
		if (TimePasted >= DamageTickSpeed) {
			TimePasted = 0.0f;
			GetComponent<BoxCollider> ().enabled = true;
			GetComponent<CapsuleCollider> ().enabled = true;
			Once = true;
		} else if (Once){
			GetComponent<BoxCollider> ().enabled = false;
			GetComponent<CapsuleCollider> ().enabled = false;
			Once = false;
		}
	}
	
	void FixedUpdate () {
		Player.transform.position = StartLoc;
		float Shortest = float.MaxValue;
		RaycastHit rayInfo = new RaycastHit();
		List<Ray> rays = new List<Ray>();
		rays.Add(new Ray(Owner.transform.position - new Vector3(0,2,0), Owner.transform.forward));
		rays.Add(new Ray(Owner.transform.position - new Vector3(0.75f,2,0.75f), Owner.transform.forward));
		rays.Add(new Ray(Owner.transform.position - new Vector3(0.75f,2,0), Owner.transform.forward));
		rays.Add(new Ray(Owner.transform.position - new Vector3(0,2,-0.75f), Owner.transform.forward));
		rays.Add(new Ray(Owner.transform.position - new Vector3(-0.75f,2,-0.75f), Owner.transform.forward));
		for (int i = 0; i < 5; i++) {
			Physics.Raycast(rays[i], out rayInfo, Range, CollideWIth);
			if (rayInfo.distance < Shortest && rayInfo.distance != 0.0f)
				Shortest = rayInfo.distance;
		}
		Vector3 newScale = transform.localScale;
		newScale.z = Shortest + 1.0f;
		if (newScale.z > Range)
			newScale.z = Range;
		transform.localScale = newScale;
		Target = MouseMarker.transform.position;
		Target.y = Player.transform.position.y;
		Player.transform.LookAt(Target);
		Target = Owner.transform.position + Owner.transform.forward.normalized * Range;
		transform.position = Owner.transform.position + Owner.transform.forward.normalized * 0.5f * transform.localScale.z;
		transform.LookAt(Target);
	}
	
	void OnTriggerEnter(Collider _object){
		if (_object.tag == "Enemy")
		{
			Vector3 pushDirection = Owner.transform.position;
			pushDirection.y = _object.transform.position.y;
			pushDirection = _object.transform.position - pushDirection;
			pushDirection.Normalize();
			pushDirection *= PushBackPower;
			_object.transform.GetComponent<Rigidbody>().AddForce(pushDirection, ForceMode.Force);
		}
		else if (_object.tag == "Solid")
		{
		}
	}
}