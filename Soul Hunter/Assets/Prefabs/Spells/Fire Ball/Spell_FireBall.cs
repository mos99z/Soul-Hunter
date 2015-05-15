using UnityEngine;
using System.Collections;

public class Spell_FireBall : MonoBehaviour {
	public float CoolDownCost = 0.0f;
	public float Speed = 0.0f;
	public float DropRate = 0.001f;
	public GameObject ImpactEffect = null;
	public GameObject Fireball = null;

	// PRIVATES
	private Vector3 ForwardDirection = Vector3.zero;
	private GameObject Owner = null;
	private GameObject MouseMarker = null;
	private GameObject Player = null;
	private float killSwitch = 20.0f;
	private bool dieing = false;

	// Use this for initialization
	void Start () {
		Player = GameObject.FindGameObjectWithTag ("Player");
		MouseMarker = GameObject.FindGameObjectWithTag ("MouseMarker");
		Owner = GameObject.FindGameObjectWithTag ("SpellCaster");
		Vector3 lookat = MouseMarker.transform.position;
		lookat.y = Player.transform.position.y;
		Player.transform.LookAt (lookat);

		GameObject.FindGameObjectWithTag("SpellCaster").GetComponent<SpellCaster>().SendMessage("SetCoolDown", CoolDownCost, SendMessageOptions.RequireReceiver);
		GameObject.Find("Main").BroadcastMessage("SpellCasted", SendMessageOptions.DontRequireReceiver);
		GetComponent<AudioSource>().Play();
		gameObject.GetComponent<BoxCollider>().enabled = true;
		lookat.y = Owner.transform.position.y;
		ForwardDirection = lookat - Owner.transform.position;
		ForwardDirection.Normalize ();
		ForwardDirection *= Speed;
	}
	
	// Update is called once per frame
	void Update () {
		killSwitch -= Time.deltaTime;
		if (killSwitch <= 0.0f)
			Destroy (gameObject);
	}

	void FixedUpdate () {
		if (!dieing) {
			ForwardDirection.y -= DropRate;
			DropRate += DropRate * 0.1f;
			transform.position += ForwardDirection;
			Vector3 temp = GetComponent<BoxCollider> ().size;
			temp.y = transform.position.y * 1.8f;
			GetComponent<BoxCollider> ().size = temp;
			temp = GetComponent<BoxCollider> ().center;
			temp.y = -transform.position.y;
			GetComponent<BoxCollider> ().center = temp;
		}
	}

	void OnTriggerEnter(Collider _object){
		if (!dieing) {
			if (_object.tag == "Enemy") {
				GetComponent<AudioSource> ().Play ();
				DestroyMe ();
			} else if (_object.tag == "Solid") {
				DestroyMe ();
			}
		}
	}

    void OnTriggerExit(Collider _object){
		if (!dieing) {
			if (_object.tag == "Enemy") {
				DestroyMe ();
			} else if (_object.tag == "Solid") {
				DestroyMe ();
			}
		}
    }

	void DestroyMe(){
		dieing = true;
		AudioSource[] sounds = GetComponents<AudioSource> ();
		sounds[0].Stop();
		sounds[1].Play();

		ImpactEffect.SetActive(true);
		Destroy (gameObject, 1.5f);
		Fireball.SetActive (false);
		GetComponent<BoxCollider> ().enabled = false;
	}
}