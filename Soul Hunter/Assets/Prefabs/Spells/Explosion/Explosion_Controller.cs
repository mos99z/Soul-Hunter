using UnityEngine;
using System.Collections;

public class Explosion_Controller : MonoBehaviour 
{
	public float damage = 25.1f;		// how much damage the spell does
	public float duration = 1.0f;		// how long in seconds for the spell to last
	public float recoveryTime = 2.0f;	// how long in seconds for spell to cooldown
	public float knockback = 1000.0f;	// how much force to apply on knockback

	public float burnDamage = 10.1f;	// how much damage to deal each tick
	public float burnChance = 0.10f;	// chance that debuff will apply
	public float burnDuration = 5.0f;	// how long burn lasts
	public float burnTick = 1.0f;		// how often to deal burn

	public float crippleChance = 0.15f;	// chance to apply debuff
	public float crippleDuration = 5.0f; // Duration of Cripple

	public GameObject SpellEffect = null;
	public LayerMask LOSBlockers;

	void Start () 
	{
		burnChance += (float)GameBrain.Instance.FireLevel / ((float)GameBrain.Instance.NumberOfLevels * 10.0f);
		crippleChance += (float)GameBrain.Instance.EarthLevel / ((float)GameBrain.Instance.NumberOfLevels * 10.0f);
		SpellEffect.GetComponent<ParticleSystem> ().startSize *= 1.0f + (float)GameBrain.Instance.EarthLevel / (float)GameBrain.Instance.NumberOfLevels;
		transform.localScale *= 1.0f + (float)GameBrain.Instance.EarthLevel / (float)GameBrain.Instance.NumberOfLevels;

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
		startpos.y = 1.25f;

		transform.position = startpos;
		SpellEffect.GetComponent<ParticleSystem> ().Play ();
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);
	}
	
	void Update () 
	{
		duration -= Time.deltaTime;
		if (duration <= 0.0f)
			Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")	// if there is a rigidbody in range, apply some force and debuffs
		{
			other.GetComponent<Rigidbody>().AddExplosionForce(knockback, transform.position, transform.GetComponent<SphereCollider>().radius);
			
			float chance = Random.Range(0.0f, 1.0f);
			if (chance <= burnChance)
			{
				GameObject burn = Instantiate(GameBrain.Instance.GetComponent<DebuffMasterList>().burning);
				burn.transform.parent = other.transform;
				burn.transform.localPosition = Vector3.zero;
				burn.GetComponent<Burning_Controller>().Damage = burnDamage;
				burn.GetComponent<Burning_Controller>().Duration = burnDuration;
				burn.GetComponent<Burning_Controller>().TickCycle = burnTick;
			}

			chance = Random.Range(0.0f, 1.0f);
			if (chance <= crippleChance)
			{
				if (other.transform.FindChild("Frozen(Clone)"))
				{
					other.transform.GetComponent<Living_Obj>().SendMessage("Die");
					return;
				}
				
				GameObject cripp = Instantiate(GameBrain.Instance.GetComponent<DebuffMasterList>().crippled);
				cripp.transform.parent = other.transform;
				cripp.transform.localPosition = Vector3.zero;
				cripp.transform.GetComponent<Crippled_Controller>().duration = crippleDuration;
			}

			other.transform.SendMessage("TakeDamage", damage);
		}
	}
}