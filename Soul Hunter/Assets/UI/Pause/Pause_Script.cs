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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{		
		if (Input.GetKeyDown (KeyCode.Escape) && gamePaused == false) 
		{
			Time.timeScale = 0;
			gamePaused = true;
			PauseMenu.SetActive(true);
		}
		
		else if (Input.GetKeyDown (KeyCode.Escape) && gamePaused == true)
		{
			Resume();
		}
	}

	public void Resume()
	{
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
}
