using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Living_Obj : MonoBehaviour
{
	// Negitave defence results in taking more damage overall.
	// Defence rating of 1 takes no damage and 0 takes normal damage.
	public float Defence = 0.0f;
	public Element ElementType = Element.None;

	public int MaxHealth = 1;
	public int CurrHealth = 1;
	public bool CanTakeDamage = true;

	public bool CanDie = true;
	public int Lives = 1;
	private bool IsAlive = true;

	public SoulType SoulValue = SoulType.None;

	// 3D text to dispaly: Crits, Resists, & Immunes.
	public GameObject InfoDisaply = null;
	void Start ()
	{
		if (CurrHealth <= 0)
		{
			Debug.LogError(transform.name + "'s health should not start at or below 0, " + transform.name + " was destroyed!");
			Destroy(gameObject);
			return;
		}

		if (Lives <= 0)
		{
			Debug.LogError(transform.name + "'s must start with atleast 1 Life, " + transform.name + " was destroyed!");
			Destroy(gameObject);
			return;
		}

		if (MaxHealth <= 0)
		{
			Debug.LogError(transform.name + "'s Max Health can not be " + MaxHealth.ToString() + " must be atleast 1, " + transform.name + "'s Max Health was set to 1!");
			MaxHealth = 1;
		}

		if (CurrHealth > MaxHealth)
		{
			Debug.LogError(transform.name + "'s Curr Health can not start above it's Max Health " + transform.name + "'s Curr Health was set to Max Health of " + MaxHealth.ToString());
			CurrHealth = MaxHealth;
		}
	}

	void Update()
	{
		if (!IsAlive)
		{
			if (transform.GetComponent<Animation> ())
				if (!transform.GetComponent<Animation> ().isPlaying)
					DropSoul();
			else
				DropSoul();
		}
	}

	void TakeDamage (float _damage)
	{
		// Get actual damage value from damage parameter.
		// Calculate damage reduction from defence before elements are accounted for.
		int ActualDamage = (int)((int)_damage * (1.0f - Defence));

		if (ElementType != Element.None)
		{
			if (CanTakeDamage)
			{
				Element ElementalDamage = (Element)((int)(_damage - ActualDamage) * 10);
				// If the obj's elemental type and the damage's elemental type are both not of type none.
				if (ElementalDamage != Element.None)
				{
					// Get elemental difference between obj's elemental type and damage type.
					int ElementalDifference = (int)ElementalDamage - (int)ElementType;
					// 1 being strong against
					if (ElementalDifference == 1)
					{
						ActualDamage = 0;
						// Display Immune - Red
						GameObject DisplayText = (GameObject)Instantiate(InfoDisaply, transform.position, transform.rotation);
						DisplayText.transform.position += new Vector3(0,2,0);
						DisplayText.GetComponent<TextMesh>().text = "IMMUNE";
						DisplayText.GetComponent<TextMesh>().color = new Color(1.0f, 0.0f, 0.0f);
						DisplayText.GetComponent<TextMesh>().characterSize = 5.0f;
					}
					// 0 being the same type
					else if (ElementalDifference == 0)
					{
						ActualDamage = ActualDamage >> 1;
						// Display Resist - White
						GameObject DisplayText = (GameObject)Instantiate(InfoDisaply, transform.position, transform.rotation);
						DisplayText.transform.position += new Vector3(0,2,0);
						DisplayText.GetComponent<TextMesh>().text = "RESIST";
						DisplayText.GetComponent<TextMesh>().color = new Color(1.0f, 1.0f, 1.0f);
						DisplayText.GetComponent<TextMesh>().characterSize = 4.0f;
					}
					// -1 being weak against
					else if (ElementalDifference == -1)
					{
						ActualDamage = ActualDamage << 1;
						// Display Crit - Yellow
						GameObject DisplayText = (GameObject)Instantiate(InfoDisaply, transform.position, transform.rotation);
						DisplayText.transform.position += new Vector3(0,2,0);
						DisplayText.GetComponent<TextMesh>().text = "IMMUNE";
						DisplayText.GetComponent<TextMesh>().color = new Color(1.0f, 1.0f, 0.0f);
						DisplayText.GetComponent<TextMesh>().characterSize = 6.0f;
					}
					// anything else has no effect on damage value
				}
			}
				
			// Take damage.
			CurrHealth -= ActualDamage;
				
			PulseCheck ();
		}
		// Display Invincible - Orange
		else
		{
			GameObject DisplayText = (GameObject)Instantiate(InfoDisaply, transform.position, transform.rotation);
			DisplayText.transform.position += new Vector3(0,2,0);
			DisplayText.GetComponent<TextMesh>().text = "INVINCIBLE";
			DisplayText.GetComponent<TextMesh>().color = new Color(1.0f, 0.5f, 0.0f);
			DisplayText.GetComponent<TextMesh>().characterSize = 8.0f;
		}
	}
	
	void PulseCheck()
	{
		if (CurrHealth <= 0 && CanDie)
		{
			CurrHealth = 0;
			Lives -= 1;
			if (Lives <= 0)
			{
				Die();
			}
		}
		// Display immortal - Blue
		else if (!CanDie)
		{
			GameObject DisplayText = (GameObject)Instantiate(InfoDisaply, transform.position, transform.rotation);
			DisplayText.transform.position += new Vector3(0,2,0);
			DisplayText.GetComponent<TextMesh>().text = "IMMORTAL";
			DisplayText.GetComponent<TextMesh>().color = new Color(0.0f, 0.0f, 1.0f);
			DisplayText.GetComponent<TextMesh>().characterSize = 8.0f;
		}
	}

	void Invincible(bool _On)
	{
		if (_On)
			CanTakeDamage = false;
		else
			CanTakeDamage = true;
	}

	void Immortal(bool _On)
	{
		if (_On)
			CanDie = false;
		else
			CanDie = true;
	}

	void Die()
	{
		IsAlive = false;
		if (transform.GetComponent<Animation> ())
			transform.GetComponent<Animation> ().Play ("Death");
	}

	void DropSoul()
	{

		Destroy (gameObject);
	}
}
