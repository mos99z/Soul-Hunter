using UnityEngine;
using System.Collections;

public class Pit_Controller : MonoBehaviour {
	
	void OnTriggerEnter(Collider _obj)
	{
		if (_obj.tag == "Player" || _obj.tag == "Enemy")
		{
			_obj.GetComponent<Living_Obj>().CurrHealth = 0;
			_obj.GetComponent<Living_Obj>().SendMessage("PulseCheck");
		}
	}
}
