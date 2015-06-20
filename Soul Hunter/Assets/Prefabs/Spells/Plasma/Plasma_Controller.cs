using UnityEngine;
using System.Collections;

public class Plasma_Controller : MonoBehaviour 
{
	public float damage = 25.4f;		// how much damage to deal
	public float recoveryCost = 1.5f;	// how long before player can cast again
	public float startHeight = 1.0f;	// where to move spell on y-axis for travel
	public float speed = 0.2f;			// how fast to move the plasma ball
	public float range = 10.0f;			// how far the plasma ball will travel
	public GameObject mouseMarker;		// pull from the gamebrain

	Vector3 startLoc = Vector3.zero;	// where the spell will spawn
	Vector3 direction = Vector3.zero;	// direction for spell to travel

	void Start ()
	{
		speed += 0.01f * GameBrain.Instance.FireLevel < GameBrain.Instance.ElectricLevel ? (float)GameBrain.Instance.FireLevel : (float)GameBrain.Instance.ElectricLevel;
		// set the appropriate spawn point
		Vector3 spawn = GameBrain.Instance.Player.transform.position;
		spawn.y = startHeight;
		transform.position = startLoc = spawn;

		// set the appropriate target location
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		Vector3 target = mouseMarker.transform.position;
		target.y = startHeight;
		transform.LookAt(target);
		direction = transform.forward.normalized * speed;

		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryCost);
	}
	
	void Update ()
	{

	}
	
	void FixedUpdate ()
	{
		transform.position += direction;
		float distance = (transform.position - startLoc).magnitude;
		
		if (distance >= range)
			Destroy (gameObject);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			other.transform.SendMessage ("TakeDamage", damage);
			
			Destroy(gameObject);
		}
		else if (other.tag == "Solid")
			Destroy(gameObject);
	}
	
}
