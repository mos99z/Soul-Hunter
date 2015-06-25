using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Whirlwind_Controller : MonoBehaviour {

	public float DamageWind = 2.2f;
	public float DamageLightning = 1.4f;
	public float DamageTickRate = 0.1f;
	public float RecoveryCost = 1.5f;
	public float PullInForce = 10.0f;
	public float Duration = 3.0f;
	public int MaxEffecting = 5;
	public LayerMask WALLS;
	
	private float AliveTime = 0.0f;
	private float TickTime = 0.0f;
	private bool Dieing = false;
	private List<GameObject> Hitting = new List<GameObject>();
	
	void Start ()
	{
		RaycastHit colliderCheck = new RaycastHit();
		Vector3 distance = (GameBrain.Instance.MouseMarker.transform.position - GameBrain.Instance.Player.transform.position);
		distance.y = 0.0f;
		Physics.Raycast (GameBrain.Instance.Player.transform.position + new Vector3 (0, 1.0f, 0), 
		                 distance.normalized,
		                 out colliderCheck,
		                 distance.magnitude + transform.GetComponent<SphereCollider>().radius * transform.localScale.x,
		                 WALLS);
		Vector3 startpos = Vector3.zero;
		if (colliderCheck.collider != null)
			startpos = colliderCheck.point - distance.normalized * transform.GetComponent<SphereCollider>().radius * transform.localScale.x;
		else
			startpos = GameBrain.Instance.MouseMarker.transform.position;
		startpos.y = 0.0f;
		
		transform.position = startpos;

		Duration += 1.5f * (float)GameBrain.Instance.WindLevel;
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", RecoveryCost);
	}
	
	void Update ()
	{
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

			if (Hitting[i].GetComponent<Living_Obj>().entType == Living_Obj.EntityType.Minion)
			{
				Vector3 PullDirection = transform.position - Hitting[i].transform.position;
				PullDirection.y = 0.0f;
				if (Hitting[i].transform.GetComponent<Rigidbody>())
					Hitting[i].transform.GetComponent<Rigidbody>().AddForce(PullDirection * PullInForce * Time.deltaTime);
			}
		}
		if (TickTime >= DamageTickRate)
			TickTime = 0.0f;
	}
	
	void OnTriggerEnter(Collider _object)
	{
		if (_object.tag == "Enemy" && Hitting.Count <= MaxEffecting)
			Hitting.Add(_object.gameObject);
	}
	
	void OnTriggerExit(Collider _object)
	{
		if (_object.tag == "Enemy")
			Hitting.Remove(_object.gameObject);
	}
}
