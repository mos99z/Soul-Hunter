using UnityEngine;
using System.Collections;

public class Rock_Spike_Controller : MonoBehaviour {

	public float RecoveryCost = 0.0f;
	public float ImpactRate = 0.0f;
	private float TimePassed = 0.0f;
	public int MaxChildren = 0;
	public int Child = 0;
	public AudioSource[] RockSpikeSound;

	void Start ()
	{
		ImpactRate -= 0.025f * (float)GameBrain.Instance.EarthLevel;

		Vector3 lookAt = GameObject.FindGameObjectWithTag ("MouseMarker").transform.position;
		lookAt.y = transform.position.y;
		transform.LookAt (lookAt);
		GameObject.FindGameObjectWithTag ("Player").GetComponent<Player_Caster_Controller>().SendMessage("SetRecoverTime", RecoveryCost, SendMessageOptions.RequireReceiver);
	}
	
	void Update ()
	{
		TimePassed += Time.deltaTime;
		if (TimePassed >= ImpactRate && MaxChildren > Child) {
			TimePassed = 0.0f;
			transform.GetChild (Child).gameObject.SetActive (true);
			Child++;
			if (MaxChildren <= Child)
				Destroy (gameObject, 1.0f);
			
			for (int i = 0; i < RockSpikeSound.Length; i++)
			{
				RockSpikeSound[i].Play();
			}
		}
		else if (MaxChildren <= Child)
		{
			for (int i = 0; i < RockSpikeSound.Length; i++)
			{
				RockSpikeSound[i].Stop();
			}
			transform.position -= new Vector3(0, 10.0f * Time.deltaTime, 0);
		}
	}
}