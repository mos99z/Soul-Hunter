using UnityEngine;
using System.Collections;

public class Meteorite_Controller : MonoBehaviour {

	public float DamageFire = 25.1f;
	public float DamageEarth = 25.1f;
	public float FallSpeed = 0.75f;
	public float StartHeight = 30.0f;
	public float recoveryCost = 3.0f;	// spell cooldown
	public GameObject MainSpell = null;
	public SphereCollider MainCollider = null;
	public GameObject ImpactExplosion = null;

	public LayerMask LOSBlockers;

	private bool UpdatePos = true;
	// Use this for initialization
	void Start ()
	{
		FallSpeed += 0.05f * (float)GameBrain.Instance.WindLevel;
		ImpactExplosion.GetComponent<Damage_Message>().DebuffChance += 0.833333f * (float)GameBrain.Instance.FireLevel;
		transform.localScale *= 1.0f + (float)GameBrain.Instance.EarthLevel / (float)GameBrain.Instance.NumberOfLevels;

		RaycastHit colliderCheck = new RaycastHit();
		Vector3 distance = (GameBrain.Instance.MouseMarker.transform.position - GameBrain.Instance.Player.transform.position);
		distance.y = 0.0f;
		Physics.Raycast (GameBrain.Instance.Player.transform.position + new Vector3 (0, 1.0f, 0), 
		                 distance.normalized,
		                out colliderCheck,
		                 distance.magnitude,
		                LOSBlockers);
		Vector3 startpos = Vector3.zero;
		if (colliderCheck.collider != null)
			startpos = colliderCheck.point - distance.normalized + new Vector3 (0, StartHeight - 1.0f, 0);
		else
			startpos = GameBrain.Instance.MouseMarker.transform.position += new Vector3 (0, StartHeight, 0);

		transform.position = startpos;
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryCost);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void FixedUpdate ()
	{
		if (UpdatePos)
		{
			transform.position -= new Vector3(0,FallSpeed,0);
		}
	}

	void OnTriggerEnter(Collider _object)
	{
		if (_object.tag == "Enemy")
		{
			if (_object.transform.FindChild("Frozen"))
			{
				_object.transform.GetComponent<Living_Obj>().SendMessage("Die");
				return;
			}
			_object.SendMessage("TakeDamage", DamageFire);
			_object.SendMessage("TakeDamage", DamageEarth);
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
