using UnityEngine;
using System.Collections;

public class EnemyShock : MonoBehaviour
{
	private float zapTicker;
	private float damageTicker;
	private bool startDamage;
	public GameObject zapPart;
	public SphereCollider zapCol;
	
	// Use this for initialization
	void Start ()
	{
		zapPart.SetActive(true);
		zapPart.GetComponent<ParticleSystem>().startSize = 0.5f;
		zapPart.GetComponent<ParticleSystem>().startColor = new Color32(255,255, 128, 255);
		zapPart.GetComponent<ParticleSystem>().maxParticles = 10;
		zapCol.enabled = false;
		startDamage = false;
	}
	
	void Update ()
	{
		zapTicker += Time.deltaTime;
		if (zapTicker > 2)
		{
			zapPart.GetComponent<ParticleSystem>().startSize = 1;
			zapPart.GetComponent<ParticleSystem>().startColor = new Color32(255,255, 0, 255);
			zapPart.GetComponent<ParticleSystem>().maxParticles = 100;
			startDamage = true;
		}
		if (startDamage)
		{
			damageTicker += Time.deltaTime;
			if (damageTicker > 0.05f)
			{
				zapCol.enabled = true;
				damageTicker = 0;
			}
			if (zapTicker > 5)
			{
				Destroy(this.gameObject);
			}
		}
	}
	
	private void OnTriggerEnter(Collider col)
	{
		float damageMod = 1.0f;
		if (col.tag == "Player")
		{
			if (col.transform.FindChild ("Wet(Clone)"))
			{
				col.SendMessage("TakeDamage", 10);
			}
			else
			{
				col.SendMessage("TakeDamage", 5);

			}
			zapCol.enabled = false;
		}
	}
}
