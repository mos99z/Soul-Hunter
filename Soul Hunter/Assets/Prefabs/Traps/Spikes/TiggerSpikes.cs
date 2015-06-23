using UnityEngine;
using System.Collections;

public class TiggerSpikes : MonoBehaviour
{

	public GameObject[] Spikes;

	void Start()
	{
		for (int EachSpike = 0; EachSpike < Spikes.Length; EachSpike++)
		{
			Spikes[EachSpike].SetActive(false);
		}
	}

	void OnTriggerEnter(Collider _obj)
	{
		if (_obj.tag == "Player")
		{
			for (int EachSpike = 0; EachSpike < Spikes.Length; EachSpike++)
			{
				Spikes[EachSpike].SetActive(true);
			}
		}
	}

	void OnTriggerExit(Collider _obj)
	{
		if (_obj.tag == "Player")
		{
			for (int EachSpike = 0; EachSpike < Spikes.Length; EachSpike++)
			{
				Spikes[EachSpike].SendMessage("Reset");
				Spikes[EachSpike].SetActive(false);
			}
		}
	}
}
