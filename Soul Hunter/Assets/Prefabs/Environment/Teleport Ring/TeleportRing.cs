using UnityEngine;
using System.Collections;

public class TeleportRing : MonoBehaviour 
{
	public string levelToLoad;		// use this to govern which level to load
	public int nextLevel;			// use this to update the gamebrain on which the next level will be
	public GameObject LoadingScreen;
	AsyncOperation ao = null;

	void Start()
	{
		LoadingScreen = GameBrain.Instance.loadingScreen;
	}

	void Update()
	{
		if (ao != null) 
		{
			if (ao.progress == 0.9f) 
			{
				ao.allowSceneActivation = true;
				LoadingScreen.SetActive(false);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			if(levelToLoad == "Tally Scene")
				GameBrain.Instance.SendMessage("ChangeMusic",GameBrain.Instance.GameOverMusic);
			GameBrain.Instance.RoomsCleared.Clear();	// empty rooms cleared in prep for next level
			GameBrain.Instance.SetLevel(nextLevel);// sets the gamebrain to know which level the player is now in
			GameBrain.Instance.Save();					// save game progress
			LoadingScreen.SetActive(true);
			LoadingScreen.GetComponentInChildren<Animator>().Play("Loading_Screen");
			ao = Application.LoadLevelAsync(levelToLoad);			// load this level
			ao.allowSceneActivation = false;
		}
	}
}
