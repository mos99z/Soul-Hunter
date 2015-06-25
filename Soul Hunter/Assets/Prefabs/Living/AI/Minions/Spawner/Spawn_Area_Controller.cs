using UnityEngine;
using System.Collections;

public class Spawn_Area_Controller : MonoBehaviour {

	public GameObject[] Spawners = null;
	public bool AreaContainsCaptain = false;
	public GameObject Captain;
	public int RoomNumber = 0;
	bool active = false;
	private GameObject HUDMast;

	//float captainTimer = 2.0f;

	// Use this for initialization
	void Start () 
	{
		HUDMast = GameBrain.Instance.HUDMaster;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (AreaContainsCaptain) 
		{
			if (Captain == null)
			{
				Destroy(gameObject);
			}
		}

		bool notNeeded = true;
		for (int i = 0; i < Spawners.Length; i++) {
			if(Spawners[i] != null)
			{
				notNeeded = false;
				break;
			}
		}

		if (notNeeded)
			Destroy (gameObject);
	}

	void OnTriggerEnter(Collider col)
	{
		if (!active && col.tag == "Player") 
		{
			if (RoomNumber != 0)
				GameBrain.Instance.RoomsCleared.Add(RoomNumber);
			active = true;
			for (int i = 0; i < Spawners.Length; i++)
			{
				Spawners[i].SetActive(true);
			}
			if (AreaContainsCaptain)
			{
				GameBrain.Instance.SendMessage("ChangeMusic",GameBrain.Instance.CaptainMusic);
				GameBrain.Instance.FightingCaptain = true;
				Captain.SetActive(true);
				HUDMast.SendMessage("ActivateCaptBar", 0);
			}
		}
	}

	// Making the game a bit harder
//	void OnTriggerExit(Collider col)
//	{
//		if (col.tag == "Player") 
//		{
//			for (int i = 0; i < Spawners.Length; i++)
//			{
//				if(Spawners[i] != null)
//					Spawners[i].SetActive(false);
//			}
//		}
//	}
}
