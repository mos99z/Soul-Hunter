using UnityEngine;
using System.Collections;

public class Crippled_Controller : MonoBehaviour 
{
	public float duration = 5.0f;					// how long in seconds for the debuff to last
	public float slowSpeedModifier = 0.5f;			// fraction to reduce objects speed by
	public float damageReductionModifier = 0.5f;	// fraction to reduce enemy attacks

	float origSpeed;			// remember objects original speed
	float origDamage;			// remember original damage value
	float origSpell, origAOE;	// for mage captain
	bool check;					// check if a clone exists already

	void Start () 
	{
		check = true;
	}
	
	void Update () 
	{
		if (check && transform.parent != null)
		{
			if (transform.parent.GetComponent<Living_Obj>().entType == Living_Obj.EntityType.Boss)
			{
				Destroy(gameObject);
				return;
			}
			int children = transform.parent.childCount;
			for (int child = 0; child < children; child++)
			{
				if (transform.parent.GetChild(child).name.Contains("Crippled") && transform.parent.GetChild(child) != this.transform)
				{
					transform.parent.GetChild(child).GetComponent<Crippled_Controller>().duration = duration; // reset timer on original, and kill self
					Destroy(gameObject);
					return;
				}
			}
			check = false;
			if (transform.parent.tag == "Player")
			{
				origSpeed = transform.parent.GetComponent<Player_Movement_Controller>().Speed;
				transform.parent.GetComponent<Player_Movement_Controller>().Speed *= slowSpeedModifier;
				transform.parent.GetComponent<Player_Movement_Controller>().isCrippled = true;
			}
			else if (transform.parent.tag == "Enemy")
			{
				origSpeed = transform.parent.GetComponent<NavMeshAgent>().speed;
				transform.parent.GetComponent<NavMeshAgent>().speed *= slowSpeedModifier;

				string name = transform.parent.name;
				if (name.Contains("Melee"))
				{
					origDamage = transform.parent.GetComponent<Melee_Minion_Controller>().Damage;
					transform.parent.GetComponent<Melee_Minion_Controller>().Damage *= damageReductionModifier;
				}
				else if (name.Contains("Ranged"))
				{
					origDamage = transform.parent.GetComponent<Ranged_Minion_Controller>().missleDamage;
					transform.parent.GetComponent<Ranged_Minion_Controller>().missleDamage *= damageReductionModifier;
				}
				else if (name.Contains("Kamikaze"))
				{
					origDamage = transform.parent.GetComponent<Kamikaze_Minion_Controller>().ExplosionDamage;
					transform.parent.GetComponent<Kamikaze_Minion_Controller>().ExplosionDamage *= damageReductionModifier;
				}
				else if (name.Contains("Binding"))
				{
					// TODO: add functionality when binding captain is made
				}
				else if (name.Contains("Juggernaut"))
				{
					// TODO: add functionality when Juggernaut captain is made
				}
				else if (name.Contains("Mage"))
				{
					origDamage = transform.parent.GetComponent<Mage_Captain_Controller>().missileDamage;
					origAOE = transform.parent.GetComponent<Mage_Captain_Controller>().aoeDamage;
					origSpell = transform.parent.GetComponent<Mage_Captain_Controller>().spellDamage;

					transform.parent.GetComponent<Mage_Captain_Controller>().missileDamage *= damageReductionModifier;
					transform.parent.GetComponent<Mage_Captain_Controller>().aoeDamage *= damageReductionModifier;
					transform.parent.GetComponent<Mage_Captain_Controller>().spellDamage *= damageReductionModifier;
				}
			}
		}
		duration -= Time.deltaTime;
		if (duration <= 0.0f)
		{
			if (transform.parent.tag == "Player")
			{
				transform.parent.GetComponent<Player_Movement_Controller>().Speed = origSpeed;
				transform.parent.GetComponent<Player_Movement_Controller>().isCrippled = false;
			}
			else if (transform.parent.tag == "Enemy")
			{
				transform.parent.GetComponent<NavMeshAgent>().speed = origSpeed;
				string name = transform.parent.name;
				if (name.Contains("Melee"))
					transform.parent.GetComponent<Melee_Minion_Controller>().Damage = origDamage;
				else if (name.Contains("Ranged"))
					transform.parent.GetComponent<Ranged_Minion_Controller>().missleDamage = origDamage;
				else if (name.Contains("Kamikaze"))
					transform.parent.GetComponent<Kamikaze_Minion_Controller>().ExplosionDamage = origDamage;
				else if (name.Contains("Binding"))
				{
					// TODO: add functionality when binding captain is made
				}
				else if (name.Contains("Juggernaut"))
				{
					// TODO: add functionality when Juggernaut captain is made
				}
				else if (name.Contains("Mage"))
				{
					transform.parent.GetComponent<Mage_Captain_Controller>().missileDamage = origDamage;
					transform.parent.GetComponent<Mage_Captain_Controller>().aoeDamage = origAOE;
					transform.parent.GetComponent<Mage_Captain_Controller>().spellDamage = origSpell;
				}
			}
			Destroy(gameObject);
		}
	}
}
