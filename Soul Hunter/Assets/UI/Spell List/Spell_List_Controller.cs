using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Spell_List_Controller : MonoBehaviour {

	public GameObject SpellListMenu;
	public GameBrain gameBrain;
	public Text[] spellSlots; 
	string[] spellNames = {"Aqua Jet","Barrier","Bolt","Bolt Chain","Concrete","Crystal Spikes","Explosion","Fire Ball","Fog","Freeze","Gravity Well","Hydrant",
		"Laser","Magma","Meteor","Muck","Plasma","Poison Cloud","Rock Spike","Sand Blast", "Shock Prism","Steam", "Torch","Whirlwind","Wind Blade"};

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update () 
	{

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

	public void Close()
	{
		SpellListMenu.SetActive (false);
	}
}

