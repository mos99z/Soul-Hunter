using UnityEngine;
using System.Collections;

public class Pause_Script : MonoBehaviour {

	// Pause Functionality
	public static bool gamePaused = false;
	public static float pauseTimer = 0.0f;

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
		pauseTimer -= 0.033f;
		
		if (Input.GetKeyDown (KeyCode.Escape) && gamePaused == false /*&& pauseTimer <= 0.0f*/) 
		{
			Time.timeScale = 0;
			gamePaused = true;
			pauseTimer = 1.0f;
			PauseMenu.SetActive(true);
		}
		
		else if (Input.GetKeyDown (KeyCode.Escape) && gamePaused == true /*&& pauseTimer <= 0.0f*/ )
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
			pauseTimer = 1.0f;
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
