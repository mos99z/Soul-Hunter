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

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{		
		if (Input.GetKeyDown (KeyCode.Escape) && gamePaused == false) 
		{
			int zero = 0;
			Time.timeScale = 0;
			gamePaused = true;
			PauseMenu.SetActive(true);
			GameBrain.Instance.SendMessage("ChangeMusic",zero);
		}
		
		else if (Input.GetKeyDown (KeyCode.Escape) && gamePaused == true)
		{
			Resume();
			GameBrain.Instance.SendMessage("ChangeMusic",1);
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
			gamePaused = false;
			PauseMenu.SetActive(false);
		}
	}

	public void Options()
	{
		OptionsMenu.SetActive (true);
	}

	public void Upgrades()
	{
		UpgradesMenu.SetActive (true);
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
