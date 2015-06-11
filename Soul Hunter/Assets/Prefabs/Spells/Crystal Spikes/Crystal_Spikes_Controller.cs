using UnityEngine;
using System.Collections;

public class Crystal_Spikes_Controller : MonoBehaviour 
{
	// set these as the crystals in spell heirarchy
	public GameObject[] crystals = new GameObject[3];		// crystals to be used

	public GameObject mouseMarker;		// mouse marker from gamebrain
	public float spawnHeight = -3.0f;	// where the first crystal starts
	public float speed = 1.0f;			// how fast a crystal moves
	public float recoveryCost = 2.0f;	// how long for spell to cooldown

	public float duration = 1.0f;		// how long after final spike spawns to destroy object
	public float damage = 5.0f;			// how much damage to deal each tick
	public float tickTime = 0.25f;		// how often to tick damage
	public GameObject slow;				// slowed debuff

	Vector3 endPoint;			// used for saving where to move the crystals to
	float timer = 0.0f;			// used for applying tick damage

	void Start () 
	{
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		Vector3 spawn = endPoint = mouseMarker.transform.position;
		transform.position = endPoint;
		spawn.y = spawnHeight;
		for (int i = 0; i < crystals.Length; ++i)
			crystals[i].transform.position = spawn;

		if (slow == null)
			slow = GameBrain.Instance.GetComponent<DebuffMasterList>().slowed;
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryCost);
		StartCoroutine("MoveCrystal", speed);
	}
	
	IEnumerator MoveCrystal(float time)
	{
		for (int i = 0; i < crystals.Length; ++i)
		{
			float currentTime = 0.0f;
			do{
				crystals[i].transform.position = Vector3.Lerp(crystals[i].transform.position, endPoint, currentTime / time);
				currentTime += Time.deltaTime;
				yield return null;
			} while ( currentTime < time);
		}
		Destroy(gameObject, duration);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			GameObject debuff = Instantiate(slow);
			debuff.transform.parent = other.transform;
			debuff.transform.localPosition = Vector3.zero;

			other.transform.SendMessage("TakeDamage", damage);
		}
	}
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Enemy")
		{
			timer += Time.deltaTime;
			if (timer >= tickTime)
			{
				timer = 0.0f;
				other.transform.SendMessage("TakeDamage", damage);
			}
		}
	}
}
