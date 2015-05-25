using UnityEngine;
using System.Collections;

public class HazeAction : MonoBehaviour
{
	void OnTriggerEnter(Collider _coll)
	{
		if (_coll.tag == "PlayerMarker")
		{
			Destroy(gameObject);
		}
	}
}
