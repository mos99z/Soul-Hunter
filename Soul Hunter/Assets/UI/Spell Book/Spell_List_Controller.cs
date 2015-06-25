using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class Spell_List_Controller : MonoBehaviour
{
	//Menu variables
	public GameObject SpellListMenu;

	//GameBrain Conection
	public GameBrain gameBrain;
	private bool[] spellCastBools;

	//helper variables
	private int selectedIndex = -1;
	private string[] spellNames = {"Barrier","Bolt","Bolt Chain","Concrete","Crystal Spikes","Explosion","Fire Ball","Fog","Freeze","Gravity Well","Hydrant",
		"Laser","Magma","Meteor","Muck","Plasma","Poison Cloud","Rock Spike","Sand Blast", "Shock Prism","Steam", "Tidal Wave", "Torch","Whirlwind","Wind Blade"};
	public Text NumSpellsDiscovered;
	private bool needsUpdate;

	//FilePath Stuff
	private string path;
	private string fullPath;
	private string[] elements = new string[6];

	//interactive text
	public Text[] spellSlots;

	//Display Text
	public Text spellName;
	public Text spellInfo;
	public Text spellCombo;

	//current method of holding data
//	public GameObject[] infoCards;

	// Use this for initialization
	void Start ()
	{
		spellCastBools = GameBrain.Instance.SpellHasBeenCast;
		path = "SpellInfo/";
		fullPath = path + "NoneSelected";
		elements[0] = "None";
		elements[1] = "Fire";
		elements[2] = "Wind";
		elements[3] = "Earth";
		elements[4] = "Electric";
		elements[5] = "Water";
		needsUpdate = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateHighlighted ();
		if (needsUpdate)
		{
			SetPath();
			ReadStuff();
			needsUpdate = false;
		}
	}

	public void UpdateSpellList()
	{
		spellCastBools = gameBrain.SpellHasBeenCast;
		
		for (int i = 0; i < spellCastBools.Length; i++) 
		{
			if(spellCastBools[i] == true)
			{
				spellSlots[i].text = spellNames[i];
			}
		}
	}

	public void UpdateHighlighted()
	{
		int spellCount = 0;
		for (int i = 0; i < spellSlots.Length; i++) 
		{
			spellSlots[i].color = Color.black;
			if(spellCastBools[i] == true)
				spellCount++;
		}
		if (selectedIndex != -1) 
		{
			spellSlots[selectedIndex].color = Color.yellow;
		}

		NumSpellsDiscovered.text = "Number of Spells Discovered: " + spellCount + "/25";
	}

	private void SetPath()
	{
		if (selectedIndex == -1)
		{
			fullPath = path + "NoneSelected.txt";
		}
		else
		{
			if (spellCastBools[selectedIndex])
			{
				fullPath = path + spellNames[selectedIndex] + ".txt";
			}
			else
			{
				fullPath = path + "Unknown.txt";
			}
		}
	}

	private void ReadStuff()
	{
		StreamReader inp_stm = new StreamReader(fullPath);

		string tempCombo = "";
		int temp1 = 0;
		int temp2 = 0;
		int temp3 = 0;

		string wholeFile = inp_stm.ReadToEnd();
		string[] fileChunks = wholeFile.Split(',');

		int.TryParse(fileChunks[0], out temp1);
		int.TryParse(fileChunks[1], out temp2);
		int.TryParse(fileChunks[2], out temp3);

		tempCombo = elements[temp1] + "+" + elements[temp2] + "+" + elements[temp3];

			spellCombo.text = "Element Combo:\n" + tempCombo;
		spellName.text = fileChunks[3];
		spellInfo.text = fileChunks[4];
		
		inp_stm.Close( );
	}

	public void MouseAway()
	{
		selectedIndex = -1;
		needsUpdate = true;
	}

	public void MouseOver0()
	{
		selectedIndex = 0;
		needsUpdate = true;
	}
	public void MouseOver1()
	{
		selectedIndex = 1;
		needsUpdate = true;
	}
	public void MouseOver2()
	{
		selectedIndex = 2;
		needsUpdate = true;
	}
	public void MouseOver3()
	{
		selectedIndex = 3;
		needsUpdate = true;
	}
	public void MouseOver4()
	{
		selectedIndex = 4;
		needsUpdate = true;
	}
	public void MouseOver5()
	{
		selectedIndex = 5;
		needsUpdate = true;
	}
	public void MouseOver6()
	{
		selectedIndex = 6;
		needsUpdate = true;
	}
	public void MouseOver7()
	{
		selectedIndex = 7;
		needsUpdate = true;
	}
	public void MouseOver8()
	{
		selectedIndex = 8;
		needsUpdate = true;
	}
	public void MouseOver9()
	{
		selectedIndex = 9;
		needsUpdate = true;
	}
	public void MouseOver10()
	{
		selectedIndex = 10;
		needsUpdate = true;
	}
	public void MouseOver11()
	{
		selectedIndex = 11;
		needsUpdate = true;
	}
	public void MouseOver12()
	{
		selectedIndex = 12;
		needsUpdate = true;
	}
	public void MouseOver13()
	{
		selectedIndex = 13;
		needsUpdate = true;
	}
	public void MouseOver14()
	{
		selectedIndex = 14;
		needsUpdate = true;
	}
	public void MouseOver15()
	{
		selectedIndex = 15;
		needsUpdate = true;
	}
	public void MouseOver16()
	{
		selectedIndex = 16;
		needsUpdate = true;
	}
	public void MouseOver17()
	{
		selectedIndex = 17;
		needsUpdate = true;
	}
	public void MouseOver18()
	{
		selectedIndex = 18;
		needsUpdate = true;
	}
	public void MouseOver19()
	{
		selectedIndex = 19;
		needsUpdate = true;
	}
	public void MouseOver20()
	{
		selectedIndex = 20;
		needsUpdate = true;
	}
	public void MouseOver21()
	{
		selectedIndex = 21;
		needsUpdate = true;
	}
	public void MouseOver22()
	{
		selectedIndex = 22;
		needsUpdate = true;
	}
	public void MouseOver23()
	{
		selectedIndex = 23;
		needsUpdate = true;
	}
	public void MouseOver24()
	{
		selectedIndex = 24;
		needsUpdate = true;
	}

	public void Close()
	{
		SpellListMenu.SetActive(false);
	}
}

