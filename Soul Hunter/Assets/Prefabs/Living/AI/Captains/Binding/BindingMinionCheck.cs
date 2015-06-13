using UnityEngine;
using System.Collections;

public class BindingMinionCheck : MonoBehaviour
{
	//need objects
	public GameObject player;
	public GameObject BindCapt;
	private Binding_Captain_Controller BCC;

	//helper vars
	private int numMinions = 0;
	public int numMinionsAllowed = 5;

	void Start ()
	{
		BCC = (Binding_Captain_Controller)BindCapt.GetComponent("Binding_Captain_Controller");
	}
	
	// Update is called once per frame
	void Update ()
	{
		this.transform.position = player.transform.position;
		if (numMinions >= numMinionsAllowed)
		{
			BCC.isSurrounded = true;
		}
		else
		{
			BCC.isSurrounded = false;
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Enemy")
		{
			numMinions++;
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.tag == "Enemy")
		{
			numMinions--;
		}
	}
}
