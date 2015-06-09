using UnityEngine;
using System.Collections;

public class Gravity_Well_Controller : MonoBehaviour 
{
	public float damage = 5.0f;			// how much damage spell does
	public float duration = 1.0f;		// how long for spell to last
	public float recoveryTime = 0.75f;	// how long for player to recharge
	public GameObject slowed;			// slow debuff
	public GameObject mouseMarker;		// mouse marker object inside of gamebrain

	void Start () 
	{
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		transform.position = mouseMarker.transform.position;
		if (slowed == null)
			slowed = GameBrain.Instance.GetComponent<DebuffMasterList>().slowed;
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);
	}
	
	void Update () 
	{
		transform.Rotate(Vector3.up, 300.0f*Time.deltaTime);	// give spinny effect
		duration -= Time.deltaTime;
		if (duration <= 0.0f)
			Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			other.transform.SendMessage("TakeDamage", damage);

			GameObject debuff = Instantiate(slowed);
			debuff.transform.parent = other.transform;
			debuff.transform.localPosition = Vector3.zero;
		}
	}
}
