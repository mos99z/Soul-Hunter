using UnityEngine;
using System.Collections;

public class Stunned_Controller : MonoBehaviour {

	public Vector3 LockPosition = Vector3.zero;
	public MonoBehaviour[] Scripts;
	public float Duration = 0.5f;
	
	private float timeAlive = 0.0f;
	private bool checkOnce = true;

	void Start ()
	{
		LockPosition = transform.parent.transform.position;
	}
	
	void Update ()
	{
		
		if (checkOnce)
		{
			int children = transform.parent.childCount;
			for (int child = 0; child < children; child++)
			{
				if (transform.parent.GetChild (child).name == "Stunned(Clone)"
				    && transform.parent.GetChild (child).gameObject != gameObject)
				{
					Destroy (transform.parent.GetChild (child).gameObject);
				}
				else if (transform.parent.GetChild (child).name == "Wet(Clone)")
				{
					transform.parent.GetChild (child).GetComponent<Wet_Controller>().TimeLeft *= 0.5f;
				}
			}

			Scripts = transform.parent.GetComponents<MonoBehaviour>();
			for (int i = 0; i < Scripts.Length; i++)
			{
				Scripts[i].enabled = false;
			}
			transform.parent.GetComponent<NavMeshAgent>().enabled = false;
			transform.parent.GetComponent<Rigidbody>().velocity = Vector3.zero;
			transform.parent.transform.position = LockPosition;
			checkOnce = false;
		}
		
		timeAlive += Time.deltaTime;
		if (timeAlive >= Duration)
		{
			transform.parent.GetComponent<NavMeshAgent>().enabled = true;
			for (int i = 0; i < Scripts.Length; i++)
			{
				Scripts[i].enabled = true;
			}
			Destroy(gameObject);
		}
		
	}
}
