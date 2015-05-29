using UnityEngine;
using System.Collections;

public class DetectPlayer : MonoBehaviour
{
	public GameObject MainCam;
	
	void OnTriggerEnter(Collider _coll)
	{
		if (_coll.tag == "Player")
		{
			MainCam.GetComponent<Tutorial>().SetTask1(true);
			gameObject.SetActive(false);
		}
	}
}
