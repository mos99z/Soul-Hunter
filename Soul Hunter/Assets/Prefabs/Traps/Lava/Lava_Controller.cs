using UnityEngine;
using System.Collections;

public class Lava_Controller : MonoBehaviour 
{
	public GameObject burnDebuff;				// set the burning debuff
	public float lavaDamage = 10.1f;			// how much damage lava does
	public float waitTime = 1.0f;				// how long to wait between doing damage
	public float BurningDOTDuration = 10.0f;	// how long for debuff to last
	public float BurningDOTTickCycle = 0.5f;	// how often to tick the debuff damage
	public float BurningDOTTickDamage = 5.1f;	// how much damage each tick does

	float timer;						// used for tracking time
	void Start () 
	{
		timer = 0.0f;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			other.transform.SendMessage("TakeDamage", lavaDamage);

			GameObject debuff = Instantiate(burnDebuff);
			debuff.transform.parent = other.transform;
			debuff.transform.localPosition = Vector3.zero;
			debuff.GetComponent<Burning_Controller>().Damage = BurningDOTTickDamage;
			debuff.GetComponent<Burning_Controller>().Duration = BurningDOTDuration;
			debuff.GetComponent<Burning_Controller>().TickCycle = BurningDOTTickCycle;
		}
	}
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			timer += Time.deltaTime;
			if (timer >= waitTime)
			{
				timer = 0.0f;
				other.transform.SendMessage("TakeDamage", lavaDamage);
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
