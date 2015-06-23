using UnityEngine;
using System.Collections;

public class Magma_Controller : MonoBehaviour 
{
	public float damage = 5.1f;			// how much damage is done
	public float tickRate = 0.2f;		// how long to wait between dealing damage
	public float recoveryTime = 2.0f;	// how long for spell to recharge
	public float growSpeed = 2.0f;		// how long for spell to grow
	public float scaleValue = 4.0f;		// how much the magma circle will grow
	public GameObject mouseMarker;		// make this the mouse marker object in gamebrain
	
	public GameObject burning;			// debuff to apply
	public float burnChance = 0.9f;		// percent chance that debuff will apply
	public float burningDuration = 5.0f;// how long debuff lasts
	public float burningTick = 1.0f;	// how long to wait between dealing damage
	public float burningDamage = 5.1f;	// how much damage to deal each tick
	public LayerMask LOSBlockers;
	
	Vector3 origScale;		// reference starting point
	Vector3 finalScale;		// keep for proportional reference
	float timer;			// for dealing extra ticks of damage

	void Start () 
	{
		burningDuration += 1.0f * (float)GameBrain.Instance.FireLevel;
		recoveryTime -= 0.25f * (float)GameBrain.Instance.EarthLevel;
		
		timer = 0.0f;

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
		startpos.y = 0.1f;
		
		transform.position = startpos;

		transform.Rotate(Vector3.right, 90.0f);		// so it spawns flat on the ground
		origScale = transform.localScale;
		finalScale = new Vector3 (origScale.x * scaleValue, origScale.y * scaleValue, origScale.z * scaleValue);

		if (burning == null)
			burning = GameBrain.Instance.GetComponent<DebuffMasterList>().burning;

		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);

		StartCoroutine("ScaleOverTime", growSpeed);
	}
	
	IEnumerator ScaleOverTime(float time)
	{
		float currentTime = 0.0f;
		
		do
		{
			transform.localScale = Vector3.Lerp(origScale, finalScale, currentTime / time);
			currentTime += Time.deltaTime;
			yield return null;
		} while (currentTime <= time);
		
		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy" || other.tag == "Player")
		{
			RaycastHit colliderCheck = new RaycastHit();
			Vector3 distance = (other.transform.position - transform.position);
			distance.y = 0.0f;
			Physics.Raycast (transform.position + new Vector3 (0, 1.0f, 0), 
			                 distance.normalized,
			                 out colliderCheck,
			                 distance.magnitude,
			                 LOSBlockers);
			if (colliderCheck.collider == null)
			{
				other.transform.SendMessage("TakeDamage", damage);
				
				float chance = Random.Range(0.0f,1.0f);
				if (chance <= burnChance)
				{
					GameObject debuff = Instantiate(burning);
					debuff.transform.parent = other.transform;
					debuff.transform.localPosition = Vector3.zero;
					debuff.GetComponent<Burning_Controller>().Damage = burningDamage;
					debuff.GetComponent<Burning_Controller>().Duration = burningDuration;
					debuff.GetComponent<Burning_Controller>().TickCycle = burningTick;
				}
			}
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Enemy" || other.tag == "Player")
		{
			RaycastHit colliderCheck = new RaycastHit ();
			Vector3 distance = (other.transform.position - transform.position);
			distance.y = 0.0f;
			Physics.Raycast (transform.position + new Vector3 (0, 1.0f, 0), 
			                 distance.normalized,
			                 out colliderCheck,
			                 distance.magnitude,
			                 LOSBlockers);
			if (colliderCheck.collider == null)
			{
				timer += Time.deltaTime;
				if (timer >= tickRate)
				{
					timer = 0.0f;
					other.transform.SendMessage ("TakeDamage", damage);
				}
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Enemy" || other.tag == "Player")
		{
			timer = 0.0f;
		}
	}
}
