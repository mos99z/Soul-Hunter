using UnityEngine;
using System.Collections;

public class Rock_Spike_Controller : MonoBehaviour {

	public float RecoveryCost = 0.0f;
	public float ImpactRate = 0.0f;
	private float TimePassed = 0.0f;
	public int MaxChildren = 0;
	private int Child = 1;

	void Start ()
	{
		GameObject Player = GameObject.FindGameObjectWithTag ("Player");
		transform.forward = Player.transform.forward;
		Player.GetComponent<Player_Caster_Controller>().SendMessage("SetRecoverTime", RecoveryCost, SendMessageOptions.RequireReceiver);
		GameObject.Find("GameBrain").BroadcastMessage("SpellCasted", SendMessageOptions.DontRequireReceiver);
	}
	
	void Update ()
	{
		if (MaxChildren <= Child)
		{
			Destroy(gameObject, 1.0f);
		}

		TimePassed += Time.deltaTime;
		if (TimePassed >= ImpactRate)
		{
			TimePassed = 0.0f;
			transform.GetChild (Child).gameObject.SetActive (true);
			Child++;
		}
	}
}