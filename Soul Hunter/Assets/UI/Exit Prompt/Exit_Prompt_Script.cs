using UnityEngine;
using System.Collections;

public class Exit_Prompt_Script : MonoBehaviour {

	public GameObject MessagePrompt;
	public GameObject PauseMenu;
	public GameObject LoadingScreen;
	AsyncOperation ao = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (ao != null) 
		{
			if (ao.progress == 0.9f) 
			{
				LoadingScreen.SetActive(false);
				ao.allowSceneActivation = true;
				GameBrain.Instance.SendMessage ("SetLevel", -1);
			}
		}
	}

	public void Exit_Message_Yes()
	{
		int zero = 0;
		LoadingScreen.SetActive(true);
		LoadingScreen.GetComponentInChildren<Animator>().Play("Loading_Screen");
		ao = Application.LoadLevelAsync ("Main menu");
		ao.allowSceneActivation = false;
		GameBrain.Instance.SendMessage ("ChangeMusic", zero);
		MessagePrompt.SetActive (false);
		PauseMenu.SetActive (false);
	}
	
	public void Exit_Message_No()
	{
		MessagePrompt.SetActive (false);
	}
}
