using UnityEngine;
using System.Collections;

public class Shock_Discharge_Controller : MonoBehaviour {

	public GameObject Discharge = null;
	private float Damage = 50.4f;
	private float TickTime = 0.1f;
	private float Timer = 0.0f;
	private GameObject Player = null;

	void Start ()
	{
		
	}
	
	void Update ()
	{
		Timer += Time.deltaTime;
		if (Timer >= TickTime)
		{
			if (Player != null)
			{
				Player.SendMessage("TakeDamage", Damage);
			}
		}
	}

	void OnTriggerEnter(Collider _obj)
	{
		if (_obj.tag == "Player")
		{
			Player = _obj.gameObject;
			Discharge.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = 1;
		}
	}

	void OnTriggerExit(Collider _obj)
	{
		if (_obj.tag == "Player")
		{
			Player = null;
			Discharge.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = 0;
		}
	}
}
