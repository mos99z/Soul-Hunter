﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Options_Script2 : MonoBehaviour
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
	
	//handle SoulIndex
	public GameObject[] soulIcons = new GameObject[5];
	
	//Get MainMenu Variables
	private Main_Menu_Script mainMenScript;
	
	//helper vars
	private int index;
	private bool needsUpdate;
	private bool opening;
	public AudioSource TestSFX;

	// Use this for initialization
	void Start () 
	{
		opening = true;
		if (MainMenu != null)
		{
			mainMenScript = (Main_Menu_Script)MainMenu.GetComponent("Main_Menu_Script");
		}
		LoadData ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (opening)
		{
			OpeningMenu();
			opening = false;
		}
		if (needsUpdate)
		{
			for (int i = 0; i < soulIcons.Length; i++)
			{
				soulIcons[i].SetActive(false);
			}
			soulIcons[index].SetActive(true);
			needsUpdate = false;
		}
		
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			index++;
			if (index > 4)
			{
				index = 0;
			}
			needsUpdate = true;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			index--;
			if (index < 0)
			{
				index = 4;
			}
			needsUpdate = true;
		}
		//MusicVolume;
		switch (index)
		{
		case 0:
		{
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				SFXSlider.value -= Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.RightArrow))
			{
				SFXSlider.value += Time.deltaTime;
			}
			break;
		}
		case 1:
		{
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				MusicSlider.value -= Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.RightArrow))
			{
				MusicSlider.value += Time.deltaTime;
			}
			break;
		}
		case 2:
		{
			if (Input.GetKey(KeyCode.Return))
			{
				ToggleFullscreen();
			}
			break;
		}
		case 3:
		{
			if (Input.GetKey(KeyCode.Return))
			{
				SaveAndClose();
			}
			if (Input.GetKey(KeyCode.RightArrow))
			{
				index++;
				needsUpdate = true;
			}
			break;
		}
		case 4:
		{
			if (Input.GetKey(KeyCode.Return))
			{
				Close();
			}
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				index--;
				needsUpdate = true;
			}
			break;
		}
		default:
			break;
		}
	}
	
	private void OpeningMenu()
	{
		index = 0;
		needsUpdate = true;
		MusicVolume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		SFXVolume = PlayerPrefs.GetFloat ("SFXVolume", 1.0f);
		MusicSlider.value = MusicVolume;
		SFXSlider.value = SFXVolume;
	}
	
	void LoadData()
	{
		MusicVolume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		SFXVolume = PlayerPrefs.GetFloat ("SFXVolume", 1.0f);
		MusicSlider.value = MusicVolume;
		SFXSlider.value = SFXVolume;
		if (!Screen.fullScreen)
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
	
	public void AdjustMusVol()
	{
		MusicVolume = MusicSlider.value;
		if (Options.activeSelf) 
		{
			GameBrain.Instance.CasualMusic.volume = MusicVolume;
			GameBrain.Instance.GameMusic.volume = MusicVolume;
		}
	}
	
	public void AdjustSFXVol()
	{
		SFXVolume = SFXSlider.value;
		AudioListener.volume = SFXSlider.value;
		
		if (Options.activeSelf) 
		{
			TestSFX.volume = SFXVolume;
			if(TestSFX.isPlaying == false)
				TestSFX.Play();		
		}
	}
	
	public void ToggleFullscreen()
	{
		Screen.fullScreen = !Screen.fullScreen;
		
		if (!Screen.fullScreen)
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
		opening = true;
		PlayerPrefs.SetFloat ("MusicVolume", MusicVolume);
		PlayerPrefs.SetFloat ("SFXVolume", SFXVolume);
		GameBrain.Instance.CasualMusic.volume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		GameBrain.Instance.GameMusic.volume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		AudioListener.volume = PlayerPrefs.GetFloat ("SFXVolume", 1.0f);

		TestSFX.volume = PlayerPrefs.GetFloat ("SFXVolume", 1.0f);
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
		opening = true;
		MusicSlider.value = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		SFXSlider.value = PlayerPrefs.GetFloat ("SFXVolume", 1.0f);
		GameBrain.Instance.CasualMusic.volume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		GameBrain.Instance.GameMusic.volume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		TestSFX.volume = PlayerPrefs.GetFloat ("SFXVolume", 1.0f);
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
	
	public void SetIndex0()
	{
		index = 0;
		needsUpdate = true;
	}
	public void SetIndex1()
	{
		index = 1;
		needsUpdate = true;
	}
	public void SetIndex2()
	{
		index = 2;
		needsUpdate = true;
	}
	public void SetIndex3()
	{
		index = 3;
		needsUpdate = true;
	}
	public void SetIndex4()
	{
		index = 4;
		needsUpdate = true;
	}
}
