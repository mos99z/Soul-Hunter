using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Options_Script : MonoBehaviour {

	public GameObject Options;
	public float SFXVolume;
	public float MusicVolume;
	public Slider MusicSlider;
	public Slider SFXSlider;
	public Text FullScreen;
	//public 

	// Use this for initialization
	void Start () 
	{
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
	}

	public void SaveAndClose()
	{
		PlayerPrefs.SetFloat ("MusicVolume", MusicVolume);
		PlayerPrefs.SetFloat ("SFXVolume", SFXVolume);
		GameBrain.Instance.Music [0].volume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		GameBrain.Instance.Music [1].volume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		Options.SetActive (false);
	}

	public void Close()
	{
		MusicSlider.value = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		SFXSlider.value = PlayerPrefs.GetFloat ("SFXVolume", 1.0f);
		GameBrain.Instance.Music [0].volume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		GameBrain.Instance.Music [1].volume = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		Options.SetActive (false);
	}
}
