using UnityEngine;
using System.Collections;

public class FlameThrowerDamage : MonoBehaviour 
{
	public float damage;		// the amount of damage a flamethrower does
	public float hurtDelay;		// how long to wait between dealing damage

	float timer;				// used for counting against delay
	void Start () 
	{
		timer = 0.0f;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			other.SendMessage("TakeDamage", damage);
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			timer += Time.deltaTime;
			if (timer >= hurtDelay)
			{
				timer = 0.0f;
				other.SendMessage("TakeDamage", damage);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			timer = 0.0f;
		}
	}
}
