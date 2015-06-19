using UnityEngine;
using System.Collections;

public class Moving_Wall_Controller : MonoBehaviour 
{

	public Vector3 newPosition;		// transform location for object to move towards
	public float speed;				// how fast to move between locations
	public float waitTime;			// how long to wait between moving
	public GameObject player;		// grab the player prefab to check for death
	public LayerMask wallLayer;		// only care about hitting walls

	float time;						// save the waitTime
//	float playerWidth;				// keep track of the width of player
	Vector3 origPosition;			// keep track of where to go back to
	bool isMoving = false;			// know if the wall is moving or not
	bool atNew = false;				// know which spot the wall is at
	bool isTouchingPlayer = false;	// know if the wall is touching player

	void Start () 
	{
		origPosition = transform.position;
		time = waitTime;
//		playerWidth = player.GetComponent<CapsuleCollider>().radius * 2.0f;
		if (player == null)
			Debug.Log ("Player MUST be set! Collisions will not kill player otherwise");
	}
	
	void Update () 
	{
		if (!isMoving)
		{
			waitTime -= Time.deltaTime;
			if (waitTime <= 0.0f)		// start moving and reset waitTime after timer is up
			{
				isMoving = true;
				waitTime = time;
			}
		}
		else
		{
			if (!atNew)
			{
				// move to the new spot if not there
				transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);
				if (Vector3.Distance(transform.position, newPosition) <= 0.1f)
				{
					transform.position = newPosition;
					atNew = true;
					isMoving = false;
				}
			}
			else
			{
				// move to the origPosition if at new
				transform.position = Vector3.MoveTowards(transform.position, origPosition, speed * Time.deltaTime);
				if (Vector3.Distance(transform.position, origPosition) <= 0.1f)
				{
					transform.position = origPosition;
					atNew = false;
					isMoving = false;
				}
			}
		}
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.collider.tag == "Solid")
		{
			newPosition = transform.position;
			if (isTouchingPlayer)
			{
				// do stuff maybe
			}
		}
	}

	void OnCollisionStay(Collision other)
	{
		if (other.collider.tag == "Player")
		{
			isTouchingPlayer = true;
			//foreach (ContactPoint contact in other)
			//{
			//	// TODO: Make this actually kill the player
			//	Ray wallCheck = new Ray(contact.point, contact.normal);
			//	float range = player.GetComponent<CapsuleCollider>().radius * 2.0f;
			//	RaycastHit rayInfo;
			//	if (Physics.Raycast(wallCheck, out rayInfo, playerWidth, wallLayer))
			//	{
			//		player.SendMessage("TakeDamage", (float)player.GetComponent<Living_Obj>().CurrHealth);
			//	}
			//}
		}
	}
}
