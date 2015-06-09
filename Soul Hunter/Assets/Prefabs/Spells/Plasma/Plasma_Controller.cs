using UnityEngine;
using System.Collections;

public class Plasma_Controller : MonoBehaviour 
{
	public float minDamage = 5.4f;		// range of damage
	public float maxDamage = 25.4f;
	public float recoveryCost = 1.5f;	// how long before player can cast again
	public float startHeight = 1.0f;	// where to move spell on y-axis for travel
	public float speed = 1.0f;			// how fast to move the plasma ball
	public float range = 5.0f;			// how far the plasma ball will travel
	public GameObject mouseMarker;		// pull from the gamebrain

	Vector3 startLoc = Vector3.zero;	// where the spell will spawn
	Vector3 direction = Vector3.zero;	// direction for spell to travel
	float killTimer = 4.0f;				// used to destroy object in case no collision happens

	void Start ()
	{
		Vector3 spawn = GameBrain.Instance.Player.transform.position;
		spawn.y = startHeight;
		transform.position = startLoc = spawn;

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
		killTimer -= Time.deltaTime;
		if (killTimer <= 0.0f)
			Destroy (gameObject);
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
			float damage = Random.Range((int)minDamage, (int)maxDamage);
			damage += minDamage % 1.0f;
			other.transform.SendMessage ("TakeDamage", damage);
			
			Destroy(gameObject);
		}
		else if (other.tag == "Solid")
			Destroy(gameObject);
	}
	
}
