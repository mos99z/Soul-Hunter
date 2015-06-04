using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Options_Script : MonoBehaviour
{
	//Handle Menus
	public GameObject Options;
	public GameObject MainMenu = null;

	//Handle Buttons
	public float SFXVolume;
	public float MusicVolume;
	public Slider MusicSlider;
	public Slider SFXSlider;
	public Text FullScreen;
	public GameObject FullOn;
	public GameObject FullOff;

	//Get MainMenu Variables
	private Main_Menu_Script mainMenScript;

	// Use this for initialization
	void Start () 
	{
		if (MainMenu != null)
		{
			mainMenScript = (Main_Menu_Script)MainMenu.GetComponent("Main_Menu_Script");
		}
		LoadData ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//MusicVolume;
	}

	void LoadData()
	{
		MusicVolume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		SFXVolume = PlayerPrefs.GetFloat ("SFXVolume", 1.0f);
		MusicSlider.value = MusicVolume;
		SFXSlider.value = SFXVolume;
		FullOn.SetActive(false);
		FullOff.SetActive(true);
	}

	public void AdjustMusVol()
	{
		MusicVolume = MusicSlider.value;
		if (Options.activeSelf) 
		{
			GameBrain.Instance.Music [0].volume = MusicVolume;
			GameBrain.Instance.Music [1].volume = MusicVolume;
		}
	}

	public void AdjustSFXVol()
	{
		SFXVolume = SFXSlider.value;
		if (Options.activeSelf) 
		{
			GameBrain.Instance.Music [2].volume = SFXVolume;
			if(GameBrain.Instance.Music[2].isPlaying == false)
				GameBrain.Instance.Music [2].Play();		
		}
	}

	public void ToggleFullscreen()
	{
		Screen.fullScreen = !Screen.fullScreen;

		FullScreen.text = Screen.fullScreen ? "Fullscreen" : "Windowed";

		if (Screen.fullScreen)
		{
			FullOn.SetActive(true);
			FullOff.SetActive(false);
		}
		else
		{
			FullOn.SetActive(false);
			FullOff.SetActive(true);
		}
	}

	public void SaveAndClose()
	{
		PlayerPrefs.SetFloat ("MusicVolume", MusicVolume);
		PlayerPrefs.SetFloat ("SFXVolume", SFXVolume);
		GameBrain.Instance.Music [0].volume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		GameBrain.Instance.Music [1].volume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		Options.SetActive (false);
		if (MainMenu != null)
		{
			for (int i = 0; i < mainMenScript.buttons.Length; i++)
			{
				mainMenScript.buttons[i].interactable = true;
				mainMenScript.stall = false;
			}
		}
	}

	public void Close()
	{
		MusicSlider.value = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		SFXSlider.value = PlayerPrefs.GetFloat ("SFXVolume", 1.0f);
		GameBrain.Instance.Music [0].volume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		GameBrain.Instance.Music [1].volume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		Options.SetActive (false);
		if (MainMenu != null)
		{
			for (int i = 0; i < mainMenScript.buttons.Length; i++)
			{
				mainMenScript.buttons[i].interactable = true;
				mainMenScript.stall = false;
			}
		}
	}
}
