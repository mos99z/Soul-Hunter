using UnityEngine;
using System.Collections;

public class BindingMinionCheck : MonoBehaviour
{
	//need objects
	public GameObject BindCapt;
	private Binding_Captain_Controller BCC;

	//helper vars
	public int numMinions = 0;
	public int numMinionsAllowed = 5;

	//Resfresh Check
	private float refreshTicker;
	private SphereCollider MiniCheck;

	void Start ()
	{
		BCC = (Binding_Captain_Controller)BindCapt.GetComponent("Binding_Captain_Controller");
		refreshTicker = 0;
		MiniCheck = this.GetComponent<SphereCollider>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		refreshTicker -= Time.deltaTime;
		if (refreshTicker <= 0)
		{
			refreshTicker = 0.5f;
			numMinions = 0;
			MiniCheck.enabled = false;
			MiniCheck.enabled = true;
		}
		this.transform.position = GameBrain.Instance.Player.transform.position;
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
