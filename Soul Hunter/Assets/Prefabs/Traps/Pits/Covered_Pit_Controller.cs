using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Covered_Pit_Controller : MonoBehaviour {

	private float Duration = 3.0f;
	private float DurationTimer = 0.0f;
	private List<GameObject> OnTop = new List<GameObject>();
	private Animator Animate = null;

	private bool TurnOn = true;
	private bool One = true;
	private bool Two = true;
	private bool Three = true;
	private bool Four = true;


	void Start()
	{
		DurationTimer = Duration;
		Animate = transform.GetComponentInChildren<Animator> ();
	}
	void Update ()
	{
		for (int i = 0; i < OnTop.Count; i++) {
			if (OnTop[i] == null)
				OnTop.RemoveAt(i);
		}

		if (OnTop.Count > 0)
		{
			float Percent = DurationTimer / Duration;
			DurationTimer -= Time.deltaTime;
			if (DurationTimer <= 0.0f)
			{
				Destroy(gameObject);
			}
			else if (One && Percent <= 0.8f && Percent > 0.6f)
			{
				Animate.Play("CrackingFloor");
				One = false;
				if (TurnOn){
					transform.GetComponentInChildren<SpriteRenderer>().enabled = true;
					TurnOn = false;
				}
			}else if (Two && Percent <= 0.6f && Percent > 0.4f)
			{
				Animate.Play("CrackingFloor2");
				Two = false;
			}else if (Three && Percent <= 0.4f && Percent > 0.2f)
			{
				Animate.Play("CrackingFloor3");
				Three = false;
			}else if (Four && Percent <= 0.2f)
			{
				Animate.Play("CrackingFloor4");
				Four = false;
			}
		}
	}

	void OnTriggerEnter(Collider _obj)
	{
		bool isUnique = true;
		if (_obj.tag == "Player" || _obj.tag == "Enemy") {
			for (int i = 0; i < OnTop.Count; i++) {
				if (OnTop [i] == _obj.gameObject) {
					isUnique = false;
					break;
				}
			}
			if (isUnique)
				OnTop.Add(_obj.gameObject);
		}
	}

	void OnTriggerExit(Collider _obj)
	{
		if (_obj.tag == "Player" || _obj.tag == "Enemy")
		{

			for (int i = 0; i < OnTop.Count; i++)
			{
				if(OnTop[i] == _obj.gameObject)	
					OnTop.RemoveAt (i);
			}
		}
	}
}
