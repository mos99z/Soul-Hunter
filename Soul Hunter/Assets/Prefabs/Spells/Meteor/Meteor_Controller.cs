using UnityEngine;
using System.Collections;

public class Meteor_Controller : MonoBehaviour 
{
	public GameObject mouseMarker;		// mouse marker from game brain
	public float duration = 4.0f;		// how long for meteor to hit the ground
	public float startHeight = 30.0f;	// where on the yaxis to spawn the meteor
	public GameObject explosion;		// the explosion effect to play when colliding
	Vector3 target;						// where the meteor will land

	public float damage = 25.0f;		// damage to deal on spell
	public float recoveryCost = 3.0f;	// spell cooldown

	public GameObject burn;				// burning debuff
	public float burnDamage = 8.1f;		// burning damage per tick
	public float burnTick = 1.0f;		// how often burning ticks
	public float burnTime = 5.0f;		// how long burning lasts
	public GameObject cripple;			// crippled debuff

	Vector3 origPos;	// for lerp function

	void Start () 
	{
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		Vector3 spawn = target = mouseMarker.transform.position;
		spawn.y = startHeight;
		transform.position = origPos = spawn;

		if (burn == null)
			burn = GameBrain.Instance.GetComponent<DebuffMasterList>().burning;
		if (cripple == null)
			cripple = GameBrain.Instance.GetComponent<DebuffMasterList>().crippled;
		
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryCost);
		StartCoroutine("MoveMeteor", duration);
	}

	IEnumerator MoveMeteor(float time)
	{
		float currentTime = 0.0f;
		do {
			transform.position = Vector3.Lerp (origPos, target, currentTime / time);
			currentTime += Time.deltaTime;
			yield return null;
		} while (currentTime < time);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			bool isFrozen = false;
			// TODO: make  frozen work
			//int children = other.transform.childCount;
			//for (int child = 0; child < children; child++)
			//{
			//	if (transform.GetChild(child).name.Contains("Frozen"))
			//	{
			//		other.GetComponent<Living_Obj>().CurrHealth = 0;
			//		other.SendMessage("PulseCheck");
			//		Debug.Log("cripple + frozen killed " + other.name);
			//		isFrozen = true;
			//		break;
			//	}
			//}
			if (!isFrozen)
			{
				other.transform.SendMessage("TakeDamage", damage);
				GameObject burning = Instantiate(burn);
				burning.transform.parent = other.transform;
				burning.transform.localPosition = Vector3.zero;
				burning.GetComponent<Burning_Controller>().Damage = burnDamage;
				burning.GetComponent<Burning_Controller>().Duration = burnTime;
				burning.GetComponent<Burning_Controller>().TickCycle = burnTick;
				
				GameObject cripp = Instantiate(cripple);
				cripp.transform.parent = other.transform;
				cripp.transform.localPosition = Vector3.zero;
			}
			GameObject effect = (GameObject)Instantiate(explosion, transform.position, transform.rotation);
			Destroy (effect, 1.0f); // destroy the particle effect after it plays
			Destroy (gameObject);
		}
		else if (other.tag == "Solid")
		{
			GameObject effect = (GameObject)Instantiate(explosion, transform.position, transform.rotation);
			Destroy (effect, 1.0f); // destroy the particle effect after it plays
			Destroy (gameObject);
		}
	}
}
