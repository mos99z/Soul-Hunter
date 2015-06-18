using UnityEngine;
using System.Collections;

public class Burning_Controller : MonoBehaviour {

	public float Damage = 10.1f;
	public float TickCycle = 0.5f;
	private float Ticktimmer = 0.0f;
	public float Duration = 10.0f;
	private float DurationTimer = 0.0f;
	private bool checkOnce = true;

	void Start ()
	{

	}

	void Update ()
	{
		if (checkOnce && transform.parent != null)
		{
			int children = transform.parent.childCount;
			bool die = false;
			for (int child = 0; child < children; child++)
			{
				if (transform.parent.GetChild(child).name == "Wet(Clone)")
				{
					Destroy(transform.parent.GetChild(child).gameObject);
					die = true;
				}
				else if (transform.parent.GetChild(child).name == "Burning(Clone)" && transform.parent.GetChild(child).gameObject != gameObject)
					Destroy(transform.parent.GetChild(child).gameObject);
			}
			checkOnce = false;
			if (die)
			{
				Destroy(gameObject);
				return;
			}
		}
		
		Ticktimmer += Time.deltaTime;
		if (Ticktimmer >= TickCycle)
		{
			Ticktimmer = 0.0f;
			transform.parent.SendMessage("TakeDamage", Damage, SendMessageOptions.DontRequireReceiver);
		}
		DurationTimer += Time.deltaTime;

		if (DurationTimer >= Duration)
			Destroy (gameObject);
	}
}
