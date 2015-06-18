using UnityEngine;
using System.Collections;

public class Fog_Controller : MonoBehaviour 
{
	public GameObject mouseMarker;		// mouse marker from game brain
	public float duration = 2.0f;		// how long spell lasts
	public float recoveryTime = 1.5f;	// how long for spell to recharge

	// TODO: enable this when blind is implemented
//	public GameObject blind;	// debuff to apply
	void Start () 
	{
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		transform.position = mouseMarker.transform.position;
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);
	}

	void Update()
	{
		duration -= Time.deltaTime;
		if (duration <= 0.0f)
			Destroy (gameObject);
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") 
		{
			GameBrain.Instance.PlayerInFog = true;
			if(Fog_Event_Manager.PlayerEntered != null)
				Fog_Event_Manager.PlayerEntered();
		}
		if (col.tag == "Enemy")
		{
			// TODO: apply blind debuff
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player") 
		{
			GameBrain.Instance.PlayerInFog = false;
			if(Fog_Event_Manager.PlayerLeft != null)
				Fog_Event_Manager.PlayerLeft();
		}
	}
}
