using UnityEngine;
using System.Collections;

public class Wet_Controller : MonoBehaviour {

	public float Duration = 10.0f;
	public float TimeLeft = 10.0f;
	private bool checkOnce = true;
	void Start () {
		TimeLeft = Duration;
	}
	
	void Update ()
	{
		if (checkOnce && transform.parent != null)
		{
			int children = transform.parent.childCount;
			bool die = false;
			for (int child = 0; child < children; child++)
			{
				if (transform.parent.GetChild(child).name == "Burning(Clone)")
				{
					Destroy(transform.parent.GetChild(child).gameObject);
					die = true;
				}
				else if (transform.parent.GetChild(child).name == "Wet(Clone)" && transform.parent.GetChild(child).gameObject != gameObject)
				{
					Destroy(transform.parent.GetChild(child).gameObject);
				}
			}
			checkOnce = false;
			if (die)
			{
				Destroy(gameObject);
				return;
			}
			TimeLeft = Duration;
		}

		TimeLeft -= Time.deltaTime;
		if (TimeLeft <= 0.0f)
			Destroy (gameObject);
	}
}
