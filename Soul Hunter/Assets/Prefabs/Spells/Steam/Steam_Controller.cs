using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Steam_Controller : MonoBehaviour 
{

	public GameObject mouseMarker;		// mouse marker from game brain
	public float duration = 2.0f;		// how long for spell to last
	public float TickTimer = 0.25f;		// Damage Tick intervals
	float Timer = 0.0f;		// Time passed
	public float MinDamage = 5.1f;		// how much damage to deal on first hit.
	public float MaxDamage = 20.1f;		// how much damage to deal on max hits.
	public float PushBackForce = 100.0f;
	int NumDamageSteps = 4;				// Number of damage steps from min to max
	float damageStep = 0;
	public float recoveryTime = 1.5f;	// how long to recover from spell
	List<List<GameObject>> DamageSteps = new List<List<GameObject>>();
		
	void Start ()
	{
		for (int ListSize = 0; ListSize < NumDamageSteps; ListSize++)
		{
			DamageSteps.Add(new List<GameObject>());
		}
		damageStep = (MaxDamage - MinDamage) / (float)(NumDamageSteps - 1);
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		transform.LookAt(mouseMarker.transform.position);
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);
	}
	
	void Update () 
	{
		transform.position = GameBrain.Instance.Player.transform.position;
		transform.LookAt(mouseMarker.transform.position);

		Timer += Time.deltaTime;
		if (Timer >= TickTimer)
		{
			Timer = 0.0f;
			for (int numLists = 0; numLists < DamageSteps.Count; numLists++)
			{
				for (int objInList = 0; objInList < DamageSteps[numLists].Count; objInList++)
				{
					if (DamageSteps[numLists][objInList] == null)
					{
						DamageSteps[numLists].RemoveAt(objInList);
						objInList--;
						continue;
					}

					DamageSteps[numLists][objInList].GetComponent<Rigidbody>().AddForce((DamageSteps[numLists][objInList].transform.position - transform.position).normalized * PushBackForce);
					DamageSteps[numLists][objInList].SendMessage("TakeDamage", MinDamage + numLists * damageStep );
				}
			}

			for (int numLists = DamageSteps.Count - 1; numLists >= 0; numLists--)
			{
				for (int objInList = 0; objInList < DamageSteps[numLists].Count; objInList++)
				{
					if (DamageSteps[numLists][objInList] == null)
					{
						DamageSteps[numLists].RemoveAt(objInList);
						objInList--;
						continue;
					}

					if (numLists < DamageSteps.Count - 1)
					{
						DamageSteps[numLists + 1].Add(DamageSteps[numLists][objInList]);
						DamageSteps[numLists].RemoveAt(objInList);
						objInList--;
					}
				}
			}
		}

		duration -= Time.deltaTime;
		if (duration <= 0.0f)
			Destroy (gameObject);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			for (int numLists = 0; numLists < DamageSteps.Count; numLists++)
			{
				if (DamageSteps[numLists].Contains(other.transform.gameObject))
					return;
			}
			DamageSteps[0].Add(other.transform.gameObject);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Enemy")
		{
			for (int numLists = 0; numLists < DamageSteps.Count; numLists++)
			{
				if (DamageSteps[numLists].Contains(other.transform.gameObject))
				{
					DamageSteps[numLists].Remove(other.transform.gameObject);
					return;
				}
			}
		}
	}
}
