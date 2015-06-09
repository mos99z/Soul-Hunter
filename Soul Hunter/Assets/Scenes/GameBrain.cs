using UnityEngine;
using UnityEngine.UI;
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

	public static GameBrain Instance;

	public int PlayerMaxHealth = 1000;
	public int PlayerCurrHealth = 1000;
	public int PlayerLivesLeft = 3;
	public int SoulCount = 0;
	// Level 0 is tutorial
	public int CurrentLevel = -1;

	private GameInfo gameInfo;

	public int FireLevel = 0;
	public int WindLevel = 0;
	public int EarthLevel = 0;
	public int ElectricLevel = 0;
	public int WaterLevel = 0;

	//Tally Specific Info
	public int NumEnemiesKilled = 0;
	public int DamageTaken = 0;
	public int DamageDealt = 0;
	public int TotalSoulCount = 0;
	public int DeathCount = 0;
	public double GameTime = 0.0;
	public int NumCastedSpells = 0;

	[Header("Must Be Set with Children!")]
	public GameObject Player = null;
	public GameObject MouseMarker = null;
	public GameObject HUDMaster = null;
	public GameObject SpellDatabase = null;
	public GameObject Souls = null;
	public GameObject Debuffs = null;
	public GameObject DisplayText = null;
	public AudioSource[] Music;// = GetComponents<AudioSource> ();




	public bool[] SpellHasBeenCast = {false, false, false, false, false, false, false, false, false,
		false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false};
	string[] spellNames = {"Aqua Jet","Barrier","Bolt","Bolt Chain","Concrete","Crystal Spikes","Explosion","Fire Ball","Fog","Freeze","Gravity Well","Hydrant",
		"Laser","Magma","Meteor","Muck","Plasma","Poison Cloud","Rock Spike","Sand Blast", "Shock Prism","Steam", "Torch","Whirlwind","Wind Blade"};

	void Awake()
	{
		if (Instance)
			DestroyImmediate (gameObject);
		else
		{
			DontDestroyOnLoad (gameObject);
			Instance = this;
		}


		
	}

	void Start ()
	{
		Music [0].volume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		Music [1].volume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);

		if (CurrentLevel >= 0) {
			if (Player != null)
				Player.SetActive (true);
			if (MouseMarker != null)
				MouseMarker.SetActive (true);
			if (HUDMaster != null)
				HUDMaster.SetActive (true);
		}
		else
		{
			if (Player != null)
				Player.SetActive (false);
			if (MouseMarker != null)
				MouseMarker.SetActive (false);
			if (HUDMaster != null)
				HUDMaster.SetActive (false);
		}

		if (SpellDatabase != null)
			SpellDatabase.SetActive (false);
		if (Souls != null)
			Souls.SetActive (false);
		if (Debuffs != null)
			Debuffs.SetActive (false);
		if (DisplayText != null)
			DisplayText.SetActive (false);
		LoadPlayerData ();
		//gameInfo.RoomsCleared [3] = false;
	}
	
	void Update () {
		if (CurrentLevel > 0)
			GameTime += Time.deltaTime;
		else if (CurrentLevel == -2) {
			GameObject TallyScreen = GameObject.Find ("TallyScreenBack");

			TallyScreen.transform.FindChild("Game Time Count").GetComponent<Text>().text = GameTime.ToString();
			TallyScreen.transform.FindChild("Damage Dealt Count").GetComponent<Text>().text = DamageDealt.ToString();
			TallyScreen.transform.FindChild("Enemies Defeated Count").GetComponent<Text>().text = NumEnemiesKilled.ToString();
			TallyScreen.transform.FindChild("Souls Collected Count").GetComponent<Text>().text = TotalSoulCount.ToString();
			TallyScreen.transform.FindChild("Damage Taken Count").GetComponent<Text>().text = DamageTaken.ToString();
			TallyScreen.transform.FindChild("Lives Lost Count").GetComponent<Text>().text = DeathCount.ToString();
			TallyScreen.transform.FindChild("Spells Casted Count").GetComponent<Text>().text = NumCastedSpells.ToString();
		}
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
		HUDMaster.GetComponent<StatsDisplay> ().SetMaxHealthDisplay(PlayerMaxHealth);
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
		HUDMaster.GetComponent<StatsDisplay> ().SetHealthDisplay(PlayerCurrHealth);
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
		HUDMaster.GetComponent<StatsDisplay> ().SetLivesDisplay((uint)PlayerLivesLeft);
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
		HUDMaster.GetComponent<StatsDisplay> ().SetSoulsDisplay((uint)SoulCount);
		if (_value > 0)
			TotalSoulCount += _value;
	}

	void SetSouls(int _NewSouls)
	{
		SoulCount = _NewSouls;
		HUDMaster.GetComponent<StatsDisplay> ().SetSoulsDisplay((uint)SoulCount);
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

	void SpellWasCast(GameObject spell)
	{
		for (int i = 0; i < spellNames.Length; i++) 
		{
			if(spell.name == spellNames[i])
				SpellHasBeenCast[i] = true;
		}
	}

	void SetLevel(int _Level)
	{
		CurrentLevel = _Level;
		if (CurrentLevel >= 0) {
			if (Player != null)
				Player.SetActive (true);
			if (MouseMarker != null)
				MouseMarker.SetActive (true);
			if (HUDMaster != null)
				HUDMaster.SetActive (true);
		}
		else
		{
			if (Player != null)
				Player.SetActive (false);
			if (MouseMarker != null)
				MouseMarker.SetActive (false);
			if (HUDMaster != null)
				HUDMaster.SetActive (false);
		}
	}

	void LoadPlayerData()
	{
		if (File.Exists (Application.persistentDataPath + "/PlayerInfo.dat")) 
		{

		}
	}

	void ChangeMusic(int index)
	{
		if (Music [index].isPlaying)
			return;

		switch (index) {
		case 0:
		{
			Music[1].Stop();
			Music[0].Play();
			break;
		}
		case 1:
		{
			Music[0].Stop();
			Music[1].Play();
			break;
		}
		}
	}

	public void ChangeSoulHud()
	{
		HUDMaster.GetComponent<StatsDisplay> ().SetSoulsDisplay((uint)SoulCount);
	}
}
