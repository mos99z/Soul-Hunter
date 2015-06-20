using UnityEngine;
using System.Collections;

public class Freeze_Controller : MonoBehaviour 
{
	public float waterDamage = 10.5f;	// how much water damage is done
	public float windDamage = 5.2f;		// how much wind damage is done
	public float duration = 1.5f;		// how long spell is active
	public float recoveryCost = 1.5f;	// how long for spell to cooldown

	public float spawnHeight = 2.5f;	// where to offset the spawn location on y-axis
	public GameObject mouseMarker;		// mouse marker from gamebrain
	public GameObject frozen;			// frozen debuff to apply
	public float chance = 0.5f;			// percent chance that frozen debuff will apply
	public float frozenDuration = 4.0f;	// how long the frozen debuff lasts

	void Start () 
	{
		chance += GameBrain.Instance.WindLevel < GameBrain.Instance.WaterLevel ? (float)GameBrain.Instance.WindLevel 
			: (float)GameBrain.Instance.WaterLevel / ((float)GameBrain.Instance.NumberOfLevels * 2.0f);

		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		Vector3 spawn = mouseMarker.transform.position;
		spawn.y = spawnHeight;
		transform.position = spawn;
		transform.Rotate (new Vector3 (1.0f, 0.0f, 0.0f), 90.0f);
		Vector3 childSpawn = transform.GetChild(0).transform.position;
		childSpawn.y -= 2.0f;
		transform.GetChild(0).transform.position = childSpawn;
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryCost);

		if (GameBrain.Instance.WindLevel > 0)
		{
			switch (GameBrain.Instance.WindLevel)
			{
			case 1: windDamage = windDamage*2.0f - 0.2f; break;
			case 2: windDamage = windDamage*3.0f - 0.4f; break;
			case 3: windDamage = windDamage*4.0f - 0.6f; break;
			}
		}
		if (GameBrain.Instance.WaterLevel > 0)
		{
			switch (GameBrain.Instance.WaterLevel)
			{
			case 1: waterDamage = waterDamage*2.0f - 0.5f; break;
			case 2: waterDamage = waterDamage*3.0f - 1.0f; break;
			case 3: waterDamage = waterDamage*4.0f - 1.5f; break;
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
			other.transform.SendMessage("TakeDamage", waterDamage);
			other.transform.SendMessage("TakeDamage", windDamage);

			float roll = Random.Range(0.0f, 1.0f);
			if (roll <= chance)
			{
				GameObject debuff = Instantiate(frozen);
				debuff.transform.parent = other.transform;
				debuff.transform.localPosition = new Vector3(0.0f, 3.0f, 0.0f);
				debuff.GetComponent<Frozen_Controller>().duration = frozenDuration;
			}
		}
	}
}
