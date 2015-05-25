using UnityEngine;
using System.Collections;

public class Timed_Spike_Controller : MonoBehaviour {

	public float Damage = 500.0f;
	public float OffSet = 0.0f;
	public float ActivateTimer = 2.0f;
	public float DeactivateTimer = 0.25f;
	private float Timer = 0.0f;

	private bool Active = false;

	public Animator animations = null;

	void Start()
	{
		animations = GetComponentInParent<Animator> ();
		Reset ();
	}

	void Update ()
	{
		if (OffSet > 0.0f)
			OffSet -= Time.deltaTime;
		else
			Timer += Time.deltaTime;

		if (Active)
		{
			if (Timer >= DeactivateTimer)
				Reset ();
		}
		else
		{
			if (Timer >= ActivateTimer)
				Trigger ();
		}
	}

	void OnTriggerEnter(Collider _object)
	{
		if (_object.tag == "Player" || _object.tag == "Enemy")
			_object.SendMessage ("TakeDamage", Damage);
	}

	void Trigger()
	{
		Active = true;
		Timer = 0.0f;
		animations.Play ("Triggered");
	}

	void Reset()
	{
		Active = false;
		Timer = 0.0f;
		animations.Play ("Reset"); 
	}
}
