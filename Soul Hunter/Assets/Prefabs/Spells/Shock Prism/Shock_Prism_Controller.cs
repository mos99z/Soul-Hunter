using UnityEngine;
using System.Collections;

public class Shock_Prism_Controller : MonoBehaviour 
{
	public float DamageWater = 5.5f;		// how much damage to deal per tick
	public float DamageLighting = 5.5f;		// how much damage to deal per tick
	public float TickRate = 0.5f;			// how much damage to deal per tick
	float TickTimer = 0.0f;					// how much damage to deal per tick
	public float recoveryTime = 1.5f;		// how long for spell to cooldwon
	float FlightDuration = 2.0f;			
	float DurationFinal = 3.0f;				// msut be larger than 0, else you will divide by zero....
	public GameObject Sparks = null;
	Vector3 origPos = Vector3.zero;			// where the spell starts
	Vector3 targetPos = Vector3.zero;		// where the spell ends
	bool StunEnabled = false;
	public LayerMask LOSBlockers;

	void Start () 
	{
		DurationFinal += 1.0f * (float)GameBrain.Instance.WaterLevel;
		FlightDuration -= 0.25f * (float)GameBrain.Instance.ElectricLevel;

		RaycastHit colliderCheck = new RaycastHit();
		Vector3 distance = (GameBrain.Instance.MouseMarker.transform.position - GameBrain.Instance.Player.transform.position);
		distance.y = 0.0f;
		Physics.Raycast (GameBrain.Instance.Player.transform.position + new Vector3 (0, 1.0f, 0), 
		                 distance.normalized,
		                 out colliderCheck,
		                 distance.magnitude + 2.5f,
		                 LOSBlockers);
		if (colliderCheck.collider != null)
			targetPos = colliderCheck.point - distance.normalized * 2.5f;
		else
			targetPos = GameBrain.Instance.MouseMarker.transform.position;
		targetPos.y = 1.5f;

		origPos = GameBrain.Instance.Player.transform.position;
		origPos.y = 1.5f;
		transform.position = origPos;
		
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);

		StartCoroutine("MoveSpell", FlightDuration);
	}
	
	IEnumerator MoveSpell(float time)
	{
		float currentTime = 0.0f;
		do{
			transform.position = Vector3.Lerp(origPos, targetPos, currentTime/time);
			currentTime += Time.deltaTime;
			yield return null;
		} while (currentTime < time);
		StartCoroutine("Expand", 0.5f);
	}
	
	IEnumerator Expand(float time)
	{
		StunEnabled = true;
		float currentTime = 0.0f;
		do{
			transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * Mathf.Lerp(1.0f, 5.0f, currentTime/time);
			if (Sparks != null)
				Sparks.GetComponent<ParticleSystem> ().emissionRate = (int)(200.0f * transform.localScale.x);
			currentTime += Time.deltaTime;
			yield return null;
		} while (currentTime < time);

		currentTime = 0.0f;
		do{
			DurationFinal -= Time.deltaTime;
			yield return null;
		} while (0.0f < DurationFinal);
		if (Sparks != null)
			Sparks.GetComponent<ParticleSystem> ().Stop ();
		Destroy (gameObject, 0.2f);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			other.transform.SendMessage("TakeDamage", DamageWater);
			other.transform.SendMessage("TakeDamage", DamageLighting);
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Enemy") {

			TickTimer += Time.deltaTime;
			if (TickTimer >= TickRate)
			{
				TickTimer = 0.0f;
				other.transform.SendMessage("TakeDamage", DamageWater);
				other.transform.SendMessage("TakeDamage", DamageLighting);

				if (StunEnabled && other.transform.FindChild("Stunned(Clone)") == null)
				{
					GameObject stunDebuff = Instantiate(GameBrain.Instance.GetComponent<DebuffMasterList>().stunned);
					stunDebuff.transform.parent = other.transform;
					stunDebuff.transform.localPosition = Vector3.zero;
					stunDebuff.GetComponent<Stunned_Controller>().Duration = DurationFinal;
				}
			}
		}
	}

}
