using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DustDevil_Controller : MonoBehaviour {

	private float Damage = 500.0f;
	private float DamageTickRate = 1.0f;
	private float TickTime = 1.0f;

	private float AliveTime = 0.0f;
	private float Duration = 7.0f;

	private bool Dieing = false;
	private GameObject Player = null;
	private bool DealDamage = false;
	private float SpellActivateTimer = 1.0f;
	private bool SpellActivated = false;
	private ParticleSystem[] ParticleSystems = new ParticleSystem[2];

	void Start ()
	{
		ParticleSystems[0] = transform.GetChild (1).GetComponent<ParticleSystem>();
		ParticleSystems[1] = transform.GetChild (2).GetComponent<ParticleSystem>();
		Player = GameObject.FindWithTag ("Player");
	}
	
	void Update ()
	{
		if (!SpellActivated)
		{
			SpellActivateTimer -= Time.deltaTime;
			if (SpellActivateTimer <= 0.0f)
			{
				ParticleSystems[0].Play();
				ParticleSystems[1].Play();
				SpellActivated = true;
			}
		}
		else if (!Dieing)
		{
			if (TickTime >= DamageTickRate)
			{
				TickTime = 0.0f;
				if (DealDamage)
					Player.SendMessage ("TakeDamage", Damage);
			}
			TickTime += Time.deltaTime;

			AliveTime += Time.deltaTime;
			if (AliveTime >= Duration - 0.5f)
			{
				Destroy (gameObject, 0.5f);
				for (int i = 0; i < ParticleSystems.Length; i++)
					ParticleSystems [i].Stop ();
				Dieing = true;
				if (DealDamage)
					Player.SendMessage ("TakeDamage", Damage);
				return;
			}
		}
	}
	
	void OnTriggerEnter(Collider _object)
	{
		if (_object.tag == "Player")
			DealDamage = true;
	}
	
	void OnTriggerExit(Collider _object)
	{
		if (_object.tag == "Player")
			DealDamage = false;
	}
}
