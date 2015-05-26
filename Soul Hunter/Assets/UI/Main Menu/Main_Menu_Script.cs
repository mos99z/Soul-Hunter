using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Main_Menu_Script : MonoBehaviour {

	public Text NewGame;
	public Text Continue;
	public Text Options;
	public Text Credits;
	public Text ExitGame;

	public GameObject NewGamePrompt;

	int selectedIndex = 0;

	RectTransform newGameArea;
	RectTransform continueArea;
	RectTransform optionsArea;
	RectTransform creditsArea;
	RectTransform exitGameArea;
	
	// Use this for initialization
	void Start () 
	{
		newGameArea = NewGame.rectTransform;
		continueArea = Continue.rectTransform;
		optionsArea = Options.rectTransform;
		creditsArea = Credits.rectTransform;
		exitGameArea = ExitGame.rectTransform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Mouse Input
		if(newGameArea.rect.Contains(Input.mousePosition))
		{
			selectedIndex = 0;
		}

		else if(continueArea.rect.Contains(Input.mousePosition))
		{
			selectedIndex = 1;
		}

		else if(optionsArea.rect.Contains(Input.mousePosition))
		{
			selectedIndex = 2;
		}

		else if(creditsArea.rect.Contains(Input.mousePosition))
		{
			selectedIndex = 3;
		}

		else if(exitGameArea.rect.Contains(Input.mousePosition))
		{
			selectedIndex = 4;
		}

		// Keyboard Input
		if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			selectedIndex--;
			if(selectedIndex < 0)
				selectedIndex = 4;
		}

		else if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			selectedIndex++;
			if(selectedIndex > 4)
				selectedIndex = 0;
		}

		UpdateHighlighted ();

		// Mouse Selection
		if(newGameArea.rect.Contains(Input.mousePosition) && Input.GetMouseButtonDown(0))
		{
			NewGamePrompt.SetActive(true);
		}
		
		else if(continueArea.rect.Contains(Input.mousePosition) && Input.GetMouseButtonDown(0))
		{
			Debug.Log("Continue from autosave");
		}
		
		else if(optionsArea.rect.Contains(Input.mousePosition) && Input.GetMouseButtonDown(0))
		{
			Debug.Log("Load Options Menu Here");
		}
		
		else if(creditsArea.rect.Contains(Input.mousePosition) && Input.GetMouseButtonDown(0))
		{
			Application.LoadLevel("Credits");
		}
		
		else if(exitGameArea.rect.Contains(Input.mousePosition) && Input.GetMouseButtonDown(0))
		{
			Application.Quit();
		}
	}

	void UpdateHighlighted()
	{
		NewGame.color = Color.black;
		Continue.color = Color.black;
		Options.color = Color.black;
		Credits.color = Color.black;
		ExitGame.color = Color.black;

		switch (selectedIndex) 
		{
		case 0:
		{
			NewGame.color = Color.yellow;
			break;
		}
		case 1:
		{
			Continue.color = Color.yellow;
			break;
		}
		case 2:
		{
			Options.color = Color.yellow;
			break;
		}
		case 3:
		{
			Credits.color = Color.yellow;
			break;
		}
		case 4:
		{
			ExitGame.color = Color.yellow;
			break;
		}
		}
	}

	public void LoadTutorial()
	{
		Application.LoadLevel ("Tutorial");
	}

	public void LoadLevel1()
	{
			Application.LoadLevel("Level 1");
	}

}    
