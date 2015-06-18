using UnityEngine;
using System.Collections;

public class Fog_Event_Manager : MonoBehaviour 
{

	public delegate void FogEvent();
	public static event FogEvent PlayerEntered;
	public static event FogEvent PlayerLeft;

	void  PlayerEnteredFog()
	{
		if(PlayerEntered != null)
			PlayerEntered();
	}

	void PlayerLeftFog()
	{
		if(PlayerLeft != null)
			PlayerLeft();
	}
}
