using UnityEngine;
using System.Collections;

public class TutorialMinion : MonoBehaviour
{
	//Handle Minion's navigation
	NavMeshAgent navigation;
	Vector3 destination;
	float waypointTimer = 5.0f;
	bool moveOn;

	//Changes Minion's element
	float staleTimer = 0.0f;

	//Changes Minion's element
	int currEl;

	//Handle Minion's Color
	Color32[] elements = new Color32[5];
	Color32 success;
	Color32 failure;
	public Animator Animate = null;
	public GameObject sprt = null;
	private Living_Obj LvObj = null;
	public GameObject debuff = null;

	//get player's spell
	GameObject macSelect;
	MacroSelect macSel;

	//Tutorial progression
	private int progression;

	//Send Message to Tutorial
	public GameObject tutor;

	// Use this for initialization
	void Start ()
	{
		currEl = 0;
		progression = 0;

		moveOn = true;
		navigation = GetComponent<NavMeshAgent> ();
		destination = Random.insideUnitSphere * 7;
		destination.y = 0;
		elements[0] = new Color32 (255, 0, 0, 255);
		elements[1] = new Color32 (196, 196, 196, 255);
		elements[2] = new Color32 (96, 64, 32, 255);
		elements[3] = new Color32 (255, 196, 32, 255);
		elements[4] = new Color32 (64, 64, 255, 255);
		success = new Color32 (0, 255, 0, 255);
		failure = new Color32 (32, 32, 32, 255);
		LvObj = (Living_Obj)this.gameObject.GetComponent("Living_Obj");

		navigation.updateRotation = false;
		macSelect = GameBrain.Instance.HUDMaster;
		macSel = (MacroSelect)macSelect.GetComponent("MacroSelect");
	}

	void Update()
	{
		int numChildren = this.transform.childCount;
		if (numChildren > 2)
		{
			debuff = this.transform.GetChild(2).gameObject;
			if (debuff != null)
			{
				Destroy(debuff);
				debuff = null;
			}
		}
	}
	// Update is called once per frame
	// If there is no target, the enemy will move to a random waypoint in the navmesh in a patrol-like fashion
	// If there is a target, the enemy will rotate around the player and lunge in after a certain amount of time has passed
	void FixedUpdate () 
	{
		if (moveOn)
		{
			destination.x = Random.Range(-10, 10);
			destination.z = Random.Range(-10, 10);
		}

		waypointTimer -= Time.deltaTime;
		if(waypointTimer <= 0.0f)
		{
			navigation.SetDestination (destination);
			waypointTimer = Random.Range(2, 5);
			moveOn = true;
		}

		staleTimer -= Time.deltaTime;
		if (staleTimer <= 0.0f)
		{
			if (progression == 3)
			{
				++progression;
			}

			if (progression >= 4)
			{
				LvObj.CanDie = true;
				LvObj.SendMessage("TakeDamage", 1000);
			}
			currEl = Random.Range(0, 4);
			sprt.GetComponent<SpriteRenderer>().color = elements[currEl];
			switch (currEl)
			{
			case 0:
			{
				LvObj.ElementType = Element.Fire;
				break;
			}
			case 1:
			{
				LvObj.ElementType = Element.Wind;
				break;
			}
			case 2:
			{
				LvObj.ElementType = Element.Earth;
				break;
			}
			case 3:
			{
				LvObj.ElementType = Element.Lightning;
				break;
			}
			case 4:
			{
				LvObj.ElementType = Element.Water;
				break;
			}
			default:
				break;
			}
			staleTimer = Random.Range(5, 20);
		}

		Vector3 movementDirection = navigation.velocity.normalized;
		if (movementDirection.magnitude >= 1.0f)
		{
			float dotProd = Vector3.Dot (new Vector3 (0, 0, 1), movementDirection);
			Vector3 crossProd = Vector3.Cross (new Vector3 (0, 0, 1), movementDirection);
			if (dotProd >= 0.75f)
			{
				Animate.Play ("Tutor_Idl_Up");
			}
			else if (dotProd <= -0.75f)
			{
				Animate.Play ("Tutor_Idl_Down");
			}
			else if (dotProd > -0.25f && dotProd <= 0.25f)
			{
				if (crossProd.y < 0.0f)
					Animate.Play ("Tutor_Idl_Left");
				else
					Animate.Play ("Tutor_Idl_Right");
			}
			else if (dotProd > 0.25f && dotProd < 0.75f)
			{
				if (crossProd.y < 0.0f)
					Animate.Play ("Tutor_Idl_Up_Left");
				else
					Animate.Play ("Tutor_Idl_Up_Right");
			}
			else
			{
				if (crossProd.y < 0.0f)
					Animate.Play ("Tutor_Idl_Down_Left");
				else
					Animate.Play ("Tutor_Idl_Down_Right");
			}
		}
	}

	void TakeDamage (float _damage)
	{
		GUIText guiStuff = macSel.spells [macSel.curMac].GetComponent<GUIText> ();
		string wholeStr = guiStuff.text.ToString ();
		string[] bits = wholeStr.Split (',');
		int a = int.Parse(bits [1]);
		int b = int.Parse(bits [2]);
		int c = int.Parse(bits [3]);

		if (LvObj.ElementType == Element.None)
		{
			return;
		}

		switch (LvObj.ElementType)
		{
		case Element.Fire:
		{
			if ((a == 5 && b == 5) || (b == 5 && c == 5) || (a == 5 && c == 5))
			{
				sprt.GetComponent<SpriteRenderer>().color = success;
				progression++;
			}
			else
			{
				LvObj.GetComponent<SpriteRenderer>().color = failure;
			}
			staleTimer = 3;
			break;
		}
		case Element.Wind:
		{
			if ((a == 1 && b == 1) || (b == 1 && c == 1) || (a == 1 && c == 1))
			{
				sprt.GetComponent<SpriteRenderer>().color = success;
				progression++;
			}
			else
			{
				LvObj.GetComponent<SpriteRenderer>().color = failure;
			}
			staleTimer = 3;
			break;
		}
		case Element.Earth:
		{
			if ((a == 2 && b == 2) || (b == 2 && c == 2) || (a == 2 && c == 2))
			{
				sprt.GetComponent<SpriteRenderer>().color = success;
				progression++;
			}
			else
			{
				LvObj.GetComponent<SpriteRenderer>().color = failure;
			}
			staleTimer = 3;
			break;
		}
		case Element.Lightning:
		{
			if ((a == 3 && b == 3) || (b == 3 && c == 3) || (a == 3 && c == 3))
			{
				sprt.GetComponent<SpriteRenderer>().color = success;
				progression++;
			}
			else
			{
				LvObj.GetComponent<SpriteRenderer>().color = failure;
			}
			staleTimer = 3;
			break;
		}
		case Element.Water:
		{
			if ((a == 4 && b == 4) || (b == 4 && c == 4) || (a == 4 && c == 4))
			{
				sprt.GetComponent<SpriteRenderer>().color = success;
				progression++;
			}
			else
			{
				LvObj.GetComponent<SpriteRenderer>().color = failure;
			}
			staleTimer = 3;
			break;
		}
		default:
			break;
		}
		LvObj.ElementType = Element.None;
	}

	void OnDestroy()
	{
		tutor.GetComponent<Tutorial>().SetTask1(true);
	}
}
