using UnityEngine;
using System.Collections;

public class MeteorExplosion : MonoBehaviour {

	void OnTriggerEnter(Collider _obj)
	{
		if (_obj.tag.Contains("Enemy"))
		{
//			_obj.GetComponent<NavMeshAgent>().enabled = false;
			_obj.transform.GetComponent<Rigidbody>().AddExplosionForce(25.0f, gameObject.transform.position, gameObject.transform.GetComponent<SphereCollider>().radius);
		}
	}
}
