using UnityEngine;
using System.Collections;

public class Barrier_Controller : MonoBehaviour 
{
	public float duration = 4.0f;		// how long for spell to last
	public float damage = 2.0f;			// how much damage to deal
	public float rotationSpeed = 90.0f;	// how much for spell to spin
	public float defence = 0.5f;		// how much defence to give player
	public float recoveryTime = 2.0f;	// how long for spell to cooldown

	float origDefence;		// save original defense
	float timer = 0.0f;		// for ticking damage
	void Start () 
	{
		origDefence = GameBrain.Instance.Player.GetComponent<Living_Obj>().Defence;
		GameBrain.Instance.Player.GetComponent<Living_Obj>().Defence = defence;

		transform.parent = GameBrain.Instance.Player.transform;
		transform.position = GameBrain.Instance.Player.transform.position;
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);
		float Divi = (float)GameBrain.Instance.NumberOfLevels * 10.0f;
		defence += (float)GameBrain.Instance.FireLevel / Divi + (float)GameBrain.Instance.ElectricLevel / Divi + (float)GameBrain.Instance.WaterLevel / Divi;
		if (defence > 1.0f)
			defence = 1.0f;
	}

	void Update()
	{
		transform.RotateAround (transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
		duration -= Time.deltaTime;
		if (duration <= 0.0f)
			Destroy(gameObject);
	}

	void OnDestroy()
	{
		GameBrain.Instance.Player.GetComponent<Living_Obj>().Defence = origDefence;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			other.transform.SendMessage("TakeDamage", damage);
		}
	}
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Enemy")
		{
			timer += Time.deltaTime;
			if (timer >= 1.0f)
			{
				timer = 0.0f;
				other.transform.SendMessage("TakeDamage", damage);
			}
		}
	}
}
