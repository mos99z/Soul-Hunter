using UnityEngine;
using System.Collections;

public class Spell_Lightning : MonoBehaviour {
	public float CoolDownCost = 0.0f;
	public float Speed = 0.0f;
	public float Range = 10.0f;
	public float MaxSpreadAngle = 0.0f;
	public GameObject ImpactEffect = null;
	public GameObject Lightingbolt = null;
	public int Damage = 10;
	public GameObject Stunned = null;
	public float StunDuration = 0.5f;

	// PRIVATES
	private Vector3 Target = Vector3.zero;
	private Vector3 StartLoc = Vector3.zero;
	private Vector3 ForwardDirection = Vector3.zero;
	//private bool Once = true;
	private GameObject Owner = null;
	private GameObject MouseMarker = null;
	private GameObject Player = null;
	private bool dieing = false;
	//private bool dieing = false;
	
	// Use this for initialization
	void Start () {
		Player = GameObject.FindGameObjectWithTag ("Player");
		MouseMarker = GameObject.FindGameObjectWithTag ("MouseMarker");
		Owner = GameObject.FindGameObjectWithTag ("SpellCaster");

		Target = MouseMarker.transform.position;
		Target.y = Owner.transform.position.y;
		transform.position = Owner.transform.position;
		StartLoc = Owner.transform.position;
		transform.LookAt(Target);
		transform.RotateAround (transform.position, new Vector3 (0, 1, 0), Random.Range(-MaxSpreadAngle * 0.5f, MaxSpreadAngle * 0.5f));
		ForwardDirection = transform.forward.normalized * Speed;

		Target.y = Player.transform.position.y;
		Player.transform.LookAt(Target);

		GameObject.FindGameObjectWithTag("SpellCaster").GetComponent<SpellCaster>().SendMessage("SetCoolDown", CoolDownCost, SendMessageOptions.RequireReceiver);
		GameObject.Find("Main").BroadcastMessage("SpellCasted", SendMessageOptions.DontRequireReceiver);
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void FixedUpdate () {
		if (!dieing) {
			transform.position += ForwardDirection;
			float distance = (transform.position - StartLoc).magnitude;
			Vector3 Scale = transform.localScale;
			Scale.z = 10.0f * (distance / Range);
			transform.localScale = Scale;
			
			Vector3 temp = GetComponent<BoxCollider> ().size;
			temp.y = transform.position.y * 2;
			GetComponent<BoxCollider> ().size = temp;
			temp = GetComponent<BoxCollider> ().center;
			temp.y = -transform.position.y;
			GetComponent<BoxCollider> ().center = temp;
			
			if (distance >= Range) {
				Destroy (gameObject);
			}
		}
	}
	
	void OnTriggerEnter(Collider _object){
		float damageMod = 1.0f;
		if (_object.tag == "Enemy") {
			if (_object.transform.Find("Wet(Clone)")){
				GameObject stun = Instantiate(Stunned);
				stun.transform.parent = _object.transform;
				stun.transform.localPosition = new Vector3(0,-_object.transform.position.y,0);
				stun.GetComponent<StunnedDebuffController>().LockPosition = _object.transform.position;
				MonoBehaviour[] scripts = _object.transform.GetComponents<MonoBehaviour>();
				stun.GetComponent<StunnedDebuffController>().Scripts = scripts;
				stun.GetComponent<StunnedDebuffController>().Duration = StunDuration;
				damageMod = 2.0f;
			}
			_object.transform.SendMessage("TakeDamage",(int)(Damage * damageMod));
			Lightingbolt.SetActive(false);
			Destroy (gameObject, 0.4f);
			dieing = true;
		}else if (_object.tag == "Solid"){
			Lightingbolt.SetActive(false);
			Destroy (gameObject, 0.4f);
			dieing = true;
		}
	}
}