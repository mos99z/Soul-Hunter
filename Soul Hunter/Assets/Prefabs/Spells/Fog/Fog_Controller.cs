using UnityEngine;
using System.Collections;

public class Fog_Controller : MonoBehaviour 
{
	public GameObject mouseMarker;		// mouse marker from game brain
	public float duration = 2.0f;		// how long spell lasts
	public float recoveryTime = 1.5f;	// how long for spell to recharge
	public GameObject Blinded;

	// TODO: enable this when blind is implemented
//	public GameObject blind;	// debuff to apply
	void Start () 
	{
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		transform.position = mouseMarker.transform.position;
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);

		if (Blinded == null) 
			Blinded = GameBrain.Instance.GetComponent<DebuffMasterList>().blinded;
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
			GameBrain.Instance.SendMessage("");
		}
		if (col.tag == "Enemy")
		{
			// TODO: apply blind debuff
			GameObject blind = Instantiate (Blinded);
			blind.transform.parent = col.transform;
			blind.transform.localPosition = new Vector3 (0, -col.transform.position.y, 0);
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player") 
		{
			GameBrain.Instance.PlayerInFog = false;
			GameBrain.Instance.SendMessage("");
		}

		if (col.tag == "Enemy") 
		{
			Destroy(col.transform.FindChild("Blinded(Clone)"));
		}
	}
}
