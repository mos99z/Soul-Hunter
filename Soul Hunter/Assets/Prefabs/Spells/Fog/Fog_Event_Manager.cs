using UnityEngine;
using System.Collections;

public class Fog_Event_Manager : MonoBehaviour {

	public delegate void FogEvent();
	public static event FogEvent PlayerEntered;
	public static event FogEvent PlayerLeft;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
