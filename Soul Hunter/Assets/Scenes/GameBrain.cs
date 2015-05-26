using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public enum Debuff
{ 
	NONE,
	Slowed, 
	Stunned,
	Wet,
	Burning,
	Crippled,
	Frozen
}

public enum Element
{
	None,
	Fire,
	Wind,
	Earth,
	Lightning,
	Water
}

public enum SoulType
{
	None,
	White,
	Green,
	Blue,
	Purple,
	Red,
	Black
}
[Serializable]
public class GameInfo
{
	public int PlayerMaxHealth;
	public int PlayerCurrHealth;
	public int PlayerLivesLeft;
	public int SoulCount;
	public GameObject player;

	public int FireLevel;
	public int WindLevel;
	public int EarthLevel;
	public int ElectricLevel;
	public int WaterLevel;

	public bool[] RoomsCleared;

	// Level 0 is tutorial
	public int CurrentLevel;


	//Tally Specific Info
	public int NumEnemiesKilled;
	public int DamageTaken;
	public int TotalSoulCount;
	public int DeathCount;
	public double GameTime;
	public int NumCastedSpells;
}

public class GameBrain : MonoBehaviour {

	public int PlayerMaxHealth = 1000;
	public int PlayerCurrHealth = 1000;
	public int PlayerLivesLeft = 3;
	public int SoulCount = 0;
	// Level 0 is tutorial
	public int CurrentLevel = 1;

	public GameInfo gameInfo;

	public int FireLevel = 0;
	public int WindLevel = 0;
	public int EarthLevel = 0;
	public int ElectricLevel = 0;
	public int WaterLevel = 0;

	//Tally Specific Info
	public int NumEnemiesKilled = 0;
	public int DamageTaken = 0;
	public int TotalSoulCount = 0;
	public int DeathCount = 0;
	public double GameTime = 0.0;
	public int NumCastedSpells = 0;
	public GameObject SpellDatabase = null;
	public GameObject Souls = null;
	public GameObject HUD = null;
	public GameObject DisplayText = null;

	// Use this for initialization
	void Start ()
	{
		//The Spell Database it's self should not be active.
		//Only in scene for ease of development.
		SpellDatabase.SetActive (false);
		Souls.SetActive (false);
		HUD.SetActive (true);
		DontDestroyOnLoad (gameObject);
		LoadPlayerData ();
		//gameInfo.RoomsCleared [3] = false;
	}
	
	// Update is called once per frame
	void Update () {
		GameTime += Time.deltaTime;
	}

	void ModMaxHealth(int _value)
	{
		PlayerMaxHealth += _value;
		VerifyMaxHealth ();
	}
	void SetMaxHealth(int _NewMaxHealth)
	{
		PlayerMaxHealth = _NewMaxHealth;
		VerifyMaxHealth ();
	}

	void VerifyMaxHealth()
	{
		if (PlayerMaxHealth <= 0)
			PlayerMaxHealth = 1;

		if (PlayerCurrHealth > PlayerMaxHealth)
			PlayerCurrHealth = PlayerMaxHealth;
		HUD.GetComponent<StatsDisplay> ().SetMaxHealthDisplay(PlayerMaxHealth);
	}

	void ModHealth(int _value)
	{
		if (_value < 0)
			DamageTaken -= _value;

		PlayerCurrHealth += _value;
		CheckHealth ();
	}
	void SetHealth(int _NewHealth)
	{
		if (_NewHealth < PlayerCurrHealth)
			DamageTaken += PlayerCurrHealth - _NewHealth;

		PlayerCurrHealth = _NewHealth;
		CheckHealth ();
	}

	void CheckHealth()
	{
		if (PlayerCurrHealth > PlayerMaxHealth)
			PlayerCurrHealth = PlayerMaxHealth;
		else if (PlayerCurrHealth <= 0)
		{
			DamageTaken += PlayerCurrHealth;
			PlayerCurrHealth = 0;
			ModLivesLeft( -1);

			if (PlayerLivesLeft <= 0)
				GameOver();
			else
				RespawnPlayer();
		}
		HUD.GetComponent<StatsDisplay> ().SetHealthDisplay(PlayerCurrHealth);
	}

	void ModLivesLeft(int _value)
	{
		PlayerLivesLeft += _value;

		if (_value < 0)
			DeathCount -= _value;

		CheckLives ();
	}
	void SetLivesLeft(int _NewLives)
	{
		if (_NewLives < PlayerLivesLeft)
			DeathCount += PlayerLivesLeft - _NewLives;

		PlayerLivesLeft = _NewLives;

		CheckLives ();
	}

	void CheckLives()
	{
		if (PlayerLivesLeft < 0)
		{
			DeathCount += PlayerLivesLeft;
			PlayerLivesLeft = 0;
		}
		HUD.GetComponent<StatsDisplay> ().SetLivesDisplay((uint)PlayerLivesLeft);
	}

	void RespawnPlayer()
	{
		PlayerCurrHealth = PlayerMaxHealth;
	}

	void GameOver()
	{

	}

	void ModSouls(int _value)
	{
		SoulCount += _value;
		HUD.GetComponent<StatsDisplay> ().SetSoulsDisplay((uint)SoulCount);
		if (_value > 0)
			TotalSoulCount += _value;
	}

	void SetSouls(int _NewSouls)
	{
		SoulCount = _NewSouls;
		HUD.GetComponent<StatsDisplay> ().SetSoulsDisplay((uint)SoulCount);
	}

	//Tally Specific Info
	void AddKill()
	{
		++NumEnemiesKilled;
	}

	void SpellCasted(string _SpellName)
	{
		++NumCastedSpells;
		// In the works
		//SpellDatabase.transform.FindChild (_SpellName).GetComponent<GUIText>().text.;
	}

	void LoadPlayerData()
	{
		if (File.Exists (Application.persistentDataPath + "/PlayerInfo.dat")) 
		{

		}
	}
}
