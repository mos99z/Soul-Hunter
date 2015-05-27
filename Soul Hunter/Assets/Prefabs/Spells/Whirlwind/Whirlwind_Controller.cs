using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Whirlwind_Controller : MonoBehaviour {

	public float DamageWind = 2.2f;
	public float DamageLightning = 1.4f;
	public float DamageTickRate = 0.1f;
	public float RecoveryCost = 1.5f;
	public float PullInForce = 10.0f;
	public float Duration = 3.0f;
	public LayerMask WALLS;
	
	private float AliveTime = 0.0f;
	private float TickTime = 0.0f;
	private bool Dieing = false;
	private List<GameObject> Hitting = new List<GameObject>();
	private bool Once = true;
	
	void Start ()
	{
	}
	
	void Update ()
	{
		if (Once)
		{
			GameObject player = GameObject.Find ("Player");
			Vector3 rayDirection = player.transform.FindChild("Direction Indicator").transform.forward;
			rayDirection.y = transform.position.y;
			Ray wallCheckRay = new Ray(transform.position, rayDirection);
			GameObject mouseMarker = GameObject.Find ("MouseMarker");
			float DistanceCheck = (mouseMarker.transform.position - transform.position).magnitude + 2.0f;
			if (Physics.Raycast (wallCheckRay, DistanceCheck, WALLS))
			{
				player.SendMessage("SetRecoverTime", 0.0f);
				Destroy(gameObject);
				return;
			}
			Once = false;
			Vector3 newPosition = mouseMarker.transform.position;
			newPosition.y = 0.0f;
			transform.position = newPosition;
			player.SendMessage("SetRecoverTime", RecoveryCost);
		}
		AliveTime += Time.deltaTime;
		if (!Dieing && AliveTime >= Duration - 0.5f) 
		{
			Destroy(gameObject, 0.5f);
			ParticleSystem[] ParticleSystems = GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < ParticleSystems.Length; i++)
				ParticleSystems[i].Stop();
			Dieing = true;
		}

		
		TickTime += Time.deltaTime;
		for (int i = 0; i < Hitting.Count; i++)
		{
			if(Hitting[i] == null)
			{
				Hitting.RemoveAt(i);
				i--;
				continue;
			}

			if (TickTime >= DamageTickRate)
			{
				Hitting[i].SendMessage ("TakeDamage", DamageWind);
				Hitting[i].SendMessage ("TakeDamage", DamageLightning);
			}

			Vector3 PullDirection = transform.position - Hitting[i].transform.position;
			PullDirection.y = 0.0f;
			if (Hitting[i].transform.GetComponent<Rigidbody>())
				Hitting[i].transform.GetComponent<Rigidbody>().AddForce(PullDirection * PullInForce);
		}
		
		if (TickTime >= DamageTickRate)
			TickTime = 0.0f;
	}
	
	void OnTriggerEnter(Collider _object)
	{
		if (_object.tag == "Enemy")
			Hitting.Add(_object.gameObject);
	}
	
	void OnTriggerExit(Collider _object)
	{
		if (_object.tag == "Enemy")
			Hitting.Remove(_object.gameObject);
	}
}
