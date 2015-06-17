using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Main_Menu_Script : MonoBehaviour
{
	//holds button info
	public Button[] buttons = new Button[5];

	//holds soul index sprites
	public GameObject[] SpellIndexs = new GameObject[5];

	//helper variables
	private int index;
	private bool needsUpdate;
	public bool stall;
	public GameObject MainMenu = null;
	public GameObject LoadingScreen;
	//other menus or popups
	public GameObject NewGamePrompt;
	public GameObject OptionsMenu;
	private Main_Menu_Script mainMenScript;
	AsyncOperation ao = null;
	
	// Use this for initialization
	void Start () 
	{
		LoadingScreen = GameBrain.Instance.loadingScreen;
		int zero = 0;
		GameBrain.Instance.SendMessage("ChangeMusic", zero);

		mainMenScript = (Main_Menu_Script)MainMenu.GetComponent("Main_Menu_Script");

		needsUpdate = false;

		index = 0;
		buttons[index].Select();
		SpellIndexs[index].SetActive(true);
		stall = false;
		//AudioSource[] sounds = GetComponents<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (ao == null) 
		{
			if (!stall) {
				if (Input.GetKeyDown (KeyCode.UpArrow)) {
					index--;
					if (index < 0)
						index = 4;
					needsUpdate = true;
				} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
					index++;
					if (index > 4)
						index = 0;
					needsUpdate = true;
				}
			}
			
			if (Input.GetKeyDown (KeyCode.Return)) {
				switch (index) {
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
			if (needsUpdate) {
				MoveSoul ();
				needsUpdate = false;
			}
		}

		else
		{
			Debug.Log(ao.progress);
			if (ao.progress == 0.9f) 
			{
				GameBrain.Instance.loadingScreen.SetActive(false);
				ao.allowSceneActivation = true;
			}
		}
	}

	private void MoveSoul()
	{
		for (int i = 0; i < SpellIndexs.Length; i++)
		{
			SpellIndexs[i].SetActive(false);
		}
		SpellIndexs[index].SetActive(true);
		buttons[index].Select();
	}

	public void SetIndex0()
	{
		if (!stall)
		{
			index = 0;
			needsUpdate = true;
		}
	}
	public void SetIndex1()
	{
		if (!stall)
		{
			index = 1;
			needsUpdate = true;
		}
	}
	public void SetIndex2()
	{
		if (!stall)
		{
			index = 2;
			needsUpdate = true;
		}
	}
	public void SetIndex3()
	{
		if (!stall)
		{
			index = 3;
			needsUpdate = true;
		}
	}
	public void SetIndex4()
	{
		if (!stall)
		{
			index = 4;
			needsUpdate = true;
		}
	}

	public void LoadTutorial()
	{
		//GameOver.SendMessage ("Reset");
		int zero = 0;
		GameObject.Find ("GameBrain").SendMessage ("SetLevel", zero);
		GameBrain.Instance.EraseFile();
		GameBrain.Instance.loadingScreen.SetActive(true);
		LoadingScreen.GetComponentInChildren<Animator>().Play("Loading_Screen");
		ao = Application.LoadLevelAsync ("Tutorial");
		ao.allowSceneActivation = false;
		GameBrain.Instance.SendMessage ("ChangeMusic", 1);
		
	}

	public void LoadLevel1()
	{
		GameObject.Find ("GameBrain").SendMessage ("SetLevel", 1);
//		DestroyImmediate (GameBrain.Instance.transform.FindChild ("Player").gameObject);
//		GameObject newPlayer = Instantiate (player);
//		newPlayer.transform.parent = GameBrain.Instance.transform;
//		newPlayer.transform.localPosition = Vector3.zero;
		GameBrain.Instance.EraseFile();
		LoadingScreen.SetActive(true);
		LoadingScreen.GetComponentInChildren<Animator>().Play("Loading_Screen");
		ao = Application.LoadLevelAsync("Level 1");
		ao.allowSceneActivation = false;
		GameBrain.Instance.SendMessage ("ChangeMusic", 1);
		
	}

	public void Cancel()
	{
		NewGamePrompt.SetActive (false);
		if (MainMenu != null)
		{
			for (int i = 0; i < mainMenScript.buttons.Length; i++)
			{
				mainMenScript.buttons[i].interactable = true;
				mainMenScript.stall = false;
			}
		}
	}

	public void MouseClick0()
	{
		NewGamePrompt.SetActive(true);
		index = 0;
		needsUpdate = true;
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].interactable = false;
		}
		stall = true;
	}

	public void MouseClick1()
	{
		//Debug.Log("Continue from autosave");
		GameBrain.Instance.Load();
		if (GameBrain.Instance.CurrentLevel == 0)
			LoadTutorial();
		else if (GameBrain.Instance.CurrentLevel > 0)
		{
			GameBrain.Instance.SendMessage ("ChangeMusic", 1);
			switch (GameBrain.Instance.CurrentLevel)
			{
			case 1: Application.LoadLevel("Level 1"); break;
			case 2: Application.LoadLevel("Level 2"); break;
			case 3: Application.LoadLevel("Level 3"); break;
			}
		}


		index = 1;
		needsUpdate = true;
	}

	public void MouseClick2()
	{
		OptionsMenu.SetActive (true);
		index = 2;
		needsUpdate = true;
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].interactable = false;
		}
		stall = true;
	}

	public void MouseClick3()
	{
		Application.LoadLevel("Credits");
		index = 3;
		needsUpdate = true;
	}
	public void MouseClick4()
	{
		Application.Quit();
		index = 4;
		needsUpdate = true;
	}

}    
