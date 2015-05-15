using UnityEngine;
using System.Collections;

public class Meteorite_Controller : MonoBehaviour {

	public float FallSpeed = 0.75f;
	public float StartHeight = 30.0f;

	private bool UpdatePos = true;
	// Use this for initialization
	void Start () {
		transform.position += new Vector3 (0, StartHeight, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		if (UpdatePos) {
			transform.position -= new Vector3(0,FallSpeed,0);
		}
	}

	void OnTriggerEnter(Collider _object){

		if (_object.tag == "Solid") {
			transform.Find("FireBall").gameObject.SetActive(false);
			transform.Find("Impact Explosion").gameObject.SetActive(true);
			Destroy(gameObject, 0.5f);
			UpdatePos = false;
		}
	}
}
