using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Spell_List_Controller : MonoBehaviour {

	public GameObject SpellListMenu;
	public GameBrain gameBrain;
	int selectedIndex = -1;
	public Text[] spellSlots; 
	public GameObject[] infoCards;
	string[] spellNames = {"Aqua Jet","Barrier","Bolt","Bolt Chain","Concrete","Crystal Spikes","Explosion","Fire Ball","Fog","Freeze","Gravity Well","Hydrant",
		"Laser","Magma","Meteor","Muck","Plasma","Poison Cloud","Rock Spike","Sand Blast", "Shock Prism","Steam", "Torch","Whirlwind","Wind Blade"};
	public Text NumSpellsDiscovered;
	bool[] spellCastBools;

	// Use this for initialization
	void Start ()
	{
		spellCastBools = gameBrain.SpellHasBeenCast;
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateHighlighted ();
	}

	public void UpdateSpellList()
	{
		bool[] spellCastBools = gameBrain.SpellHasBeenCast;
		
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
			infoCards[i].SetActive(false);
			if(spellCastBools[i] == true)
				spellCount++;
		}
		if (selectedIndex != -1) 
		{
			spellSlots [selectedIndex].color = Color.yellow;
			if(spellCastBools[selectedIndex] == true)
				infoCards[selectedIndex].SetActive(true);
		}

		NumSpellsDiscovered.text = "Number of Spells Discovered: " + spellCount + "/25";
	}

	public void MouseAway()
	{
		selectedIndex = -1;
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

	public void MouseOver5()
	{
		selectedIndex = 5;
	}
	
	public void MouseOver6()
	{
		selectedIndex = 6;
	}
	
	public void MouseOver7()
	{
		selectedIndex = 7;
	}
	
	public void MouseOver8()
	{
		selectedIndex = 8;
	}
	
	public void MouseOver9()
	{
		selectedIndex = 9;
	}

	public void MouseOver10()
	{
		selectedIndex = 10;
	}
	
	public void MouseOver11()
	{
		selectedIndex = 11;
	}
	
	public void MouseOver12()
	{
		selectedIndex = 12;
	}
	
	public void MouseOver13()
	{
		selectedIndex = 13;
	}
	
	public void MouseOver14()
	{
		selectedIndex = 14;
	}

	public void MouseOver15()
	{
		selectedIndex = 15;
	}
	
	public void MouseOver16()
	{
		selectedIndex = 16;
	}
	
	public void MouseOver17()
	{
		selectedIndex = 17;
	}
	
	public void MouseOver18()
	{
		selectedIndex = 18;
	}
	
	public void MouseOver19()
	{
		selectedIndex = 19;
	}

	public void MouseOver20()
	{
		selectedIndex = 20;
	}
	
	public void MouseOver21()
	{
		selectedIndex = 21;
	}
	
	public void MouseOver22()
	{
		selectedIndex = 22;
	}
	
	public void MouseOver23()
	{
		selectedIndex = 23;
	}
	
	public void MouseOver24()
	{
		selectedIndex = 24;
	}

	void UpdateInformation()
	{

	}

	public void Close()
	{
		SpellListMenu.SetActive (false);
	}
}

