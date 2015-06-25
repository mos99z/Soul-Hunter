using UnityEngine;
using System.Collections;

public class Game_Over_Controller : MonoBehaviour {

	public GameObject player;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (player == null) 
		{
			if (Input.anyKey) 
			{
				gameObject.SetActive(false);
				GameBrain.Instance.SendMessage("ChangeMusic", GameBrain.Instance.MenuMusic);
				Application.LoadLevel ("Main menu");	
			}
		}
	}

	void Reset()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
	}
}
