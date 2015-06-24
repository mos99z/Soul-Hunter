using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class MacroMenu : MonoBehaviour
{
	//helper variables
	private bool hasChanged;
	private bool reset;

	public Text ptotentialName;

	//Elemental graphic display of potential macro
	public Image potential1;
	public Image potential2;
	public Image potential3;

	//Index of currently editing slot
	public Image pIndex1;
	public Image pIndex2;
	public Image pIndex3;

	//Keeps track of what speels are being edited
	private int index;

	//Holds a numerical combination to search for
	private int[] fillSlot = new int[3];

	//Holds the five element Icons and an X
	public Sprite[] elements = new Sprite[6];

	//Holds acces to the five buttons
	public Button[] buttons = new Button[7];

	//Allows control over the tab and menu
	public GameObject tabObj;
	public GameObject menObj;

	//Allows altering of the Macro Selection script
	public GameObject HUDMaster;

	//Shut down spells
	public GameObject plyr;

	//Allows Macro Menu to send Macro Select the correct spell
	public GameObject spellDatabase;
	private GameObject potentialSpell;

	//Used to change color of the pIndecies
	private Color32 selected;
	private Color32 unSelected;

	//Puase when paused
	public bool paussed;

	// Use this for initialization
	void Start ()
	{
		hasChanged = false;
		paussed = false;

		resetButtons();
		
		selected = new Color ((float)0.75, (float)0.75, (float)0.75, (float)0.5);
		unSelected = new Color ((float)0.0, (float)0.0, (float)0.0, (float)0.0);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!paussed)
		{
			if (reset)
			{
				resetButtons();
				reset = false;
			}
			
			if (hasChanged)
			{
				updateIndex();
				hasChanged = false;
			}
			
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				activateMenu();
			}
			
			if (Input.GetKeyDown(KeyCode.Space) && index == 3 && menObj.activeSelf)
			{
				AddSpell();
			}
		}
	}

	public void cutOffSpells()
	{
		Player_Caster_Controller macSelect = (Player_Caster_Controller)plyr.GetComponent("Player_Caster_Controller");
		macSelect.enabled = false;
	}

	public void activateSpells()
	{
		Player_Caster_Controller macSelect = (Player_Caster_Controller)plyr.GetComponent("Player_Caster_Controller");
		macSelect.enabled = true;
	}

	private void activateMenu()
	{
		if (tabObj.activeSelf)
		{
			tabObj.SetActive(false);
			menObj.SetActive(true);
		}
		else if (menObj.activeSelf)
		{
			tabObj.SetActive(true);
			menObj.SetActive(false);
		}
	}

	public void resetButtons()
	{
		index = 0;

		potential1.sprite = elements[0];
		fillSlot[0] = 0;
		potential2.sprite = elements[0];
		fillSlot[1] = 0;
		potential3.sprite = elements[0];
		fillSlot[2] = 0;

		for (int i = 0; i < buttons.Length - 1; i++)
		{
			buttons[i].interactable = true;
		}
		buttons[5].interactable = false;
		buttons[6].interactable = false;

		ptotentialName.text = "???";

		hasChanged = true;
	}

	private void closeButtons()
	{
		for (int i = 0; i < buttons.Length - 1; i++)
		{
			buttons[i].interactable = false;
		}
		buttons[5].interactable = true;
	}

	private void updateIndex()
	{
		switch (index)
		{
		case 0:
		{
			pIndex1.color = selected;
			pIndex2.color = unSelected;
			pIndex3.color = unSelected;
			break;
		}
		case 1:
		{
			pIndex1.color = unSelected;
			pIndex2.color = selected;
			pIndex3.color = unSelected;
			break;
		}
		case 2:
		{
			pIndex1.color = unSelected;
			pIndex2.color = unSelected;
			pIndex3.color = selected;
			break;
		}
		case 3:
		{
			pIndex1.color = unSelected;
			pIndex2.color = unSelected;
			pIndex3.color = unSelected;
			break;
		}
		default:
			break;
		}
	}

	public void AddFire()
	{
		switch (index)
		{
		case 0:
		{
			potential1.sprite = elements[1];
			fillSlot[0] = 1;
			break;
		}
		case 1:
		{
			potential2.sprite = elements[1];
			fillSlot[1] = 1;
			break;
		}
		case 2:
		{
			potential3.sprite = elements[1];
			fillSlot[2] = 1;
			break;
		}
		default:
			break;
		}
		RightShift();
	}

	public void AddWind()
	{
		switch (index)
		{
		case 0:
		{
			potential1.sprite = elements[2];
			fillSlot[0] = 2;
			break;
		}
		case 1:
		{
			potential2.sprite = elements[2];
			fillSlot[1] = 2;
			break;
		}
		case 2:
		{
			potential3.sprite = elements[2];
			fillSlot[2] = 2;
			break;
		}
		default:
			break;
		}
		RightShift();
	}

	public void AddEarth()
	{
		switch (index)
		{
		case 0:
		{
			potential1.sprite = elements[3];
			fillSlot[0] = 3;
			break;
		}
		case 1:
		{
			potential2.sprite = elements[3];
			fillSlot[1] = 3;
			break;
		}
		case 2:
		{
			potential3.sprite = elements[3];
			fillSlot[2] = 3;
			break;
		}
		default:
			break;
		}
		RightShift();
	}

	public void AddElectric()
	{
		switch (index)
		{
		case 0:
		{
			potential1.sprite = elements[4];
			fillSlot[0] = 4;
			break;
		}
		case 1:
		{
			potential2.sprite = elements[4];
			fillSlot[1] = 4;
			break;
		}
		case 2:
		{
			potential3.sprite = elements[4];
			fillSlot[2] = 4;
			break;
		}
		default:
			break;
		}
		RightShift();
	}

	public void AddWater()
	{
		switch (index)
		{
		case 0:
		{
			potential1.sprite = elements[5];
			fillSlot[0] = 5;
			break;
		}
		case 1:
		{
			potential2.sprite = elements[5];
			fillSlot[1] = 5;
			break;
		}
		case 2:
		{
			potential3.sprite = elements[5];
			fillSlot[2] = 5;
			break;
		}
		default:
			break;
		}
		RightShift();
	}

	private void RightShift()
	{
		index++;

		switch (index)
		{
		case 1:
		{
			buttons[6].interactable = true;
			break;
		}
		case 2:
		{
			findAvailables();
			break;
		}
		case 3:
		{
			displayName();
			closeButtons();
			break;
		}
		default:
			break;
		}


		hasChanged = true;
	}

	private void displayName()
	{
		Array.Sort(fillSlot);
		
		string tempS = fillSlot[0].ToString() + "," + fillSlot[1].ToString() + "," + fillSlot[2].ToString();

		foreach (Transform child in spellDatabase.transform)
		{
			GUIText capture = child.GetComponent<GUIText>();
			String spellInfo = capture.text.ToString();
			string[] data = spellInfo.Split(',');
			String combo = data[1] + "," + data[2] + "," + data[3];
			
			if (tempS == combo)
			{
				potentialSpell = child.gameObject;
				ptotentialName.text = data[0];
				break;
			}
		}
	}

	public void AddSpell()
	{
		MacroSelect macSelect = (MacroSelect)HUDMaster.GetComponent("MacroSelect");
		macSelect.spells[macSelect.curMac] = potentialSpell;
		macSelect.needsUpdate = true;

		ptotentialName.text = "???";

		hasChanged = true;
		reset = true;
	}

	private void findAvailables()
	{
		int[] tempSlots = new int[3];

		for (int i = 0; i < fillSlot.Length; i++)
		{
			tempSlots[i] = fillSlot[i];
		}

		Array.Sort(tempSlots);

		switch (tempSlots[1])
		{
		case 1: //fire
		{
			switch (tempSlots[2])
			{
			case 1: //fire, fire
			{
				buttons[1].interactable = false;
				break;
			}
			case 2: //fire, wind
			{
				buttons[0].interactable = false;
				buttons[3].interactable = false;
				break;
			}
			case 3: //fire, earth
			{
				buttons[3].interactable = false;
				buttons[4].interactable = false;
				break;
			}
			case 4: //fire, electric
			{
				buttons[1].interactable = false;
				buttons[2].interactable = false;
				break;
			}
			case 5: //fire, water
			{
				buttons[2].interactable = false;
				buttons[4].interactable = false;
				break;
			}
			}
			break;
		}
		case 2: //wind
		{
			switch (tempSlots[2])
			{
			case 2: //wind, wind
			{
				buttons[2].interactable = false;
				break;
			}
			case 3: //wind, earth
			{
				buttons[1].interactable = false;
				buttons[4].interactable = false;
				break;
			}
			case 4: //wind, electric
			{
				buttons[0].interactable = false;
				buttons[4].interactable = false;
				break;
			}
			case 5: //wind, water
			{
				buttons[2].interactable = false;
				buttons[3].interactable = false;
				break;
			}
			}
			break;
		}
		case 3: //earth
		{
			switch (tempSlots[2])
			{
			case 3: //earth, earth
			{
				buttons[3].interactable = false;
				break;
			}
			case 4: //earth, electric
			{
				buttons[0].interactable = false;
				buttons[2].interactable = false;
				break;
			}
			case 5: //earth, electric
			{
				buttons[0].interactable = false;
				buttons[1].interactable = false;
				break;
			}
			}
			break;
		}
		case 4: //electric
		{
			switch (tempSlots[2])
			{
			case 4: //electric, electric
			{
				buttons[4].interactable = false;
				break;
			}
			case 5: //electric, water
			{
				buttons[1].interactable = false;
				buttons[3].interactable = false;
				break;
			}
			}
			break;
		}
		case 5: //water
		{
			switch (tempSlots[2])
			{
			case 5: //water, water
			{
				buttons[0].interactable = false;
				break;
			}
			}
			break;
		}
		}
	}
}
