using UnityEngine;
using System.Collections;

public class Fire_Ball_Impact_Controller : MonoBehaviour {

	public float ImpactDamage = 10.1f;
	public float IgniteChance = 1.0f;
	public float BurningDOTDuration = 10.0f;
	public float BurningDOTTickCycle = 0.5f;
	public float BurningDOTTickDamage = 5.1f;
	public GameObject BurningDebuff = null;
	void Start ()
	{
		if (BurningDebuff == null)
		{
			if (IgniteChance > 1.0f)
				IgniteChance = 1.0f;
			else if (IgniteChance < 0.0f)
				IgniteChance = 0.0f;

			Debug.Log("To Reduce CPU Cycles assign the Debuff \"Burning\" to the Burning Debuff Parameter on prefab GameBrain/Spell Database/" + transform.parent.transform.parent.name + "/" + transform.parent.name + ":" + name);
			BurningDebuff = GameObject.Find("GameBrain/Debuffs/Burning");
		}
	}
	
	void Update () {
	
	}

	void OnTriggerEnter(Collider _object)
	{
		if (_object.tag == "Enemy")
		{
			_object.transform.SendMessage("TakeDamage", ImpactDamage);
			float roll = UnityEngine.Random.Range(0.0f, 1.0f);
			if (roll <= IgniteChance)
			{
				GameObject debuff = Instantiate(BurningDebuff);
				debuff.transform.parent = _object.transform;
				debuff.transform.localPosition = Vector3.zero;
				debuff.GetComponent<Burning_Controller>().Damage = BurningDOTTickDamage;
				debuff.GetComponent<Burning_Controller>().Duration = BurningDOTDuration;
				debuff.GetComponent<Burning_Controller>().TickCycle = BurningDOTTickCycle;
			}
		}
	}
}
