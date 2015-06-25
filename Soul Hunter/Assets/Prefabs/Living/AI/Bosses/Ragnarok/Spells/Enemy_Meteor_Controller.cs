using UnityEngine;
using System.Collections;

public class Enemy_Meteor_Controller : MonoBehaviour 
{
	public float Damage = 500.0f;
	public float FallSpeed = 0.75f;
	public float StartHeight = 30.0f;
	public GameObject MainSpell = null;
	public SphereCollider MainCollider = null;
	public GameObject ImpactExplosion = null;

	private bool UpdatePos = true;

	void Start ()
	{
		transform.position += new Vector3(0,StartHeight,0);
	}
	
	void FixedUpdate ()
	{
		if (UpdatePos)
			transform.position -= new Vector3(0,FallSpeed,0);
	}
	
	void OnTriggerEnter(Collider _object)
	{
		if (_object.tag == "Player")
		{
			if (_object.transform.FindChild("Frozen"))
			{
				_object.transform.GetComponent<Living_Obj>().SendMessage("Die");
				return;
			}
			_object.SendMessage("TakeDamage", Damage);
			GameObject cripp = Instantiate(GameBrain.Instance.GetComponent<DebuffMasterList>().crippled);
			cripp.transform.parent = _object.transform;
			cripp.transform.localPosition = Vector3.zero;
		}
		if (_object.tag == "VOID")
		{
			MainSpell.SetActive(false);
			MainCollider.enabled = false;
			ImpactExplosion.SetActive(true);
			Destroy(gameObject, 0.5f);
			UpdatePos = false;
		}
	}
}
