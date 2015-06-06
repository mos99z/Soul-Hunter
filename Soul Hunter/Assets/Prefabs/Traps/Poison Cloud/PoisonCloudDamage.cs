using UnityEngine;
using System.Collections;

public class PoisonCloudDamage : MonoBehaviour 
{
	public float damage = 15.0f;		// the amount of damage a flamethrower does
	public float hurtDelay = 0.3f;		// how long to wait between dealing damage
	[Header ("Optional. If null, slow debuff does not apply in cloud")]
	public GameObject slowedDebuff;		// apply a slow debuff when entering poison cloud
	
	float timer;						// used for counting against delay
	void Start () 
	{
		timer = 0.0f;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			other.SendMessage("TakeDamage", damage);

			if (slowedDebuff != null)
			{
				GameObject debuff = Instantiate(slowedDebuff);
				debuff.transform.parent = other.transform;
				debuff.transform.localPosition = Vector3.zero;
			}
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
	}}
