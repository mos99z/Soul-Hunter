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

	//other menus or popups
	public GameObject NewGamePrompt;
	public GameObject OptionsMenu;
	
	// Use this for initialization
	void Start () 
	{
		int zero = 0;
		GameBrain.Instance.SendMessage("ChangeMusic", zero);

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
		if (!stall)
		{
			if (Input.GetKeyDown (KeyCode.UpArrow)) 
			{
				index--;
				if (index < 0)
					index = 4;
				needsUpdate = true;
			}
			else if (Input.GetKeyDown (KeyCode.DownArrow))
			{
				index++;
				if (index > 4)
					index = 0;
				needsUpdate = true;
			}
		}

		if (Input.GetKeyDown (KeyCode.Return)) 
		{
			switch (index)
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
		if (needsUpdate)
		{
			MoveSoul();
			needsUpdate = false;
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
		Debug.Log("Continue from autosave");
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
