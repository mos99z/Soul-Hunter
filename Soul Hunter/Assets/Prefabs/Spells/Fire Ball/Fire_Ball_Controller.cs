﻿using UnityEngine;
using System.Collections;

public class Fire_Ball_Controller : MonoBehaviour {

	public float RecoveryCost = 0.0f;
	public float Speed = 0.0f;
	public float DropRate = 0.001f;
	public float StartHeight = 1.5f;
	public GameObject ImpactEffect = null;
	public GameObject Fireball = null;

	private Vector3 ForwardDirection = Vector3.zero;
	private GameObject MouseMarker = null;
	private GameObject Player = null;
	private float killSwitch = 5.0f;
	private bool dieing = false;

	void Start () {
		// Start at desired height
		Vector3 newHeight = transform.position;
		newHeight.y = StartHeight;
		transform.position = newHeight;

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

	void Update () {
		killSwitch -= Time.deltaTime;
		// Kill if alive too long, safty net incase no collision happens
		if (killSwitch <= 0.0f)
			Destroy (gameObject);
	}
	
	void FixedUpdate () {
		if (!dieing) {
			// Slowly drop the fireball faster and faster, while moving it forward
			ForwardDirection.y -= DropRate;
			DropRate += DropRate * 0.1f;
			transform.position += ForwardDirection;
			// Set the box collider from the floor up 6 so spell wont go over or under targets.
			Vector3 temp = GetComponent<BoxCollider> ().center;
			temp.y = 3.1f - transform.position.y;
			GetComponent<BoxCollider> ().center = temp;
		}
	}
	
	void OnTriggerEnter(Collider _object){
		if (!dieing) {
			if (_object.tag == "Enemy" || _object.tag == "Solid")
			{
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
		if (!dieing) {
			dieing = true;
			AudioSource[] sounds = GetComponents<AudioSource> ();
			// Moving
			sounds [0].Stop ();
			// Collision
			sounds [1].Play ();
			
			ImpactEffect.SetActive (true);
			Destroy (gameObject, 1.5f);
			Fireball.SetActive (false);
			GetComponent<BoxCollider> ().enabled = false;
		}
	}
}
