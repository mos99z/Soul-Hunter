using UnityEngine;
using System.Collections;

public class Poision_Cloud_Controller : MonoBehaviour 
{
	public GameObject mouseMarker;		// mouse marker from game brain
	public float duration = 5.0f;		// how long spell lasts
	public float damage = 2.3f;			// how much damage it does
	public float recoveryTime = 2.0f;	// how long for spell to recharge
	public GameObject SpellEffect = null;
	public float GrowthRate = 2.0f;
	bool active = true;

	public LayerMask LOSBlockers;
	
	float timer = 0.0f;		// for ticking damage
	
	// TODO: enable this when blind is implemented
	//	public GameObject blind;	// debuff to apply

	void Start () 
	{
		duration += 2.0f * GameBrain.Instance.EarthLevel < GameBrain.Instance.ElectricLevel 
			? (GameBrain.Instance.EarthLevel < GameBrain.Instance.WaterLevel 
			   ? (float)GameBrain.Instance.EarthLevel : (GameBrain.Instance.WaterLevel < GameBrain.Instance.ElectricLevel 
			                                          ? (float)GameBrain.Instance.WaterLevel : (float)GameBrain.Instance.ElectricLevel)) 
				: GameBrain.Instance.ElectricLevel < GameBrain.Instance.WaterLevel 
				? (float)GameBrain.Instance.ElectricLevel : (float)GameBrain.Instance.WaterLevel;

		RaycastHit colliderCheck = new RaycastHit();
		Vector3 distance = (GameBrain.Instance.MouseMarker.transform.position - GameBrain.Instance.Player.transform.position);
		distance.y = 0.0f;
		Physics.Raycast (GameBrain.Instance.Player.transform.position + new Vector3 (0, 1.0f, 0), 
		                 distance.normalized,
		                 out colliderCheck,
		                 distance.magnitude,
		                 LOSBlockers);
		Vector3 startpos = Vector3.zero;
		if (colliderCheck.collider != null)
			startpos = colliderCheck.point - distance.normalized;
		else
			startpos = GameBrain.Instance.MouseMarker.transform.position;
		startpos.y = 0.0f;
		
		transform.position = startpos;

		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);
	}
	
	void Update()
	{
		if (active) {
			transform.localScale += new Vector3 (GrowthRate * Time.deltaTime, 0, GrowthRate * Time.deltaTime);
			SpellEffect.GetComponent<ParticleSystem> ().maxParticles = (int)(150 * transform.localScale.x);

			duration -= Time.deltaTime;
			if (duration <= 0.0f)
			{
				if (SpellEffect != null)
					SpellEffect.GetComponent<ParticleSystem> ().Stop ();
				else
					Debug.LogError ("Spell: " + transform.name.ToString () + " Parameter SpellEffect is NOT assigned.");
			
				Destroy (gameObject, 2.0f);
				active = false;
			}
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (active && other.tag == "Enemy")
		{
			RaycastHit colliderCheck = new RaycastHit();
			Vector3 distance = (other.transform.position - transform.position);
			distance.y = 0.0f;
			Physics.Raycast (transform.position + new Vector3 (0, 1.0f, 0), distance.normalized, out colliderCheck, distance.magnitude, LOSBlockers);

			if (colliderCheck.collider == null)
				other.transform.SendMessage("TakeDamage", damage);
			// TODO: apply blind debuff
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		if (active && other.tag == "Enemy")
		{
			RaycastHit colliderCheck = new RaycastHit();
			Vector3 distance = (other.transform.position - transform.position);
			distance.y = 0.0f;
			Physics.Raycast (transform.position + new Vector3 (0, 1.0f, 0), distance.normalized, out colliderCheck, distance.magnitude, LOSBlockers);
			
			if (colliderCheck.collider == null)
			{
				timer += Time.deltaTime;
				if (timer >= 0.4f)
				{
					timer = 0.0f;
					other.transform.SendMessage("TakeDamage", damage);
				}
			}
		}
	}
}
