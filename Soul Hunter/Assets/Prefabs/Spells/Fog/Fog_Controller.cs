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
		duration += GameBrain.Instance.WindLevel;
		transform.localScale *= 1.0f + GameBrain.Instance.FireLevel < GameBrain.Instance.WaterLevel ? (float)GameBrain.Instance.FireLevel 
			: (float)GameBrain.Instance.WaterLevel / (float)GameBrain.Instance.NumberOfLevels;

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
		{
			Destroy (gameObject);
			GameBrain.Instance.PlayerInFog = false;
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") 
		{
			GameBrain.Instance.PlayerInFog = true;
			GameBrain.Instance.SendMessage("PlayerEnteredFog");
		}
		if (col.tag == "Enemy")
		{
			if(col.name.Contains("Juggernaut"))
			{
				if (col.gameObject.GetComponent<Juggernaut_Captain_Controller>().isCharging == true)
				{
					return;
				}
			}
			// TODO: apply blind debuff
			GameObject blind = Instantiate (Blinded);
			blind.GetComponent<Blinded_Controller>().Duration = duration;
			blind.transform.parent = col.transform;
			blind.transform.localPosition = new Vector3 (0, -col.transform.position.y, 0);
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player") 
		{
			GameBrain.Instance.PlayerInFog = false;
			GameBrain.Instance.SendMessage("PlayerLeftFog");
		}

		if (col.tag == "Enemy") 
		{
			Destroy(col.transform.FindChild("Blinded(Clone)").gameObject);
		}
	}
}
