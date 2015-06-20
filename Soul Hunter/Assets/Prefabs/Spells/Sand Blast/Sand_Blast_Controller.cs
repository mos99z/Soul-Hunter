using UnityEngine;
using System.Collections;

public class Sand_Blast_Controller : MonoBehaviour 
{
	public GameObject mouseMarker;		// mouse marker from game brain
	public float duration = 2.0f;		// how long for spell to last
	public float damage = 5.0f;			// how much damage to deal
	public float recoveryTime = 1.5f;	// how long to recover from spell
	GameObject forwardIndicator = null;

	void Start () 
	{
		transform.localScale *= 1.0f + (float)GameBrain.Instance.EarthLevel / (float)GameBrain.Instance.NumberOfLevels;

		forwardIndicator = GameBrain.Instance.Player.transform.FindChild ("Direction Indicator").gameObject;
		transform.forward = forwardIndicator.transform.forward;
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);
	}
	
	void Update () 
	{
		transform.position = GameBrain.Instance.Player.transform.position;
		transform.forward = forwardIndicator.transform.forward;
		duration -= Time.deltaTime;
		if (duration <= 0.0f)
			Destroy (gameObject);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			GameObject debuff = Instantiate(GameBrain.Instance.GetComponent<DebuffMasterList>().blinded);
			debuff.GetComponent<Blinded_Controller>().Duration = 5.0f;
			debuff.transform.parent = other.transform;
			debuff.transform.localPosition = Vector3.zero;
			other.transform.SendMessage("TakeDamage", damage);
		}
	}
}
