﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pressure_Spike_Controller : MonoBehaviour {

	public float Damage = 500.0f;

	public float TriggerWait = 0.5f;
	private float TriggerTimer = 0.0f;
	private bool Triggered = false;
	private bool Triggering = false;

	public float ResetWait = 1.0f;
	private float ResetTimer = 0.0f;
	private bool Resetting = false;

	private List<GameObject> OnTop = new List<GameObject>();
	

	public AudioSource SFX = null;
	private Animator animations = null;

	void Start()
	{
		if (animations == null)
			animations = GetComponent<Animator> ();
		if (SFX == null)
			SFX = GetComponent<AudioSource> ();
	}

	void Update ()
	{
		if (Triggering)
		{
			TriggerTimer -= Time.deltaTime;
			if (TriggerTimer <= 0.0f)
			{
				TriggerTimer = 0.0f;
				Trigger();
			}
		}

		if (Resetting && !Triggering)
		{
			ResetTimer -= Time.deltaTime;
			if (ResetTimer <= 0.0f)
				Reset();
		}

		for (int OnTrap = 0; OnTrap < OnTop.Count; OnTrap++)
		{
			if (OnTop[OnTrap] == null)
			{
				OnTop.RemoveAt(OnTrap);
				if (OnTop.Count == 0)
					Resetting = true;
			}
		}
	}

	void OnTriggerEnter(Collider _object)
	{
		if (_object.tag == "Player" || _object.tag == "Enemy")
			OnTop.Add(_object.gameObject);

		if (OnTop.Count == 1 && !Triggering)
		{
			if (!Triggered)
			{
				Triggering = true;
				TriggerTimer = TriggerWait;
			}
			Resetting = false;
			ResetTimer = ResetWait;
		}
	}

	void OnTriggerExit(Collider _object)
	{
		if (_object.tag == "Player" || _object.tag == "Enemy")
		{
			for (int i = 0; i < OnTop.Count; i++)
			{
				if (OnTop[i] == _object.gameObject)
				{
					OnTop.RemoveAt(i);
					if (OnTop.Count == 0)
						Resetting = true;
					break;
				}
			}
		}
	}

	void Trigger()
	{
		Triggered = true;
		Triggering = false;

		SFX.Play ();
		animations.Play ("Triggered");

		for (int i = 0; i < OnTop.Count; i++) {
			if(OnTop[i] == null){
				OnTop.RemoveAt(i);
				--i;
			}
			else
				OnTop[i].transform.SendMessage("TakeDamage", Damage);
		}

	}

	void Reset()
	{
		Resetting = false;
		Triggered = false;

		animations.Play ("Reset");
	}
}
