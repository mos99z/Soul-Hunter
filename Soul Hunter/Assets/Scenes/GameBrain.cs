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
	public int CurrentLevel;
	
	
	//Tally Specific Info
	public int NumEnemiesKilled;
	public int DamageTaken;
	public int TotalSoulCount;
	public int DeathCount;
	public double GameTime;
	public int NumCastedSpells;
	
	// other info
	public bool[] spellsCast;
	//public GameObject[] selectedMacros;	
	//public bool[,] miniMap;	// to be enabled once figured out
}

public class GameBrain : MonoBehaviour {

	public static GameBrain Instance;

	public int PlayerMaxHealth = 1000;
	public int PlayerCurrHealth = 1000;
	public int PlayerLivesLeft = 3;
	public int SoulCount = 0;
	public int MeleeEnemyCounter = 0;
	// Level 0 is tutorial
	public int CurrentLevel = -1;
	public List<int> RoomsCleared;
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
	//public GameObject SpellDatabase = null;
	public GameObject Souls = null;		// replaced with master spell list
	//public GameObject Debuffs = null;		// replaced with master list script
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

//		if (SpellDatabase != null)
//			SpellDatabase.SetActive (false);
		if (Souls != null)
			Souls.SetActive (false);
//		if (Debuffs != null)
//			Debuffs.SetActive (false);
		if (DisplayText != null)
			DisplayText.SetActive (false);
		LoadPlayerData ();
		//gameInfo.RoomsCleared [3] = false;
	}

	void OnLevelWasLoaded(int level)
	{
		// level variable passed in is the build order scene level

		// currently using gamebrains actual level to load level position approriately
		int count = RoomsCleared.Count;
		if (CurrentLevel == 1)
		{
			if (count == 0)
				Player.transform.position = Vector3.zero;
			else
			{
				for (int i = 0; i < count; ++i)
				{
					switch (RoomsCleared[i])
					{
					case 1: Destroy(GameObject.Find("Room 01").transform.FindChild("Spawn Area").gameObject); break;
					case 2: 
					{
						Destroy(GameObject.Find("Room 02").transform.FindChild("Spawn Area").gameObject);
						Destroy(GameObject.Find("Room 02").transform.FindChild("Mage Captain").gameObject);
						break;
					}
					case 3: 
					{
						Destroy(GameObject.Find("Room 03").transform.FindChild("Spawn Area").gameObject);
						Destroy(GameObject.Find("Room 03").transform.FindChild("Mage Captain").gameObject);
						break;
					}
					case 4: Destroy(GameObject.Find("Room 04").transform.FindChild("Spawn Area").gameObject); break;
					case 5: 
					{
						Destroy(GameObject.Find("Room 05").transform.FindChild("Spawn Area").gameObject);
						Destroy(GameObject.Find("Room 05").transform.FindChild("Mage Captain").gameObject);
						break;
					}
					}
				}
				switch (RoomsCleared[count-1]) // spawn player in room last cleared
				{
				case 1: Player.transform.position = Vector3.zero; break;
				case 2: Player.transform.position = new Vector3(45.0f, 0.0f, 85.0f); break;
				case 3: Player.transform.position = new Vector3(-40.0f, 0.0f, 85.0f); break;
				case 4: Player.transform.position = new Vector3(-50.0f, 0.0f, 35.0f); break;
				case 5: Player.transform.position = new Vector3(-20.0f, 0.0f, 140.0f); break;
				}
			}
			GameBrain.Instance.HUDMaster.SetActive(true);
			GameBrain.Instance.Player.SetActive(true);
			GameBrain.Instance.MouseMarker.SetActive(true);
		}
		else if (CurrentLevel == 2)
		{
			if (count == 0)
				Player.transform.position = Vector3.zero;
			else
			{
				for (int i = 0; i < count; ++i)
				{
					switch (RoomsCleared[i])
					{
					case 1: Destroy(GameObject.Find("Room 01").transform.FindChild("Spawn Area").gameObject); break;
					case 2: 
					{
						Destroy(GameObject.Find("Room 02").transform.FindChild("Spawn Area").gameObject);
						Destroy(GameObject.Find("Room 02").transform.FindChild("Mage Captain").gameObject);
						break;
					}
					case 3: 
					{
						Destroy(GameObject.Find("Room 03").transform.FindChild("Spawn Area").gameObject);
						Destroy(GameObject.Find("Room 03").transform.FindChild("Mage Captain").gameObject);
						break;
					}
					case 4: Destroy(GameObject.Find("Room 04").transform.FindChild("Spawn Area").gameObject); break;
					case 5: 
					{
						Destroy(GameObject.Find("Room 05").transform.FindChild("Spawn Area").gameObject);
						Destroy(GameObject.Find("Room 05").transform.FindChild("Mage Captain").gameObject);
						break;
					}
					}
				}
				switch (RoomsCleared[count-1]) // spawn player in room last cleared
				{
				case 1: Player.transform.position = Vector3.zero; break;
				case 2: Player.transform.position = new Vector3(45.0f, 0.0f, 85.0f); break;
				case 3: Player.transform.position = new Vector3(-40.0f, 0.0f, 85.0f); break;
				case 4: Player.transform.position = new Vector3(-50.0f, 0.0f, 35.0f); break;
				case 5: Player.transform.position = new Vector3(-20.0f, 0.0f, 140.0f); break;
				}
			}
			GameBrain.Instance.HUDMaster.SetActive(true);
			GameBrain.Instance.Player.SetActive(true);
			GameBrain.Instance.MouseMarker.SetActive(true);
		}
		else if (CurrentLevel == 3)
		{
			if (count == 0)
				Player.transform.position = Vector3.zero;
			else
			{
				for (int i = 0; i < count; ++i)
				{
					switch (RoomsCleared[i])
					{
					case 1: Destroy(GameObject.Find("Room 01").transform.FindChild("Spawn Area").gameObject); break;
					case 2: 
					{
						Destroy(GameObject.Find("Room 02").transform.FindChild("Spawn Area").gameObject);
						Destroy(GameObject.Find("Room 02").transform.FindChild("Mage Captain").gameObject);
						break;
					}
					case 3: 
					{
						Destroy(GameObject.Find("Room 03").transform.FindChild("Spawn Area").gameObject);
						Destroy(GameObject.Find("Room 03").transform.FindChild("Mage Captain").gameObject);
						break;
					}
					case 4: Destroy(GameObject.Find("Room 04").transform.FindChild("Spawn Area").gameObject); break;
					case 5: 
					{
						Destroy(GameObject.Find("Room 05").transform.FindChild("Spawn Area").gameObject);
						Destroy(GameObject.Find("Room 05").transform.FindChild("Mage Captain").gameObject);
						break;
					}
					}
				}
				switch (RoomsCleared[count-1]) // spawn player in room last cleared
				{
				case 1: Player.transform.position = Vector3.zero; break;
				case 2: Player.transform.position = new Vector3(45.0f, 0.0f, 85.0f); break;
				case 3: Player.transform.position = new Vector3(-40.0f, 0.0f, 85.0f); break;
				case 4: Player.transform.position = new Vector3(-50.0f, 0.0f, 35.0f); break;
				case 5: Player.transform.position = new Vector3(-20.0f, 0.0f, 140.0f); break;
				}
			}
			GameBrain.Instance.HUDMaster.SetActive(true);
			GameBrain.Instance.Player.SetActive(true);
			GameBrain.Instance.MouseMarker.SetActive(true);
		}


	}
	void Update () {
		if (CurrentLevel > 0)
			GameTime += Time.deltaTime;
		else if (CurrentLevel == -2) {
			GameObject TallyScreen = GameObject.Find ("TallyScreenBack");

			TallyScreen.transform.FindChild("Game Time Count").GetComponent<Text>().text = GameTime.ToString("##00.00");
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
		
		info.CurrentLevel = CurrentLevel;
		info.NumEnemiesKilled = NumEnemiesKilled;
		info.DamageTaken = DamageTaken;
		info.TotalSoulCount = TotalSoulCount;
		info.DeathCount = DeathCount;
		info.GameTime = GameTime;
		info.NumCastedSpells = NumCastedSpells;
		
		info.spellsCast = SpellHasBeenCast;
		//info.selectedMacros = new GameObject[5];
		//for (int i = 0; i < 5; ++i)
		//	info.selectedMacros[i] = GameBrain.Instance.HUDMaster.GetComponent<MacroSelect>().spells[i];

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
		info.RoomsCleared[0] = 0;

		info.CurrentLevel = 0;
		info.NumEnemiesKilled = 0;
		info.DamageTaken = 0;
		info.TotalSoulCount = 0;
		info.DeathCount = 0;
		info.GameTime = 0;
		info.NumCastedSpells = 0;

		info.spellsCast = new bool[25];
		for (int i = 0; i < 25; ++i)
			info.spellsCast[i] = false;
//		info.selectedMacros = new string[5];
//		for (int i = 0; i < 5; ++i)			// TODO: when refactoring spells, assign the defaults here
//			info.selectedMacros[i] = GameBrain.Instance.HUDMaster.GetComponent<MacroSelect>().spells[i].transform.name;

		
		bf.Serialize (file, info);
		file.Close ();
	}

	// call this function when selecting continue
	public void Load()
	{
		string loadFile = Application.persistentDataPath + "/PlayerInfo.dat";
		
		if(File.Exists(loadFile))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(loadFile, FileMode.Open);
			GameInfo info = (GameInfo)bf.Deserialize(file);
			file.Close();
			
			// set values of our gameBrain to those loaded from save
			PlayerMaxHealth = info.PlayerMaxHealth;
			PlayerCurrHealth = info.PlayerCurrHealth;
			PlayerLivesLeft = info.PlayerLivesLeft;
			SoulCount = info.SoulCount;

			FireLevel = info.FireLevel;
			WindLevel = info.WindLevel;
			EarthLevel = info.EarthLevel;
			ElectricLevel = info.ElectricLevel;
			WaterLevel = info.WaterLevel;

			RoomsCleared.Clear();
			for (int i = 0; i < info.RoomsCleared.Length; ++i)
				RoomsCleared.Add(info.RoomsCleared[i]);

			CurrentLevel = info.CurrentLevel;
			NumEnemiesKilled = info.NumEnemiesKilled;
			DamageTaken = info.DamageTaken;
			TotalSoulCount = info.TotalSoulCount;
			DeathCount = info.DeathCount;
			GameTime = info.GameTime;
			NumCastedSpells = info.NumCastedSpells;

			SpellHasBeenCast = info.spellsCast;
//			for (int i = 0; i < 5; i++)
//				GameBrain.Instance.HUDMaster.GetComponent<MacroSelect>().spells[i] = info.selectedMacros[i];
		}
	}

}
