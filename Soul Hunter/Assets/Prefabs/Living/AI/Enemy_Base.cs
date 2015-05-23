using UnityEngine;
using System.Collections;

public class Enemy_Base : MonoBehaviour {

	public int MaxHealth = 100;
	int currentHealth;
	public int Damage = 100;
	public int Defense = 0;

	// Use this for initialization
	void Start () 
	{
		currentHealth = MaxHealth;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Bob()
	{

	}

	// Enemy will die when this function gets called
	void Die()
	{

	}

}
