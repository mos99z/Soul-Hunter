using UnityEngine;
using System.Collections;

public class Credits_Controller : MonoBehaviour {

	bool phaseOne = false;
	bool phaseTwo = false;
	bool phaseThree = false;
	public GameObject[] PhaseOneObjects;
	public GameObject[] PhaseTwoObjects;
	public GameObject[] PhaseThreeObjects;
	public float TimeUntilPhase3 = 3.0f;
	float currentTimer = 0.0f;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		currentTimer += Time.deltaTime;
		if (phaseOne == false && currentTimer >= (TimeUntilPhase3 * 0.33f)) 
		{
			phaseOne = true;
			for (int i = 0; i < PhaseOneObjects.Length; i++) 
			{
				PhaseOneObjects[i].SetActive(true);
			}
		}

		if (phaseTwo == false && currentTimer >= (TimeUntilPhase3 * 0.66f)) 
		{
			phaseTwo = true;
			for (int i = 0; i < PhaseTwoObjects.Length; i++) 
			{
				PhaseTwoObjects[i].SetActive(true);
			}
		}

		if (phaseThree == false && currentTimer >= TimeUntilPhase3) 
		{
			phaseThree = true;
			for (int i = 0; i < PhaseThreeObjects.Length; i++) 
			{
				PhaseThreeObjects[i].SetActive(true);
			}
		}
		if (phaseThree && Input.anyKey)
		{
			// save level as 0 to not break the save file
			GameBrain.Instance.CurrentLevel = 0;
			GameBrain.Instance.Save();
			// set to -1 to load the main menu
			GameBrain.Instance.CurrentLevel = -1;
			Application.LoadLevel ("Main menu");
		}
	}
}
