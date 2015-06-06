using UnityEngine;
using System.Collections;

public class Frozen_Controller : MonoBehaviour 
{
	public float duration =  10.0f;		// how long in seconds for the spell to last

	bool check;			// for deleting duplicates
	void Start () 
	{
		check = true;
	}
	
	void Update () 
	{
		if (check && transform.parent != null)
		{
			check = false;
			int children = transform.parent.childCount;
			for (int child = 0; child < children; child++)
			{
				if (transform.parent.GetChild(child).name.Contains("Frozen") && transform.parent.GetChild(child).gameObject != gameObject)
				{
					transform.parent.GetChild(child).GetComponent<Crippled_Controller>().duration = duration; // reset timer on original and kill self
					Destroy(gameObject);
					return;
				}
			}
			if (transform.parent.tag == "Enemy")
			{
				if (transform.parent.GetComponent<Living_Obj>().entType != Living_Obj.EntityType.Minion)
				{
					Destroy (gameObject);
					return;
				}
				else
				{
					transform.parent.GetComponent<NavMeshAgent>().enabled = false;
					string name = transform.parent.name;
					if (name.Contains("Kamikaze"))
						transform.parent.GetComponent<Kamikaze_Minion_Controller>().isFrozen = true;
					else if (name.Contains("Melee"))
						transform.parent.GetComponent<Melee_Minion_Controller>().isFrozen = true;
					else if (name.Contains("Ranged"))
						transform.parent.GetComponent<Ranged_Minion_Controller>().isFrozen = true;
				}
			}

		}

		duration -= Time.deltaTime;
		if (duration <= 0.0f)
		{
			transform.parent.GetComponent<NavMeshAgent>().enabled = true;
			
			string name = transform.parent.name;
			if (name.Contains("Kamikaze"))
				transform.parent.GetComponent<Kamikaze_Minion_Controller>().isFrozen = false;
			else if (name.Contains("Melee"))
				transform.parent.GetComponent<Melee_Minion_Controller>().isFrozen = false;
			else if (name.Contains("Ranged"))
				transform.parent.GetComponent<Ranged_Minion_Controller>().isFrozen = false;
			Destroy(gameObject);
		}
	}

}
