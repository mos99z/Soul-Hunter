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
	public GameObject OptionsMenu;
	public GameObject GameOver;
//	public GameObject player = null;

	int selectedIndex = 0;
	
	// Use this for initialization
	void Start () 
	{
		int zero = 0;
		GameBrain.Instance.SendMessage ("ChangeMusic", zero);

		//AudioSource[] sounds = GetComponents<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Keyboard Input
		if (Input.GetKeyDown (KeyCode.UpArrow)) 
		{
			selectedIndex--;
			if (selectedIndex < 0)
				selectedIndex = 4;
		} 

		else if (Input.GetKeyDown (KeyCode.DownArrow))
		{
			selectedIndex++;
			if (selectedIndex > 4)
				selectedIndex = 0;
		}

		else if (Input.GetKeyDown (KeyCode.Return)) 
		{
			switch (selectedIndex) 
			{
			case 0:
			{
				MouseClick0 ();
				break;
			}
			case 1:
			{
				MouseClick1 ();
				break;
			}
			case 2:
			{
				MouseClick2 ();
				break;
			}
			case 3:
			{
				MouseClick3 ();
				break;
			}
			case 4:
			{
				MouseClick4 ();
				break;
			}
			}
		}

		UpdateHighlighted ();
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
		//GameOver.SendMessage ("Reset");
		int zero = 0;
		GameObject.Find ("GameBrain").SendMessage ("SetLevel", zero);
		Application.LoadLevel ("TempTutorial");
		GameBrain.Instance.SendMessage ("ChangeMusic", 1);
		
	}

	public void LoadLevel1()
	{
		GameObject.Find ("GameBrain").SendMessage ("SetLevel", 1);
//		DestroyImmediate (GameBrain.Instance.transform.FindChild ("Player").gameObject);
//		GameObject newPlayer = Instantiate (player);
//		newPlayer.transform.parent = GameBrain.Instance.transform;
//		newPlayer.transform.localPosition = Vector3.zero;
		Application.LoadLevel("Level 1");
		GameBrain.Instance.SendMessage ("ChangeMusic", 1);
		
	}

	public void Cancel()
	{
		NewGamePrompt.SetActive (false);
	}

	public void MouseOver0()
	{
		selectedIndex = 0;
	}

	public void MouseOver1()
	{
		selectedIndex = 1;
	}

	public void MouseOver2()
	{
		selectedIndex = 2;
	}

	public void MouseOver3()
	{
		selectedIndex = 3;
	}

	public void MouseOver4()
	{
		selectedIndex = 4;
	}

	public void MouseClick0()
	{
		NewGamePrompt.SetActive(true);
	}

	public void MouseClick1()
	{
		Debug.Log("Continue from autosave");
	}

	public void MouseClick2()
	{
		OptionsMenu.SetActive (true);
	}

	public void MouseClick3()
	{
		Application.LoadLevel("Credits");
	}

	public void MouseClick4()
	{
		Application.Quit();
	}

}    
