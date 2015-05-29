using UnityEngine;
using System.Collections;

public class DetectMouse : MonoBehaviour
{
	public GameObject MainCam;

	void OnTriggerEnter(Collider _coll)
	{
		if (_coll.tag == "MouseMarker")
		{
			MainCam.GetComponent<Tutorial>().SetTask2(true);
			gameObject.SetActive(false);
		}
	}
}
