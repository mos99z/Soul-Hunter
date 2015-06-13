using UnityEngine;
using System.Collections;

public class HazeMaker : MonoBehaviour
{
	public GameObject Haze;

	// Use this for initialization
	void Start ()
	{
		for (int i = -150; i <= 150; i += 5)
		{
			for (int j = -50; j <= 400; j += 5)
			{
				Vector3 newPosition = new Vector3(i, 50, j);
				Instantiate(Haze, newPosition, Haze.transform.rotation);
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
