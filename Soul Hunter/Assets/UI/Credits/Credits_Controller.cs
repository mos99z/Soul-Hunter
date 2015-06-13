using UnityEngine;
using System.Collections;

public class Credits_Controller : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.anyKey)
		{
			// save level as 0 to not break the save file
			GameBrain.Instance.CurrentLevel = 0;
			GameBrain.Instance.Save();
			// set to -1 to load the main menu
			GameBrain.Instance.CurrentLevel = -1;
			Application.LoadLevel ("Main menu");
		}
	}
}
