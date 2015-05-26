using UnityEngine;
using System.Collections;

public class Options_Script : MonoBehaviour {

	public int SFXVolume;
	public int MusicVolume;
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
		MusicVolume = PlayerPrefs.GetInt ("MusicVolume", 100);
		SFXVolume = PlayerPrefs.GetInt ("SFXVolume", 100);
	}

	public void SaveAndClose()
	{
		PlayerPrefs.SetInt ("MusicVolume", MusicVolume);
		PlayerPrefs.SetInt ("SFXVolume", SFXVolume);
	}

	public void Close()
	{}
}
