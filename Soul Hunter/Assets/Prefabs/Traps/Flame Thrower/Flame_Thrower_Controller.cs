using UnityEngine;
using System.Collections;

public class Flame_Thrower_Controller : MonoBehaviour 
{
	public GameObject fire;		// assign the particle system and it's collision box
	public float startDelay;	// set these different in multiple objects to offset start time
	public float burnTime;		// how long to burn for
	public float pauseTime;		// how long to wait before turning on again

	float timer;				// increment this to compare against other times
	bool isBurning;				// is the fire on
	bool wait;					// used if there is a start delay					
	
	void Start () 
	{
		timer = 0.0f;
		isBurning = false;
		if (startDelay > 0.0f)
			wait = true;
		else
			wait = false;
	}
	
	void Update () 
	{
		if (wait) 
		{
			// wait if needed
			timer += Time.deltaTime;
			if (timer >= startDelay)
			{
				timer = 0.0f;
				wait = false;
			}
		}
		else
		{
			if (isBurning) 
			{
				// burn until duration is up
				timer += Time.deltaTime;
				if (timer >= burnTime) 
				{
					isBurning = false;
					timer = 0.0f;
					fire.SetActive(false);
				}
			}
			else 
			{
				// remain off until duration is up
				timer += Time.deltaTime;
				if (timer >= pauseTime) 
				{
					isBurning = true;
					timer = 0.0f;
					fire.SetActive(true);
				}
			}
		}
	}
}
