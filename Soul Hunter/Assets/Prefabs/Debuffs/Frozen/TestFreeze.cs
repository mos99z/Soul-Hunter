using UnityEngine;
using System.Collections;

public class TestFreeze : MonoBehaviour 
{
	public GameObject frozenDebuff; 

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy" && other.GetComponent<Living_Obj>().entType == Living_Obj.EntityType.Minion)
		{
			GameObject frozen = Instantiate(frozenDebuff);
			frozen.transform.parent = other.transform;
			frozen.transform.localPosition = new Vector3(0.0f, 3.0f, 0.0f);
		}
	}
}
