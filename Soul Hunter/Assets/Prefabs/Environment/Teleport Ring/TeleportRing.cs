﻿using UnityEngine;
using System.Collections;

public class TeleportRing : MonoBehaviour 
{
	public string levelToLoad;		// use this to govern which level to load
	public int nextLevel;			// use this to update the gamebrain on which the next level will be
	public GameObject LoadingScreen;
	AsyncOperation ao = null;

	void Start()
	{
		LoadingScreen = GameBrain.Instance.transform.FindChild("Loading Screen").gameObject;
	}

	void Update()
	{
		if (ao != null) 
		{
			if (ao.progress == 0.9f) 
			{
				LoadingScreen.SetActive(false);
				ao.allowSceneActivation = true;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			GameBrain.Instance.RoomsCleared.Clear();	// empty rooms cleared in prep for next level
			GameBrain.Instance.CurrentLevel = nextLevel;// sets the gamebrain to know which level the player is now in
			GameBrain.Instance.Save();					// save game progress
			LoadingScreen.SetActive(true);
			LoadingScreen.GetComponentInChildren<Animator>().Play("Loading_Screen");
			ao = Application.LoadLevelAsync(levelToLoad);			// load this level
		}
	}
}
