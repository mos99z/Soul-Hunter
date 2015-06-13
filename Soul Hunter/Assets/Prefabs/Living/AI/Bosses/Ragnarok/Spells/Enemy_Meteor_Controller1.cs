using UnityEngine;
using System.Collections;

public class Enemy_Meteor_Controller1 : MonoBehaviour
{
	public Transform center = null;		// player from game brain
	public float duration = 4.0f;		// how long for meteor to hit the ground
	public float delay = 1.0f;			// how long to show the spot on ground before falling
	public GameObject targetIndicator;	// object to show where meteor will hit
	
	public float startHeight = 30.0f;	// where on the yaxis to spawn the meteor
	public GameObject explosion;		// the explosion effect to play when colliding
	Vector3 target;						// where the meteor will land
	Vector3 spawn;						// for keeping the indicator up to date
	
	public float damage = 25.0f;		// damage to deal on spell
	public GameObject burn;				// burning debuff
	public float burnDamage = 8.1f;		// burning damage per tick
	public float burnTick = 1.0f;		// how often burning ticks
	public float burnTime = 5.0f;		// how long burning lasts
	public GameObject cripple;			// crippled debuff
	
	Vector3 origPos;	// for lerp function
	
	void Start () 
	{
		GameObject centerObj = GameObject.Find("roomCenter");
		center = centerObj.transform;
		float tempX = Random.Range(center.position.x - 30, center.position.x + 30);
		float tempY = Random.Range(center.position.z - 30, center.position.z + 30);
		Vector3 tempCenter = new Vector3(tempX, 0.5f, tempY);
		spawn = target = tempCenter;
		spawn.y = startHeight;	// for meteor
		transform.position = origPos = spawn;
		spawn.y = 0.05f;			// for target location
		targetIndicator.transform.position = spawn;
		if (burn == null)
			burn = GameBrain.Instance.GetComponent<DebuffMasterList>().burning;
		if (cripple == null)
			cripple = GameBrain.Instance.GetComponent<DebuffMasterList>().crippled;
		
		StartCoroutine("MoveMeteor", duration);
	}
	
	IEnumerator MoveMeteor(float time)
	{
		float currentTime = 0.0f;
		do {
			transform.position = Vector3.Lerp (origPos, target, currentTime / time);
			currentTime += Time.deltaTime;
			targetIndicator.transform.position = spawn;
			yield return null;
		} while (currentTime < time);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
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
