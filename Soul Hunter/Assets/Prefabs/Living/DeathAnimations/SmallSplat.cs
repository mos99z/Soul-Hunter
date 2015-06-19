using UnityEngine;
using System.Collections;

public class SmallSplat : MonoBehaviour
{
	public float stainTime = 5;

	// Use this for initialization
	void Start ()
	{
		if (stainTime < 5)
		{
			stainTime = 5;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		stainTime -= Time.deltaTime;
		if (stainTime <= 0)
		{
			Destroy(this.gameObject);
		}
	}
}
