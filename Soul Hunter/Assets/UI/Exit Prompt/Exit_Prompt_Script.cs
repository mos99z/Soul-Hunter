using UnityEngine;
using System.Collections;

public class Exit_Prompt_Script : MonoBehaviour {

	public GameObject MessagePrompt;
	public GameObject PauseMenu;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Exit_Message_Yes()
	{
		int zero = 0;
		GameBrain.Instance.SendMessage ("SetLevel", -1);
		Application.LoadLevel ("Main menu");
		GameBrain.Instance.SendMessage ("ChangeMusic", zero);
		MessagePrompt.SetActive (false);
		PauseMenu.SetActive (false);
	}
	
	public void Exit_Message_No()
	{
		MessagePrompt.SetActive (false);
	}
}
