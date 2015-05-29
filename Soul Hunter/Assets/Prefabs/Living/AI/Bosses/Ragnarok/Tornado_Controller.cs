using UnityEngine;
using System.Collections;

public class Tornado_Controller : MonoBehaviour {

	public float Damage = 5.0f;
	public float DamageTickRate = 0.1f;
	private float TickTime = 0.0f;
	
	private float AliveTime = 0.0f;
	private float Duration = 15.0f;
	
	private float MovementSpeed = 0.125f;
	
	private bool Dieing = false;
	private GameObject Player = null;
	private bool DealDamage = false;
	private float SpellActivateTimer = 1.5f;
	private bool SpellActivated = false;
	private ParticleSystem[] ParticleSystems = new ParticleSystem[2];
	private Vector3 ForwardDirection = Vector3.zero;
	
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
			AliveTime += Time.deltaTime;
			if (AliveTime >= Duration - 0.5f)
			{
				Destroy (gameObject, 0.5f);
				for (int i = 0; i < ParticleSystems.Length; i++)
					ParticleSystems [i].Stop ();
				Dieing = true;
				return;
			}
			
			TickTime += Time.deltaTime;
			if (TickTime >= DamageTickRate)
			{
				TickTime = 0.0f;
				if (DealDamage)
					Player.SendMessage ("TakeDamage", Damage);
			}
		}
	}
	
	void FixedUpdate()
	{
		if (SpellActivated)
		{
			ForwardDirection = Player.transform.position - transform.position;
			ForwardDirection.y = 0.0f;
			ForwardDirection.Normalize ();
			ForwardDirection *= MovementSpeed;
			transform.position += ForwardDirection;
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
