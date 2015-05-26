using UnityEngine;
using System.Collections;

public class Exit_Prompt_Script : MonoBehaviour {

	public GameObject MessagePrompt;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Exit_Message_Yes()
	{
		Debug.Log ("Return to Main Menu");
		MessagePrompt.SetActive (false);
	}
	
	public void Exit_Message_No()
	{
		MessagePrompt.SetActive (false);
	}
}
