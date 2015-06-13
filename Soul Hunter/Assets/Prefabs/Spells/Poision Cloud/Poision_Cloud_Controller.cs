using UnityEngine;
using System.Collections;

public class Poision_Cloud_Controller : MonoBehaviour 
{
	public GameObject mouseMarker;		// mouse marker from game brain
	public float duration = 2.0f;		// how long spell lasts
	public float damage = 5.0f;			// how much damage it does
	public float recoveryTime = 1.5f;	// how long for spell to recharge
	
	float timer = 0.0f;		// for ticking damage
	
	// TODO: enable this when blind is implemented
	//	public GameObject blind;	// debuff to apply
	void Start () 
	{
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		transform.position = mouseMarker.transform.position;
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);
	}
	
	void Update()
	{
		duration -= Time.deltaTime;
		if (duration <= 0.0f)
			Destroy (gameObject);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			other.transform.SendMessage("TakeDamage", damage);
			// TODO: apply blind debuff
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Enemy")
		{
			timer += Time.deltaTime;
			if (timer >= 0.5f)
			{
				timer = 0.0f;
				other.transform.SendMessage("TakeDamage", damage);
			}
		}
	}
}
