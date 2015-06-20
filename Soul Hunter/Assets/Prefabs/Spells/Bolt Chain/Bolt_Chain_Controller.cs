using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bolt_Chain_Controller : MonoBehaviour 
{
	public float duration = 1.0f;		// how long for spell to last
	float timer = 0.0f;
	public float damage = 20.4f;			// how much damage to deal
	public float recoveryTime = 1.5f;	// how long to recover from spell
	public float JumpDistance = 2.0f;
	public int NumberOfJumps = 7;
	public LayerMask Hitables;
	public GameObject ChainEffect = null;
	Vector3 ForwardFace = Vector3.zero;
	bool DamageOnce = true;
	List<GameObject> HitTargets = new List<GameObject>();
	List<GameObject> Sparks = new List<GameObject>();

	
	void Start () 
	{
		NumberOfJumps += GameBrain.Instance.ElectricLevel;
		Vector3 mouseLocation = GameBrain.Instance.MouseMarker.transform.position;
		float smallestLength = float.MaxValue;
		int startingTarget = -1;
		Collider[] PossibleStarts = Physics.OverlapSphere (mouseLocation, 0.5f, Hitables);
		for (int target = 0; target < PossibleStarts.Length; target++)
		{
			float tempDistance = (PossibleStarts[target].transform.position - mouseLocation).magnitude;
			if (tempDistance < smallestLength)
			{
				startingTarget = target;
				smallestLength = tempDistance;
			}
		}
		if (startingTarget == -1)
		{
			Debug.Log("No Target Within Mouse Range Found To Start Bolt Chain Spell!");
			Destroy(gameObject);
			return;
		}
		// Add Player as first Target
		HitTargets.Add (GameBrain.Instance.Player.gameObject);
		// Add first real target
		HitTargets.Add (PossibleStarts [startingTarget].gameObject);

		// From next target in list check for another target
		for (;;)
		{
			if (HitTargets.Count - 1 < NumberOfJumps)
			{
				Collider[] Potentials = Physics.OverlapSphere (HitTargets[HitTargets.Count - 1].transform.position, JumpDistance, Hitables);
				float smallestDist = float.MaxValue;
				int NextTarget = -1;
				for (int target = 0; target < Potentials.Length; target++)
				{
					if (Potentials[target].gameObject == null)
						continue;
					if (HitTargets.Contains(Potentials[target].gameObject))
						continue;
					if (HitTargets[HitTargets.Count - 1] == null)
					{
						HitTargets.RemoveAt(HitTargets.Count - 1);
						target--;
						if (HitTargets.Count < 2)
						{
							Destroy(gameObject);
							return;
						}
						continue;
					}
					
					float tempDistance = (Potentials[target].transform.position - HitTargets[HitTargets.Count - 1].transform.position).magnitude;
					if (tempDistance < smallestDist)
					{
						NextTarget = target;
						smallestDist = tempDistance;
					}
				}
				if (NextTarget == -1)
				{
					Debug.Log("No Next Found Within Targets Range To Continue Bolt Chain Spell!");
					break;
				}
				else
					HitTargets.Add(Potentials[NextTarget].gameObject);
			}
			else
				break;
		}
		// Reached maxed jumps, or no new target close enough: Do damage and Spell Effect.
		for (int target = 1; target < HitTargets.Count; target++)
		{
			GameObject moarSparks = Instantiate(ChainEffect);
			moarSparks.transform.position -= new Vector3(0, 2.0f, 0);
			Sparks.Add(moarSparks);
		}

		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);
	}

	void Update()
	{
		timer += Time.deltaTime;
		if (DamageOnce && timer >= duration * 0.5f)
		{
			DamageOnce = false;

			for (int target = 0; target < HitTargets.Count; target++)
			{
				if (HitTargets [target] != null && target != 0)
				{
					HitTargets [target].SendMessage("Aggro");
					float damageMod = 1.0f;
					if (HitTargets [target].transform.FindChild ("Wet(Clone)"))
					{
						GameObject stun = Instantiate (GameBrain.Instance.GetComponent<DebuffMasterList>().stunned);
						stun.transform.parent = HitTargets [target].transform;
						stun.transform.localPosition = new Vector3 (0, 0, 0);
						stun.GetComponent<Stunned_Controller> ().Duration = 3.0f;
						damageMod = 3.0f;
					}
					HitTargets [target].SendMessage ("TakeDamage", (float)((int)damage * damageMod) + 0.4f);
				}
			}
		} else if (timer >= duration)
		{
			for (int spellEffect = 0; spellEffect < Sparks.Count; spellEffect++)
			{
				Destroy(Sparks[spellEffect]);
			}
			Destroy (gameObject);
		}
	}

	void FixedUpdate()
	{
		for (int target = 0; target < HitTargets.Count; target++)
		{
			if (HitTargets [target] == null)
				return;
			if (target != HitTargets.Count - 1)
			{
				if (HitTargets [target + 1] == null)
					return;
				ForwardFace = (HitTargets [target + 1].transform.position - HitTargets [target].transform.position);
				float distance = ForwardFace.magnitude;
				ForwardFace.Normalize ();
				Sparks [target].transform.forward = ForwardFace;
				Sparks [target].transform.position = HitTargets [target].transform.position + new Vector3 (0, 0.2f, 0);
				
				Vector3 newScale = Sparks [target].transform.localScale;
				newScale.z = distance * 0.25f;
				Sparks [target].transform.localScale = newScale;
				Sparks [target].transform.position += ForwardFace * distance * 0.5f;
			}
		}
	}
}