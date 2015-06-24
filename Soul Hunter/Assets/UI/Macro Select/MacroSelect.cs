using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MacroSelect : MonoBehaviour
{
	//allow scrolling selecting
	private float scrollTime;
	public float scrollDelay;

	//numerical place holder for spells
	public int prevMac;
	public int curMac;
	public int nextMac;

	//Text display for spells
	public Text curSpell;
	public Text nextSpell;
	public Text prevSpell;

	//Macro Index graphics
	public Image slot1;
	public Image slot2;
	public Image slot3;
	public Image slot4;
	public Image slot5;

	//Elements Icons in Combo
	public Image element1;
	public Image element2;
	public Image element3;

	//Element Icons
	public Sprite[] elements = new Sprite[5];

	//Stored spells
	public GameObject[] spells = new GameObject[5];

	//Curr Mac indication Color
	private Color32 selected;
	private Color32 unSelected;

	//Store Player
	public GameObject Player = null;

	public bool needsUpdate;

	//Alow pussing
	public bool paussed;

	// Use this for initialization
	void Start ()
	{
		needsUpdate = false;
		paussed = false;

		scrollTime = 0;
		scrollDelay = (float)0.05;
		
		selected = new Color ((float)0.0, (float)1.0, (float)0.0, (float)0.75);
		unSelected = new Color ((float)0.5, (float)0.5, (float)0.5, (float)0.75);

		curMac = 0;
		nextMac = 1;
		prevMac = 4;
		slot1.color = selected;

		toggleMacs();
		colorManagment();
		changeText();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!paussed)
		{
			scrollTime += Time.deltaTime;
			
			//only do calculations when needed
			if (Input.GetAxis("Mouse ScrollWheel") != 0 || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q) || needsUpdate)
			{
				toggleMacs();
				
				colorManagment();
				
				changeText();
				
				needsUpdate = false;
			}
		}
	}

	private void toggleMacs()
	{
		float mouse = Input.GetAxis("Mouse ScrollWheel");
		
		if (Input.GetKeyDown(KeyCode.Q) || (mouse < 0 && scrollTime > scrollDelay))
		{
			scrollTime = 0;
			curMac--;
		}
		if (Input.GetKeyDown(KeyCode.E) || (mouse > 0 && scrollTime > scrollDelay))
		{
			scrollTime = 0;
			curMac++;
		}

		if (curMac < 0)
		{
			curMac = 4;
		}
		if (curMac > 4)
		{
			curMac = 0;
		}
		
		prevMac = curMac - 1;
		nextMac = curMac + 1;

		if (prevMac < 0)
		{
			prevMac = 4;
		}
		if (prevMac > 4)
		{
			prevMac = 0;
		}
		
		if (nextMac < 0)
		{
			nextMac = 4;
		}
		if (nextMac > 4)
		{
			nextMac = 0;
		}
	}
	
	private void colorManagment()
	{
		switch (curMac)
		{
		case 0:
		{
			slot1.color = selected;
			slot2.color = unSelected;
			slot3.color = unSelected;
			slot4.color = unSelected;
			slot5.color = unSelected;
			break;
		}
		case 1:
		{
			slot1.color = unSelected;
			slot2.color = selected;
			slot3.color = unSelected;
			slot4.color = unSelected;
			slot5.color = unSelected;
			break;
		}
		case 2:
		{
			slot1.color = unSelected;
			slot2.color = unSelected;
			slot3.color = selected;
			slot4.color = unSelected;
			slot5.color = unSelected;
			break;
		}
		case 3:
		{
			slot1.color = unSelected;
			slot2.color = unSelected;
			slot3.color = unSelected;
			slot4.color = selected;
			slot5.color = unSelected;
			break;
		}
		case 4:
		{
			slot1.color = unSelected;
			slot2.color = unSelected;
			slot3.color = unSelected;
			slot4.color = unSelected;
			slot5.color = selected;
			break;
		}
		default:
			break;
		}
	}

	private void changeText()
	{
		int temp;

		//Handle previous spell
		string info1 = spells[prevMac].GetComponent<GUIText> ().text.ToString();
		string[] data1 = info1.Split(',');
		prevSpell.text = data1[0];

		//Handle next spell
		string info2 = spells[nextMac].GetComponent<GUIText> ().text.ToString();
		string[] data2 = info2.Split(',');
		nextSpell.text = data2[0];

		//Handle current spell
		string info3 = spells[curMac].GetComponent<GUIText> ().text.ToString();
		string[] data3 = info3.Split(',');
		curSpell.text = data3[0];

		Player.SendMessage ("ChangeSpell", spells[curMac], SendMessageOptions.RequireReceiver);

		int.TryParse(data3[1], out temp);
		element1.sprite = elements[temp - 1];
		int.TryParse(data3[2], out temp);
		element2.sprite = elements[temp - 1];
		int.TryParse(data3[3], out temp);
		element3.sprite = elements[temp - 1];
	}
}
