using UnityEngine;
using System.Collections;

public class Crystal_Spikes_Controller : MonoBehaviour 
{
	// set these as the crystals in spell heirarchy
	public GameObject[] crystals = new GameObject[3];		// crystals to be used

	public GameObject mouseMarker;		// mouse marker from gamebrain
	public float spawnHeight = -3.0f;	// where the first crystal starts
	public float speed = 1.0f;			// how fast a crystal moves
	public GameObject Crystals = null;	// Group of crystals to move
	public float recoveryCost = 2.0f;	// how long for spell to cooldown

	public float duration = 1.0f;		// how long after final spike spawns to destroy object
	public float DamageEarth = 1.3f;			// how much damage to deal each tick
	public float DamageLightning = 1.4f;			// how much damage to deal each tick
	public float tickTime = 0.25f;		// how often to tick damage
	public GameObject slow;				// slowed debuff

	Vector3 endPoint;					// used for saving where to move the crystals to
	float timer = 0.0f;					// used for applying tick damage

	public LayerMask LOSBlockers;

	void Start () 
	{
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;

		RaycastHit colliderCheck = new RaycastHit();
		Vector3 distance = (GameBrain.Instance.MouseMarker.transform.position - GameBrain.Instance.Player.transform.position);
		distance.y = 0.0f;
		Physics.Raycast (GameBrain.Instance.Player.transform.position + new Vector3 (0, 1.0f, 0), 
		                 distance.normalized,
		                 out colliderCheck,
		                 distance.magnitude + transform.GetComponent<SphereCollider>().radius * transform.localScale.x,
		                 LOSBlockers);
		Vector3 startpos = Vector3.zero;
		if (colliderCheck.collider != null)
			startpos = colliderCheck.point - distance.normalized * transform.GetComponent<SphereCollider>().radius * transform.localScale.x;
		else
			startpos = GameBrain.Instance.MouseMarker.transform.position;
		startpos.y = 0.0f;


		Vector3 spawn = endPoint = startpos;
		transform.position = endPoint;
		spawn.y = spawnHeight;

		Crystals.transform.position = spawn;

		if (slow == null)
			slow = GameBrain.Instance.GetComponent<DebuffMasterList>().slowed;
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryCost);
		StartCoroutine("MoveCrystal", speed);
	}
	
	IEnumerator MoveCrystal(float time)
	{
			float currentTime = 0.0f;
			do{
				Crystals.transform.position = Vector3.Lerp(Crystals.transform.position, endPoint, currentTime / time);
				currentTime += Time.deltaTime;
				yield return null;
			} while ( currentTime < time);
		Destroy(gameObject, duration);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			GameObject debuff = Instantiate(slow);
			debuff.transform.parent = other.transform;
			debuff.transform.localPosition = Vector3.zero;
			debuff.GetComponent<Slowed_Controller>().duration = 0.25f;
			debuff.GetComponent<Slowed_Controller>().slowSpeedModifier = 0.5f;

			other.transform.SendMessage("TakeDamage", DamageEarth);
			other.transform.SendMessage("TakeDamage", DamageLightning);
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
				GameObject debuff = Instantiate(slow);
				debuff.transform.parent = other.transform;
				debuff.transform.localPosition = Vector3.zero;
				debuff.GetComponent<Slowed_Controller>().duration = 0.25f;
				debuff.GetComponent<Slowed_Controller>().slowSpeedModifier = 0.25f;

				other.transform.SendMessage("TakeDamage", DamageEarth);
				other.transform.SendMessage("TakeDamage", DamageLightning);
			}
		}
	}
}
