using UnityEngine;
using System.Collections;

public class Explosion_Controller : MonoBehaviour 
{
	public float damage = 25.1f;		// how much damage the spell does
	public float duration = 1.0f;		// how long in seconds for the spell to last
	public float recoveryTime = 2.0f;	// how long in seconds for spell to cooldown
	public GameObject mouseMarker;		// mouse marker from gamebrain
	public float spawnHeight = 1.0f;	// modify the height of the object
	public ForceMode forceMode;			// used for which kind of force to use
	public float knockback = 300.0f;	// how much force to apply on knockback
	public float radius = 2.0f;			// how far to apply explosion force

	public GameObject burning;			// debuff to apply
	public float burnChance = 0.10f;	// chance that debuff will apply
	public float burnDuration = 5.0f;	// how long burn lasts
	public float burnTick = 1.0f;		// how often to deal burn
	public float burnDamage = 10.1f;	// how much damage to deal each tick

	public GameObject cripple;			// debuff to apply
	public float crippleChance = 0.15f;	// chance to apply debuff

	void Start () 
	{
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		Vector3 spawn = mouseMarker.transform.position;
		spawn.y = spawnHeight;
		transform.position = spawn;

		foreach (Collider other in Physics.OverlapSphere(transform.position, radius))
		{
			if (other.GetComponent<Rigidbody>() != null && other.tag == "Enemy")	// if there is a rigidbody in range, apply some force and debuffs
			{

				other.GetComponent<Rigidbody>().AddExplosionForce(knockback, transform.position, radius, 0.0f, forceMode);
				other.transform.SendMessage("TakeDamage", damage);

				float chance = Random.Range(0.0f,1.0f);
				if (chance <= burnChance)
				{
					GameObject burn = Instantiate(burning);
					burn.transform.parent = other.transform;
					burn.transform.localPosition = Vector3.zero;
					burn.GetComponent<Burning_Controller>().Damage = burnDamage;
					burn.GetComponent<Burning_Controller>().Duration = burnDuration;
					burn.GetComponent<Burning_Controller>().TickCycle = burnTick;
				}
				chance = Random.Range(0.0f,1.0f);
				if (chance <= crippleChance)
				{
					// TODO: make frozen debuff kill object
//					int children = other.transform.childCount;
//					for (int child = 0; child < children; child++)
//					{
//					if (transform.FindChild("Frozen(Clone)"))
//						{
//							other.GetComponent<Living_Obj>().CurrHealth = 0;
//							other.SendMessage("PulseCheck");
//							Debug.Log("cripple + frozen killed " + other.name);
//							return;
//						}
//					}

					GameObject cripp = Instantiate(cripple);
					cripp.transform.parent = other.transform;
					cripp.transform.localPosition = Vector3.zero;
				}
			}
		}

		if (burning == null)
			burning = GameBrain.Instance.GetComponent<DebuffMasterList>().burning;
		if (cripple == null)
			cripple = GameBrain.Instance.GetComponent<DebuffMasterList>().crippled;

		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);

		if (GameBrain.Instance.FireLevel > 0)
		{
			switch (GameBrain.Instance.FireLevel)
			{
			case 1: damage = damage*2.0f - 0.1f; break;
			case 2: damage = damage*3.0f - 0.2f; break;
			case 3: damage = damage*4.0f - 0.3f; break;
			}
		}
	}
	
	void Update () 
	{
		duration -= Time.deltaTime;
		if (duration <= 0.0f)
			Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			other.transform.SendMessage("TakeDamage", damage);
			
			float chance = Random.Range(0.0f,1.0f);
			if (chance <= burnChance)
			{
				GameObject burn = Instantiate(burning);
				burn.transform.parent = other.transform;
				burn.transform.localPosition = Vector3.zero;
				burn.GetComponent<Burning_Controller>().Damage = burnDamage;
				burn.GetComponent<Burning_Controller>().Duration = burnDuration;
				burn.GetComponent<Burning_Controller>().TickCycle = burnTick;
			}
			chance = Random.Range(0.0f,1.0f);
			if (chance <= crippleChance)
			{
//					int children = other.transform.childCount;
//					for (int child = 0; child < children; child++)
//					{
//					if (transform.FindChild("Frozen(Clone)"))
//						{
//							other.GetComponent<Living_Obj>().CurrHealth = 0;
//							other.SendMessage("PulseCheck");
//							Debug.Log("cripple + frozen killed " + other.name);
//							return;
//						}
//					}
				GameObject cripp = Instantiate(cripple);
				cripp.transform.parent = other.transform;
				cripp.transform.localPosition = Vector3.zero;
			}
		}
	}
}
