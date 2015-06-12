using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Living_Obj : MonoBehaviour
{
	// Negitave defence results in taking more damage overall.
	// Defence rating of 1 takes no damage and 0 takes normal damage.
	[Header ("Defence range between 1 and 0")]
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

	public enum EntityType {Player, Minion, Captain, Boss}
	public EntityType entType = EntityType.Minion;

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

		InfoDisaply = GameBrain.Instance.GetComponent<GameBrain>().DisplayText;
		if (InfoDisaply == null)
			Debug.LogError("Can NOT Find Display Text Prefab In Scene!");

		if (transform.GetComponentInChildren<SpriteRenderer> () != null)
			Image = transform.GetComponentInChildren<SpriteRenderer> ();

		Souls = GameBrain.Instance.Souls;
		if (Souls == null)
			Debug.Log("Can NOT Find Souls Prefab In Scene!");
		if (entType == EntityType.Player)
		{
			GameBrain.Instance.SendMessage("SetMaxHealth", MaxHealth);
			GameBrain.Instance.SendMessage("SetHealth", CurrHealth);
			GameBrain.Instance.SendMessage("SetLivesLeft", Lives);
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
					if (entType == EntityType.Player)
					{
						// TODO: Restart Level

					}
					else
						DropSoul ();
				}
			}
			else
				DropSoul ();
		}

		if (entType == EntityType.Captain)
		{
			GameBrain.Instance.HUDMaster.SendMessage("SetCaptainHealthDisplay", CurrHealth);
			if (CurrHealth == 0)
			{
				GameBrain.Instance.HUDMaster.SendMessage("DeactivateCaptBar", 0);
			}
		}

		if (entType == EntityType.Boss)
		{
			GameBrain.Instance.HUDMaster.SendMessage("SetBossHealthDisplay", CurrHealth);
			GameBrain.Instance.HUDMaster.SendMessage("SetBossName", gameObject.name);
			if (CurrHealth == 0)
			{
				GameBrain.Instance.HUDMaster.SendMessage("DeactivateBossBar", 0);
			}
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

		Debug.Log(ActualDamage + " Damage recieved by " + transform.name);
		
		if (GameBrain.Instance.Player.GetComponent<Player_Movement_Controller>().isCrippled &&
		    entType != EntityType.Player)
		{
			int children = GameBrain.Instance.Player.transform.childCount;
			for (int child = 0; child < children; child++)
			{
				if (GameBrain.Instance.Player.transform.GetChild(child).name.Contains("Crippled"))
				{
					float tempDamage = (float)ActualDamage;		// type conversion nonsense
					tempDamage *= GameBrain.Instance.Player.transform.GetChild(child).GetComponent<Crippled_Controller>().damageReductionModifier;
					ActualDamage = (int)tempDamage;
				}
			}
			Debug.Log(ActualDamage + " Damage recieved by " + transform.name + " due to cripple");
		}

		// check if frozen, set frozen duration to 0 to let debuff kill self if taking damage
		if (entType == EntityType.Minion)
		{
			int children = transform.childCount;
			for (int child = 0; child < children; ++child)
			{
				if (transform.GetChild(child).name.Contains("Frozen"))
					transform.GetChild(child).GetComponent<Frozen_Controller>().duration = 0.0f;
			}

		}

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

				if(entType == EntityType.Player)
					GameBrain.Instance.SendMessage("ModHealth", -ActualDamage);
				else
					GameBrain.Instance.GetComponent<GameBrain>().DamageDealt += ActualDamage;

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

		if (CurrHealth < 0)
		{
			CurrHealth = 0;
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
				GameBrain.Instance.SendMessage("SetHealth", MaxHealth);
				if (entType == EntityType.Player)
				{
					GameBrain.Instance.HUDMaster.SendMessage("DeactivateCaptBar");
					Application.LoadLevel(Application.loadedLevel);	// gamebrain assigns player location based on save when scene loads

					// below works to respawn player when dead, above should do so more eloquently

					//int count = GameBrain.Instance.RoomsCleared.Count;
					//if (count > 0)
					//{
					//	switch (GameBrain.Instance.RoomsCleared[count - 1])
					//	{
					//	case 1: GameBrain.Instance.Player.transform.position = Vector3.zero; break;
					//	case 2: GameBrain.Instance.Player.transform.position = new Vector3(45.0f, 0.0f, 85.0f); break;
					//	case 3: GameBrain.Instance.Player.transform.position = new Vector3(-40.0f, 0.0f, 85.0f); break;
					//	case 4: GameBrain.Instance.Player.transform.position = new Vector3(-50.0f, 0.0f, 35.0f); break;
					//	case 5: GameBrain.Instance.Player.transform.position = new Vector3(-20.0f, 0.0f, 140.0f); break;
					//	}
					//}
					//else
					//	GameBrain.Instance.Player.transform.position = Vector3.zero;
				}
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
		if (entType != EntityType.Player)
			GameBrain.Instance.SendMessage ("AddKill");
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

		// captain dropping soul clears room
		// TODO: check for entire rooms death for room cleared
		if (entType == EntityType.Captain)
		{
			if (transform.name.Contains("Mage"))
			{
				GameBrain.Instance.RoomsCleared.Add(GetComponent<Mage_Captain_Controller>().roomNumber);
				GameBrain.Instance.Save();
			}
			else if (transform.name.Contains("Binding"))
			{
				// TODO: get roomnumber
			}			
			else if (transform.name.Contains("Juggernaut"))
			{
				// TODO: get roomnumber
			}
		}

		if (entType != EntityType.Player)
			Destroy (gameObject);
		else
		{
// TODO: Player "Death"
			Application.LoadLevel("Tally Scene");
			GameBrain.Instance.SendMessage("SetLevel", -2);
		}
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
