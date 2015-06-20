using UnityEngine;
using System.Collections;

public class LookatPlayer : MonoBehaviour
{
	private GameObject player;
	// Use this for initialization
	void Start ()
	{
		player = GameBrain.Instance.Player;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 tempVec = player.transform.position - this.transform.position;
		if (tempVec.magnitude >= 1)
		{
			this.transform.LookAt(player.transform.position, new Vector3(0,1,0));
		}
	}
}
