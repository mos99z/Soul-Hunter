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
	private GameObject Souls = null;

	// 3D text to dispaly: Crits, Resists, & Immunes.
	private GameObject InfoDisaply = null;

	private SpriteRenderer Image = null;
	private float FlashSpeed = 0.1f;
	private float FlashTimer = 0.0f;
	private float FlashCoolDown = 0.0f;

	public bool isPlayer = false;
	private GameObject GameBrain = null;
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


		InfoDisaply = GameObject.Find ("GameBrain").GetComponent<GameBrain>().DisplayText;
		if (InfoDisaply == null)
			Debug.LogError("Can NOT Find Display Text Prefab In Scene!");

		if (transform.GetComponentInChildren<SpriteRenderer> () != null)
			Image = transform.GetComponentInChildren<SpriteRenderer> ();

		GameBrain = GameObject.Find ("GameBrain");
		Souls = GameBrain.transform.FindChild("Souls").gameObject;
		if (Souls == null)
			Debug.Log("Can NOT Find Souls Prefab In Scene!");
		if (isPlayer)
		{
			GameBrain.SendMessage("SetMaxHealth", MaxHealth);
			GameBrain.SendMessage("SetHealth", CurrHealth);
			GameBrain.SendMessage("SetLivesLeft", Lives);
		}
	}

	void Update()
	{
		if (!IsAlive)
		{
			if (transform.GetComponentInChildren<Animation> () != null)
			{
				if (!transform.GetComponentInChildren<Animation> ().isPlaying)
				{
					DropSoul ();
				}
			}
			else
				DropSoul ();
		}

		if (Image != null && FlashTimer > 0.0f)
		{
			FlashTimer -= Time.deltaTime;
			if (FlashTimer <= 0.0f)
			{
				Color flash = transform.GetComponentInChildren<SpriteRenderer>().color;
				flash.a = 1.0f;
				transform.GetComponentInChildren<SpriteRenderer>().color = flash;
			}
		}

		if (FlashCoolDown > 0.0f)
			FlashCoolDown -= Time.deltaTime;
	}

	void TakeDamage (float _damage)
	{
		// Get actual damage value from damage parameter.
		// Calculate damage reduction from defence before elements are accounted for.
		int rawDamage = (int)_damage;
		int ActualDamage = (int)(((float)rawDamage) * (1.0f - Defence));

		if (CanTakeDamage)
		{
			if (ElementType != Element.None)
			{
				Element ElementalDamage = (Element)Mathf.RoundToInt((_damage - rawDamage) * 10.0f);
				// If the obj's elemental type and the damage's elemental type are both not of type none.
				if (ElementalDamage != Element.None)
				{
					// Get elemental difference between obj's elemental type and damage type.
					int ElementalDifference = (int)ElementalDamage - (int)ElementType;
					// 1 being strong against
					if (ElementalDifference == 1 || ElementalDifference == -4)
					{
						ActualDamage = 0;
						// Display Immune - Red
						DisplayTextInfo("IMMUNE", new Color(1.0f, 0.0f, 0.0f), 5.0f);
					}
					// 0 being the same type
					else if (ElementalDifference == 0)
					{
						ActualDamage = ActualDamage >> 1;
						// Display Resist - White
						DisplayTextInfo("RESIST", new Color(1.0f, 1.0f, 1.0f), 4.0f);
					}
					// -1 being weak against
					else if (ElementalDifference == -1 || ElementalDifference == 4)
					{
						ActualDamage = ActualDamage << 1;
						// Display Crit - Yellow
						DisplayTextInfo("CRITICAL", new Color(1.0f, 1.0f, 0.0f), 6.0f);
					}
					// anything else has no effect on damage value
				}
			}
				
			// Take damage.
			if (ActualDamage > 0)
			{
				CurrHealth -= ActualDamage;

				if(isPlayer)
					GameBrain.SendMessage("ModHealth", -ActualDamage);

				if (Image != null && FlashCoolDown <= 0.0f)
				{
					Color flash = Image.color;
					flash.a = 0.0f;
					Image.color = flash;
					FlashTimer = FlashSpeed;
					FlashCoolDown = 0.25f;
				}
				PulseCheck ();
			}
				
		}
		// Display Invincible - Orange
		else
		{
			DisplayTextInfo("INVINCIBLE", new Color(1.0f, 0.5f, 0.0f), 8.0f);
		}
	}
	
	void PulseCheck()
	{
		if (CurrHealth <= 0 && CanDie)
		{
			Lives -= 1;
			if (Lives <= 0)
				Die();
			else
			{
				CurrHealth = MaxHealth;
				GameBrain.SendMessage("SetHealth", MaxHealth);
			}
		}
		// Display immortal - Blue
		else if (!CanDie)
		{
			DisplayTextInfo("IMMORTAL", new Color(0.0f, 0.0f, 1.0f), 8.0f);
			CurrHealth = 1;
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
		if (!isPlayer)
			GameBrain.SendMessage ("AddKill");
		if (transform.GetComponentInChildren<Animation> () != null)
			transform.GetComponentInChildren<Animation> ().Play ("Death");
	}

	void DropSoul()
	{
		if (SoulValue != SoulType.None)
		{
			GameObject dropedSoul = Instantiate<GameObject> (Souls.transform.FindChild (System.Enum.GetName (typeof(SoulType), SoulValue)).gameObject);
			Vector3 spawnPosition = transform.position;
			spawnPosition.y = 0.5f;
			dropedSoul.transform.position = spawnPosition;
		}

		Destroy (gameObject);
	}

	void DisplayTextInfo(string _text, Color _color, float _fontSize)
	{
		GameObject DisplayText = (GameObject)Instantiate(InfoDisaply, transform.position, transform.rotation);
		DisplayText.transform.position += new Vector3(0,2,0);
		DisplayText.GetComponent<TextMesh>().text = _text;
		DisplayText.GetComponent<TextMesh>().color = _color;
		DisplayText.GetComponent<TextMesh>().characterSize = _fontSize;
		DisplayText.SetActive(true);
	}
}
