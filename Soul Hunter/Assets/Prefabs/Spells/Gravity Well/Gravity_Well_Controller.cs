using UnityEngine;
using System.Collections;

public class Gravity_Well_Controller : MonoBehaviour 
{
	public float duration = 5.0f;		// how long for spell to last
	public float recoveryTime = 1.0f;	// how long for player to recharge
	public GameObject SpellEffect = null;
	bool active = true;
	public LayerMask LOSBlockers;
	
	void Start () 
	{
		duration += GameBrain.Instance.ElectricLevel * 0.5f;

		RaycastHit colliderCheck = new RaycastHit();
		Vector3 distance = (GameBrain.Instance.MouseMarker.transform.position - GameBrain.Instance.Player.transform.position);
		distance.y = 0.0f;
		Physics.Raycast (GameBrain.Instance.Player.transform.position + new Vector3 (0, 1.0f, 0), 
		                 distance.normalized,
		                 out colliderCheck,
		                 distance.magnitude + transform.GetComponent<SphereCollider>().radius * transform.localScale.x,
		                 LOSBlockers);
		Vector3 startpos = Vector3.zero;
		if (colliderCheck.collider != null)
			startpos = colliderCheck.point - distance.normalized * transform.GetComponent<SphereCollider>().radius * transform.localScale.x;
		else
			startpos = GameBrain.Instance.MouseMarker.transform.position;
		startpos.y = 0.0f;
		
		transform.position = startpos;

		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);
	}
	
	void Update () 
	{
		transform.Rotate(Vector3.up, 100.0f*Time.deltaTime);	// give spinny effect
		duration -= Time.deltaTime;
		if (active && duration - 0.8f <= 0.0f)
		{
			SpellEffect.GetComponent<ParticleSystem>().Stop();
			Destroy(gameObject, 0.8f);
			active = false;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			if (other.GetComponent<Living_Obj>().entType == Living_Obj.EntityType.Minion)
				other.GetComponent<NavMeshAgent>().velocity = Vector3.zero;
			else if (other.GetComponent<Living_Obj>().entType == Living_Obj.EntityType.Captain)
				other.GetComponent<NavMeshAgent>().velocity *= 0.33f;
			else if (other.GetComponent<Living_Obj>().entType == Living_Obj.EntityType.Boss)
				other.GetComponent<NavMeshAgent>().velocity *= 0.66f;
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Enemy")
		{
			GameObject debuff = Instantiate(GameBrain.Instance.GetComponent<DebuffMasterList>().slowed);
			debuff.transform.parent = other.transform;
			debuff.transform.localPosition = Vector3.zero;
			debuff.GetComponent<Slowed_Controller>().duration = 0.25f;
			
			switch (other.GetComponent<Living_Obj>().entType)
			{
			case Living_Obj.EntityType.Minion:
				debuff.GetComponent<Slowed_Controller>().slowSpeedModifier = 0.0f;
				break;
			case Living_Obj.EntityType.Captain:
				debuff.GetComponent<Slowed_Controller>().slowSpeedModifier = 0.33f;
				break;
			case Living_Obj.EntityType.Boss:
				debuff.GetComponent<Slowed_Controller>().slowSpeedModifier = 0.66f;
				break;

			default:
				Destroy(debuff);
				break;
			}
		}
	}
}
