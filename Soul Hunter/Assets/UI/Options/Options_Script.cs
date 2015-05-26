using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Options_Script : MonoBehaviour {

	public GameObject Options;
	public int SFXVolume;
	public int MusicVolume;
	public Slider MusicSlider;
	public Slider SFXSlider;
	//public 

	// Use this for initialization
	void Awake () 
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
		MusicVolume = PlayerPrefs.GetInt ("MusicVolume", 100);
		SFXVolume = PlayerPrefs.GetInt ("SFXVolume", 100);
		MusicSlider.value = MusicVolume;
		SFXSlider.value = SFXVolume;
	}

	public void AdjustMusVol()
	{
		MusicVolume = (int)MusicSlider.value;
	}

	public void AdjustSFXVol()
	{
		SFXVolume = (int)SFXSlider.value;
	}

	public void SaveAndClose()
	{
		PlayerPrefs.SetInt ("MusicVolume", MusicVolume);
		PlayerPrefs.SetInt ("SFXVolume", SFXVolume);
		Options.SetActive (false);
	}

	public void Close()
	{
		MusicSlider.value = PlayerPrefs.GetInt ("MusicVolume", 100);
		SFXSlider.value = PlayerPrefs.GetInt ("SFXVolume", 100);
		Options.SetActive (false);
	}
}
