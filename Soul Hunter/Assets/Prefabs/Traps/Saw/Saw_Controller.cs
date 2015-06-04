using UnityEngine;
using System.Collections;

public class Saw_Controller : MonoBehaviour 
{
	public float spinSpeed = 180.0f;		// how many degrees per second to rotate
	public float damage = 10.0f;			// how much damage is done each tick
	public float hurtDelay = 0.25f;			// how long to wait in seconds before dealing damage again
	public float cycleTime = 3.0f;			// how long to wait in seconds between going up and down
	public float hideSpeed = 2.0f;			// how fast the blade moves in and out of the floor

	float damageTimer;						// used for ticking damage
	float cycleTimer;						// used for cycling 
	Vector3 origPos, hidePos;				// used for convenience in moving saw up and down
	bool isCycling;							// used for knowing if saw is moving in or out of floor
	bool isDown;							// used for knowing if saw is in floor

	void Start () 
	{
		spinSpeed *= -1;		// negative to compensate for sprite
		damageTimer = cycleTimer = 0.0f;
		origPos = transform.position;
		hidePos = new Vector3 (transform.position.x, transform.position.y - 3.0f, transform.position.z);
		isCycling = isDown = false;
	}
	
	void Update () 
	{
		transform.Rotate (0.0f, 0.0f, spinSpeed * Time.deltaTime);

		if (!isCycling)
		{
			cycleTimer += Time.deltaTime;
			if (cycleTimer >= cycleTime)
			{
				cycleTimer = 0.0f;
				isCycling = true;

			}
		}
		else
		{
			if (!isDown)
			{
				transform.position = Vector3.MoveTowards(transform.position, hidePos, hideSpeed * Time.deltaTime);
				if (Mathf.Abs(Vector3.Distance(transform.position, hidePos)) <= 0.1f)
				{
					isDown = true;
					transform.position = hidePos;
					isCycling = false;
				}
			}
			else
			{
				transform.position = Vector3.MoveTowards(transform.position, origPos, hideSpeed * Time.deltaTime);
				if (Mathf.Abs(Vector3.Distance(transform.position, origPos)) <= 0.1f)
				{
					transform.position = origPos;
					isCycling = false;
					isDown = false;
				}
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			other.SendMessage("TakeDamage", damage);
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			damageTimer += Time.deltaTime;
			if (damageTimer >= hurtDelay)
			{
				damageTimer = 0.0f;
				other.SendMessage("TakeDamage", damage);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			damageTimer = 0.0f;
		}
	}
}
