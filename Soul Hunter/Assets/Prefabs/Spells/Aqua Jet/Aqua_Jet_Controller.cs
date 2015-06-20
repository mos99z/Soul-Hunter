using UnityEngine;
using System.Collections;

public class Aqua_Jet_Controller : MonoBehaviour 
{
	public GameObject mouseMarker;		// mouse marker from game brain
	public float startHeight = 1.5f;	// where to overwrite the y-axis
	public float duration = 1.0f;		// how long in seconds for spell to travel
	public float damage = 5.5f;			// how much damage to deal
	public float recoveryTime = 1.5f;	// how long for spell to cooldwon
	public int maxHits = 5;				// how many times spelll can hit before dieing

	public AudioSource AquaJetTravel;

	int hits = 0;		// keep track of the hits
	Vector3 origPos;	// where the spell starts
	Vector3 targetPos;	// where the spell ends

	void Start () 
	{
		Vector3 spawn = GameBrain.Instance.Player.transform.position;
		spawn.y = startHeight;
		transform.position = origPos = spawn;
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		spawn = mouseMarker.transform.position;
		spawn.y = startHeight;
		targetPos = spawn;

		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);
		
		if (GameBrain.Instance.WaterLevel > 0)
		{
			switch (GameBrain.Instance.WaterLevel)
			{
			case 1: damage = damage*2.0f - 0.5f; break;
			case 2: damage = damage*3.0f - 1.0f; break;
			case 3: damage = damage*4.0f - 1.5f; break;
			}
		}
		StartCoroutine("MoveSpell", duration);
	}

	IEnumerator MoveSpell(float time)
	{
		float currentTime = 0.0f;
		do{
			transform.position = Vector3.Lerp(origPos, targetPos, currentTime/time);
			currentTime += Time.deltaTime;
			yield return null;
		} while (currentTime < time);
		Destroy(gameObject, 0.5f);		// safety timer to kill self
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			other.transform.SendMessage("TakeDamage", damage);
			hits++;
			if (hits >= maxHits)
				Destroy(gameObject);
		}
	}
}
