using UnityEngine;
using System.Collections;

public class Pause_Script : MonoBehaviour {

	// Pause Functionality
	public static bool gamePaused = false;

	public GameObject PauseMenu;
	public GameObject OptionsMenu;
	public GameObject UpgradesMenu;
	public GameObject SpellListMenu;
	public GameObject MessagePrompt;

	public GameObject[] pauseSouls = new GameObject[5];
	AudioSource[] sources;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{		
		if (Input.GetKeyDown (KeyCode.Escape) && gamePaused == false) 
		{
			Time.timeScale = 0;
			sources = GameObject.FindObjectsOfType<AudioSource>();
			for (int i = 0; i < sources.Length; i++) 
			{
				if(sources[i] != GameBrain.Instance.Music)
					sources[i].Pause();
			}
			gamePaused = true;
			PauseMenu.SetActive(true);
			GameBrain.Instance.SendMessage("ChangeMusic",GameBrain.Instance.MenuMusic);
		}
		
		else if (Input.GetKeyDown (KeyCode.Escape) && gamePaused == true)
		{
			Resume();
		}
	}

	public void Resume()
	{
		for (int i = 0; i < pauseSouls.Length; i++)
		{
			pauseSouls[i].SetActive(false);
		}
		if (OptionsMenu.activeSelf == false && 
			UpgradesMenu.activeSelf == false &&
		    SpellListMenu.activeSelf == false &&
			MessagePrompt.activeSelf == false) 
		{
			Time.timeScale = 1;
			for (int i = 0; i < sources.Length; i++) 
			{
				if(sources[i] != GameBrain.Instance.Music)
					sources[i].UnPause();
			}
			gamePaused = false;
			PauseMenu.SetActive(false);
			if(GameBrain.Instance.FightingBoss == true)
				GameBrain.Instance.SendMessage("ChangeMusic",GameBrain.Instance.BossMusic);
			else if(GameBrain.Instance.FightingCaptain == true)
				GameBrain.Instance.SendMessage("ChangeMusic",GameBrain.Instance.CaptainMusic);
			else
				GameBrain.Instance.SendMessage("ChangeMusic",GameBrain.Instance.GameplayMusic);
		}

	}

	public void Options()
	{
		OptionsMenu.SetActive (true);
	}

	public void Upgrades()
	{
		UpgradesMenu.SetActive (true);
		GameBrain.Instance.HUDMaster.GetComponent<Upgrades_Script> ().CheckLevelAvailability ();
	}

	public void SpellList()
	{
		SpellListMenu.SetActive (true);
	}

	public void Exit()
	{
		MessagePrompt.SetActive (true);
	}

	public void ActivateSoul1()
	{
		pauseSouls[0].SetActive(true);
	}
	public void ActivateSoul2()
	{
		pauseSouls[1].SetActive(true);
	}
	public void ActivateSoul3()
	{
		pauseSouls[2].SetActive(true);
	}
	public void ActivateSoul4()
	{
		pauseSouls[3].SetActive(true);
	}
	public void ActivateSoul5()
	{
		pauseSouls[4].SetActive(true);
	}

	public void DeactivateSoul1()
	{
		pauseSouls[0].SetActive(false);
	}
	public void DeactivateSoul2()
	{
		pauseSouls[1].SetActive(false);
	}
	public void DeactivateSoul3()
	{
		pauseSouls[2].SetActive(false);
	}
	public void DeactivateSoul4()
	{
		pauseSouls[3].SetActive(false);
	}
	public void DeactivateSoul5()
	{
		pauseSouls[4].SetActive(false);
	}
}
