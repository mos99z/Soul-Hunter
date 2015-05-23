using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

public class GameBrain : MonoBehaviour {

	public int PlayerMaxHealth = 1000;
	public int PlayerCurrHealth = 1000;
	public int PlayerLivesLeft = 3;
	public int SoulCount = 0;
	// Level 0 is tutorial
	public int CurrentLevel = 1;


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
	}
	
	// Update is called once per frame
	void Update () {
		GameTime += Time.deltaTime;
	
	}

	void ModMaxHealth(int _value)
	{
		PlayerMaxHealth += _value;
		if (PlayerMaxHealth <= 0)
		{
			PlayerMaxHealth = 0;
		}

		if (PlayerCurrHealth > PlayerMaxHealth)
			PlayerCurrHealth = PlayerMaxHealth;
	}
	void SetMaxHealth(int _NewMaxHealth)
	{
		PlayerMaxHealth = _NewMaxHealth;
		if (PlayerMaxHealth < 0)
		{
			PlayerMaxHealth = 0;
		}
		if (PlayerCurrHealth > PlayerMaxHealth)
			PlayerCurrHealth = PlayerMaxHealth;
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
	}

	void ModLivesLeft(int _value)
	{
		PlayerLivesLeft += _value;

		if (_value < 0)
			DeathCount -= _value;

		if (PlayerLivesLeft < 0)
		{
			DeathCount += PlayerLivesLeft;
			PlayerLivesLeft = 0;
		}
	}
	void SetLivesLeft(int _NewLives)
	{
		if (_NewLives < PlayerLivesLeft)
			DeathCount += PlayerLivesLeft - _NewLives;

		PlayerLivesLeft = _NewLives;

		if (PlayerLivesLeft < 0)
		{
			DeathCount += PlayerLivesLeft;
			PlayerLivesLeft = 0;
		}
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
		if (_value > 0)
			TotalSoulCount += _value;
	}
	void SetSouls(int _NewSouls)
	{
		SoulCount = _NewSouls;
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

}
