using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour
{
	//tutorial diplay items
	public GameObject TutorialBack1;
	public GameObject TutorialBack2;
	public GameObject Tutorial2;
	public GameObject Tutorial3;
	public GameObject Tutorial4;
	public GameObject Tutorial5;
	public GameObject Tutorial6;
	public GameObject Tutorial7;
	public GameObject Tutorial8;

	//tutorial items
	public GameObject MouseTrigger;
	public GameObject PlayerTrigger;
	public GameObject DummieMinion;

	//tutorial progression
	int progression;
	bool activatedTxt;
	bool task1;
	bool task2;
	public GameObject macMen;
	float taskTicker;
	float taskTime;

	// Use this for initialization
	void Start ()
	{
		progression = 0;
		activatedTxt = false;
		task1 = false;
		task2 = false;
		taskTicker = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			progression++;
			MouseTrigger.SetActive(false);
			PlayerTrigger.SetActive(false);
			DummieMinion.SetActive(false);
			TutorialBack1.SetActive(false);
			TutorialBack2.SetActive(false);
			Tutorial2.SetActive(false);
			Tutorial3.SetActive(false);
			Tutorial4.SetActive(false);
			Tutorial5.SetActive(false);
			Tutorial6.SetActive(false);
			Tutorial7.SetActive(false);
			Tutorial8.SetActive(false);
			activatedTxt = false;
			task1 = false;
			task2 = false;
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			MouseTrigger.SetActive(false);
			PlayerTrigger.SetActive(false);
			DummieMinion.SetActive(false);
			TutorialBack1.SetActive(false);
			TutorialBack2.SetActive(false);
			Tutorial2.SetActive(false);
			Tutorial3.SetActive(false);
			Tutorial4.SetActive(false);
			Tutorial5.SetActive(false);
			Tutorial6.SetActive(false);
			Tutorial7.SetActive(false);
			Tutorial8.SetActive(false);
		}

		switch (progression)
		{
		case 0:
		{
			part0();
			break;
		}
		case 1:
		{
			part1();
			break;
		}
		case 2:
		{
			part2();
			break;
		}
		case 3:
		{
			part3();
			break;
		}
		case 4:
		{
			part4();
			break;
		}
		case 5:
		{
			part5();
			break;
		}
		case 6:
		{
			part6();
			break;
		}
		case 7:
		{
			part7();
			break;
		}
		default:
			break;
		}
	}

	void part0()
	{
		if (!activatedTxt)
		{
			TutorialBack1.SetActive(true);
			activatedTxt = true;
		}
	}

	void part1()
	{
		if (!activatedTxt)
		{
			TutorialBack2.SetActive(true);
			Tutorial2.SetActive(true);
			MouseTrigger.SetActive(true);
			PlayerTrigger.SetActive(true);
			activatedTxt = true;
		}
		if (task1 && task2)
		{
			++progression;
			TutorialBack2.SetActive(false);
			Tutorial2.SetActive(false);
			MouseTrigger.SetActive(false);
			PlayerTrigger.SetActive(false);
			activatedTxt = false;
			task1 = task2 = false;
		}
	}

	void part2()
	{
		if (!activatedTxt)
		{
			TutorialBack2.SetActive(true);
			Tutorial3.SetActive(true);
			activatedTxt = true;
		}
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			++progression;
			TutorialBack2.SetActive(false);
			Tutorial3.SetActive(false);
			activatedTxt = false;
		}
	}

	void part3()
	{
		if (!activatedTxt)
		{
			TutorialBack2.SetActive(true);
			Tutorial4.SetActive(true);
			activatedTxt = true;
		}
		if (Input.GetAxis("Mouse ScrollWheel") != 0 || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
		{
			++progression;
			TutorialBack2.SetActive(false);
			Tutorial4.SetActive(false);
			activatedTxt = false;
		}
	}

	void part4()
	{
		if (!activatedTxt)
		{
			TutorialBack2.SetActive(true);
			Tutorial5.SetActive(true);
			activatedTxt = true;
		}
		if (macMen.activeSelf)
		{
			++progression;
			TutorialBack2.SetActive(false);
			Tutorial5.SetActive(false);
			activatedTxt = false;
		}
	}

	void part5()
	{
		if (!activatedTxt)
		{
			TutorialBack2.SetActive(true);
			Tutorial6.SetActive(true);
			activatedTxt = true;
		}
	}

	void part6()
	{
		if (!activatedTxt)
		{
			TutorialBack2.SetActive(true);
			Tutorial7.SetActive(true);
			activatedTxt = true;
			DummieMinion.SetActive(true);
		}
		if (task1)
		{
			++progression;
			TutorialBack2.SetActive(false);
			Tutorial7.SetActive(false);
			activatedTxt = false;
			task1 = false;
		}
	}

	void part7()
	{
		taskTicker -= Time.deltaTime;
		if (!activatedTxt)
		{
			TutorialBack2.SetActive(true);
			Tutorial8.SetActive(true);
			activatedTxt = true;
			taskTicker = 3;
		}
		if (taskTicker <= 0)
		{
			++progression;
			TutorialBack2.SetActive(false);
			Tutorial8.SetActive(false);
			activatedTxt = false;
			taskTicker = 0;
		}
	}

	public void SetTask1(bool _comp)
	{
		task1 = _comp;
	}

	public void SetTask2(bool _comp)
	{
		task2 = _comp;
	}
}
