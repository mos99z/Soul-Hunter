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
[System.Serializable]
public class GameInfo		// for game save/load
{
	public int PlayerMaxHealth;
	public int PlayerCurrHealth;
	public int PlayerLivesLeft;
	public int SoulCount;
	//public GameObject player;		// this shouldn't be needed

	// upgrades
	public int FireLevel;
	public int WindLevel;
	public int EarthLevel;
	public int ElectricLevel;
	public int WaterLevel;
	
	public int[] RoomsCleared;	// used for keeping track of minion rooms cleared and where to spawn player
	
	// Level 0 is tutorial
	public int LevelProgress;
	
	//Tally Specific Info
	public int NumEnemiesKilled;
	public int DamageTaken;
	public int TotalSoulCount;
	public int DeathCount;
	public double GameTime;
	public int NumCastedSpells;
	
	// other info
	public bool[] spellsCast;
	public float RespawnLocX;
	public float RespawnLocY;
	public float RespawnLocZ;
	//public bool[,] miniMap;	// to be enabled once figured out
}

public class GameBrain : MonoBehaviour {

	public static GameBrain Instance;

	public AudioClip MenuMusic;
	public AudioClip GameplayMusic;
	public AudioClip CaptainMusic;
	public AudioClip BossMusic;
	public AudioClip GameOverMusic;

	public int PlayerMaxHealth = 1000;
	public int PlayerCurrHealth = 1000;
	public int PlayerLivesLeft = 3;
	public int SoulCount = 0;
	public int SoulsAtLevelStart = 0;
	public int MeleeEnemyCounter = 0;
	public bool PlayerInFog = false;
	public bool FightingCaptain = false;
	public bool FightingBoss = false;
	// Level 0 is tutorial
	public int CurrentLevel = -1;
	public int LevelProgress = 0;
	public List<int> RoomsCleared;
	public Vector3 RespawnLoc = Vector3.zero;
	private GameInfo gameInfo;
	public AudioSource Music;
	public float MusicVolume;
	public float SFXVolume;

	public int FireLevel = 0;
	public int WindLevel = 0;
	public int EarthLevel = 0;
	public int ElectricLevel = 0;
	public int WaterLevel = 0;
	public int NumberOfLevels = 3;
	public int FireLevelAtStart = 0;
	public int WindLevelAtStart = 0;
	public int EarthLevelAtStart = 0;
	public int ElectricLevelAtStart = 0;
	public int WaterLevelAtStart = 0;

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
	public GameObject Souls = null;
	public GameObject DisplayText = null;
	public GameObject loadingScreen = null;
	
	public bool[] SpellHasBeenCast = {false, false, false, false, false, false, false, false, false,
		false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false};
	string[] spellNames = {"Barrier","Bolt","Bolt Chain","Concrete","Crystal Spikes","Explosion","Fire Ball","Fog","Freeze","Gravity Well","Hydrant",
		"Laser","Magma","Meteor","Muck","Plasma","Poison Cloud","Rock Spike","Sand Blast", "Shock Prism","Steam", "Tidal Wave", "Torch","Whirlwind","Wind Blade"};

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
		MusicVolume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		SFXVolume = PlayerPrefs.GetFloat ("SFXVolume", 1.0f);

		AudioListener.volume = SFXVolume;
		Music.ignoreListenerVolume = true;
		Music.volume = MusicVolume;
		Music.Play ();

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

		if (Souls != null)
			Souls.SetActive (false);
		if (DisplayText != null)
			DisplayText.SetActive (false);
	}

	void OnLevelWasLoaded(int level)
	{
		// level variable passed in is the build order scene level

		// currently using gamebrains actual level to load level position approriately

		// send these messages to deactivate in case they are active
		GameBrain.Instance.HUDMaster.SendMessage("DeactivateCaptBar", SendMessageOptions.DontRequireReceiver);
		GameBrain.Instance.HUDMaster.SendMessage("DeactivateBossBar", SendMessageOptions.DontRequireReceiver);
		GameBrain.Instance.HUDMaster.SendMessage("DeactivateDualBar", SendMessageOptions.DontRequireReceiver);
		if (CurrentLevel >= 0)
		{
			GameBrain.Instance.HUDMaster.SetActive(true);
			GameBrain.Instance.Player.SetActive(true);
			GameBrain.Instance.MouseMarker.SetActive(true);
			int count = RoomsCleared.Count;
			if (count == 0)
				Player.transform.position = Vector3.zero;
			else
			{
				for (int i = 0; i < count; ++i)
					GameObject.Find("Room " + RoomsCleared[i].ToString()).transform.FindChild("Spawn Area").gameObject.SetActive(false);
				Player.transform.position = RespawnLoc;
			}
		}
	}

	void Update ()
	{
		if (Input.GetKey(KeyCode.I) && Input.GetKeyDown(KeyCode.V))
		    Player.GetComponent<Living_Obj>().CanTakeDamage = !Player.GetComponent<Living_Obj>().CanTakeDamage;
		if (CurrentLevel > 0)
			GameTime += Time.deltaTime;
		else if (CurrentLevel == -2)
		{
			GameObject TallyScreen = GameObject.Find ("TallyScreenBack");
			double tempTime = GameTime;
			int hours = (int)(tempTime / 3600);
			tempTime = (int)tempTime - hours * 3600;
			int minutes = (int)(tempTime / 60);
			int seconds = (int)tempTime - minutes * 60;

			string fulltime;
			if (hours < 10)
			{
				fulltime = "0" + hours + ":";
			}
			else
			{
				fulltime = hours + ":";
			}
			if (minutes < 10)
			{
				fulltime += "0" + minutes + ":";
			}
			else
			{
				fulltime += minutes + ":";
			}
			if (seconds < 10)
			{
				fulltime += "0" + seconds;
			}
			else
			{
				fulltime += seconds.ToString();
			}

			TallyScreen.transform.FindChild("Game Time Count").GetComponent<Text>().text = fulltime;
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

			if (PlayerLivesLeft > 0)
				PlayerCurrHealth = PlayerMaxHealth;
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

	public void SetLevel(int _Level)
	{
		CurrentLevel = _Level;
		if (CurrentLevel > LevelProgress)
		{
			RoomsCleared = new List<int>();
			RespawnLoc = Vector3.zero;
			LevelProgress = CurrentLevel;
			SoulsAtLevelStart = SoulCount;
			FireLevelAtStart = FireLevel;
			WaterLevelAtStart = WaterLevel;
			ElectricLevelAtStart = ElectricLevel;
			EarthLevelAtStart = EarthLevel;
			WindLevelAtStart = WindLevel;
		}

		if (CurrentLevel >= 0)
		{
			if (CurrentLevel == 0)
				Reset();

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
		Player.GetComponent<Player_Caster_Controller> ().SetRecoverTime (0.001f);
	}

	public void Reset()
	{
		SetMaxHealth(2000);
		SetHealth(2000);
		SetLivesLeft(3);
		SetSouls(0);
		
		FireLevel = 0;
		WindLevel = 0;
		EarthLevel = 0;
		ElectricLevel = 0;
		WaterLevel = 0;
		
		RoomsCleared = new List<int>();
		
		LevelProgress = 0;
		NumEnemiesKilled = 0;
		DamageTaken = 0;
		TotalSoulCount = 0;
		DeathCount = 0;
		GameTime = 0;
		NumCastedSpells = 0;
		RespawnLoc = Vector3.zero;
		transform.position = Vector3.zero;
		
		HUDMaster.GetComponent<MacroSelect> ().curMac = 0;
		HUDMaster.GetComponent<MacroSelect> ().spells[0] = GameBrain.Instance.GetComponent<SpellMasterList> ().fireBall;
		HUDMaster.GetComponent<MacroSelect> ().spells[1] = GameBrain.Instance.GetComponent<SpellMasterList> ().windBlade;
		HUDMaster.GetComponent<MacroSelect> ().spells[2] = GameBrain.Instance.GetComponent<SpellMasterList> ().rockSpike;
		HUDMaster.GetComponent<MacroSelect> ().spells[3] = GameBrain.Instance.GetComponent<SpellMasterList> ().bolt;
		HUDMaster.GetComponent<MacroSelect> ().spells[4] = GameBrain.Instance.GetComponent<SpellMasterList> ().hydrant;
		
		HUDMaster.GetComponent<MacroSelect> ().needsUpdate = true;
	}

	public void ChangeMusic(AudioClip clip)
	{
		if (Music.clip == clip)
			return;
		Music.Stop ();
		Music.clip = clip;
		Music.Play ();
	}

	public void ChangeSoulHud()
	{
		HUDMaster.GetComponent<StatsDisplay> ().SetSoulsDisplay((uint)SoulCount);
	}

	// call this function anywhere to save the game at anytime
	public void Save()
	{
		BinaryFormatter bf = new BinaryFormatter ();

		string saveDestination = Application.persistentDataPath + "/PlayerInfo.dat";
		Debug.Log (saveDestination);
		FileStream file = File.Create (saveDestination);
		
		// store all the game information into our container class
		GameInfo info = new GameInfo();
		info.PlayerMaxHealth = PlayerMaxHealth;
		info.PlayerCurrHealth = PlayerCurrHealth;
		info.PlayerLivesLeft = PlayerLivesLeft;
		info.SoulCount = SoulCount;

		info.FireLevel = FireLevel;
		info.WindLevel = WindLevel;
		info.EarthLevel = EarthLevel;
		info.ElectricLevel = ElectricLevel;
		info.WaterLevel = WaterLevel;

		info.RoomsCleared = new int[RoomsCleared.Count];
		for (int i = 0; i < RoomsCleared.Count; i++)
			info.RoomsCleared[i] = RoomsCleared[i];
		
		info.LevelProgress = CurrentLevel;
		info.NumEnemiesKilled = NumEnemiesKilled;
		info.DamageTaken = DamageTaken;
		info.TotalSoulCount = TotalSoulCount;
		info.DeathCount = DeathCount;
		info.GameTime = GameTime;
		info.NumCastedSpells = NumCastedSpells;
		
		info.spellsCast = SpellHasBeenCast;
		info.RespawnLocX = Player.transform.position.x;
		info.RespawnLocY = Player.transform.position.y;
		info.RespawnLocZ = Player.transform.position.z;
	
		bf.Serialize (file, info);
		file.Close ();
	}

	// call this function when selecting new game to nuke the original file
	public void EraseFile()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		
		string saveDestination = Application.persistentDataPath + "/PlayerInfo.dat";
		Debug.Log (saveDestination);
		FileStream file = File.Create (saveDestination);
		
		// store all the game information into our container class
		GameInfo info = new GameInfo();
		info.PlayerMaxHealth = 2000;
		info.PlayerCurrHealth = 2000;
		info.PlayerLivesLeft = 3;
		info.SoulCount = 0;
		
		info.FireLevel = 0;
		info.WindLevel = 0;
		info.EarthLevel = 0;
		info.ElectricLevel = 0;
		info.WaterLevel = 0;

		info.RoomsCleared = new int[1];
		info.RoomsCleared[0] = -1;

		info.LevelProgress = 1;
		info.NumEnemiesKilled = 0;
		info.DamageTaken = 0;
		info.TotalSoulCount = 0;
		info.DeathCount = 0;
		info.GameTime = 0;
		info.NumCastedSpells = 0;
		info.RespawnLocX = info.RespawnLocY = info.RespawnLocZ = 0.0f;

		HUDMaster.SetActive (true);
		HUDMaster.GetComponent<MacroSelect> ().spells[0] = GameBrain.Instance.GetComponent<SpellMasterList> ().fireBall;
		HUDMaster.GetComponent<MacroSelect> ().spells[1] = GameBrain.Instance.GetComponent<SpellMasterList> ().windBlade;
		HUDMaster.GetComponent<MacroSelect> ().spells[2] = GameBrain.Instance.GetComponent<SpellMasterList> ().rockSpike;
		HUDMaster.GetComponent<MacroSelect> ().spells[3] = GameBrain.Instance.GetComponent<SpellMasterList> ().bolt;
		HUDMaster.GetComponent<MacroSelect> ().spells[4] = GameBrain.Instance.GetComponent<SpellMasterList> ().hydrant;
		HUDMaster.GetComponent<MacroSelect> ().curMac = 0;

		HUDMaster.GetComponent<MacroSelect> ().needsUpdate = true;

		info.spellsCast = new bool[25];
		for (int i = 0; i < 25; ++i)
			info.spellsCast[i] = false;

		bf.Serialize (file, info);
		file.Close ();
	}

	// call this function when selecting continue
	public void Load()
	{
		string loadFile = Application.persistentDataPath + "/PlayerInfo.dat";
		
		if (File.Exists (loadFile))
		{
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (loadFile, FileMode.Open);
			GameInfo info = (GameInfo)bf.Deserialize (file);
			file.Close ();
			
			// set values of our gameBrain to those loaded from save
			SetMaxHealth (info.PlayerMaxHealth);
			GameBrain.Instance.Player.GetComponent<Living_Obj> ().MaxHealth = info.PlayerMaxHealth;

			SetHealth (info.PlayerCurrHealth);
			GameBrain.Instance.Player.GetComponent<Living_Obj> ().CurrHealth = info.PlayerCurrHealth;

			SetLivesLeft (info.PlayerLivesLeft);
			GameBrain.Instance.Player.GetComponent<Living_Obj> ().Lives = info.PlayerLivesLeft;

			SetSouls (info.SoulCount);

			FireLevel = info.FireLevel;
			WindLevel = info.WindLevel;
			EarthLevel = info.EarthLevel;
			ElectricLevel = info.ElectricLevel;
			WaterLevel = info.WaterLevel;

			RoomsCleared = new List<int> ();
			if (info.RoomsCleared.Length > 0 && info.RoomsCleared [0] != -1)
				for (int i = 0; i < info.RoomsCleared.Length; ++i)
					RoomsCleared.Add (info.RoomsCleared [i]);

			CurrentLevel = LevelProgress = info.LevelProgress;
			NumEnemiesKilled = info.NumEnemiesKilled;
			DamageTaken = info.DamageTaken;
			TotalSoulCount = info.TotalSoulCount;
			DeathCount = info.DeathCount;
			GameTime = info.GameTime;
			NumCastedSpells = info.NumCastedSpells;
			SpellHasBeenCast = info.spellsCast;

			Vector3 newRespawn;
			newRespawn.x = info.RespawnLocX;
			newRespawn.y = info.RespawnLocY;
			newRespawn.z = info.RespawnLocZ;
			RespawnLoc = newRespawn;
			Player.transform.position = RespawnLoc;
		}
		else
		{
			EraseFile();
			Load();
		}
	}
}
