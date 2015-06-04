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
	Color32 correct;
	public Material tutorMat;

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
		correct = new Color32 (0, 255, 0, 255);

		macSelect = GameBrain.Instance.HUDMaster;
		macSel = (MacroSelect)macSelect.GetComponent("MacroSelect");
	}
	
	// Update is called once per frame
	// If there is no target, the enemy will move to a random waypoint in the navmesh in a patrol-like fashion
	// If there is a target, the enemy will rotate around the player and lunge in after a certain amount of time has passed
	void FixedUpdate () 
	{
		if (progression == 3)
		{
			++progression;
			tutor.GetComponent<Tutorial>().SetTask1(true);
		}

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
			if (progression >= 4)
			{
				this.gameObject.SetActive(false);
			}
			currEl = Random.Range(0, 4);
			tutorMat.color = elements[currEl];
			staleTimer = Random.Range(5, 20);
		}
	}

	void TakeDamage (float _damage)
	{
		GUIText guiStuff = macSel.spells[macSel.curMac].GetComponent<GUIText>();
		string wholeStr = guiStuff.text.ToString();
		string[] bits = wholeStr.Split (',');
		int a = int.Parse (bits[1]);
		int b = int.Parse (bits[2]);
		int c = int.Parse (bits[3]);

		if (currEl == 5)
		{
			return;
		}

		if ((a == 1 && b == 1) || (a == 1 && c == 1) ||  (b == 1 && c == 1))
		{
			if (currEl == 1)
			{
				tutorMat.color = correct;
				staleTimer = 3;
				currEl = 5;
				++progression;
			}
			else
			{
				tutorMat.color = new Color32(32, 32, 32, 255);
				staleTimer = 3;
				currEl = 5;
			}
		}
		else if ((a == 2 && b == 2) || (a == 2 && c == 2) ||  (b == 2 && c == 2))
		{
			if (currEl == 2)
			{
				tutorMat.color = correct;
				staleTimer = 3;
				currEl = 5;
				++progression;
			}
			else
			{
				tutorMat.color = new Color32(32, 32, 32, 255);
				staleTimer = 3;
				currEl = 5;
			}
		}
		else if ((a == 3 && b == 3) || (a == 3 && c == 3) ||  (b == 3 && c == 3))
		{
			if (currEl == 3)
			{
				tutorMat.color = correct;
				staleTimer = 3;
				currEl = 5;
				++progression;
			}
			else
			{
				tutorMat.color = new Color32(32, 32, 32, 255);
				staleTimer = 3;
				currEl = 5;
			}
		}
		else if ((a == 4 && b == 4) || (a == 4 && c == 4) ||  (b == 4 && c == 4))
		{
			if (currEl == 4)
			{
				tutorMat.color = correct;
				staleTimer = 3;
				currEl = 5;
				++progression;
			}
			else
			{
				tutorMat.color = new Color32(32, 32, 32, 255);
				staleTimer = 3;
				currEl = 5;
			}
		}
		else if ((a == 5 && b == 5) || (a == 5 && c == 5) ||  (b == 5 && c == 5))
		{
			if (currEl == 0)
			{
				tutorMat.color = correct;
				staleTimer = 3;
				currEl = 5;
				++progression;
			}
			else
			{
				tutorMat.color = new Color32(32, 32, 32, 255);
				staleTimer = 3;
				currEl = 5;
			}
		}
	}
}
