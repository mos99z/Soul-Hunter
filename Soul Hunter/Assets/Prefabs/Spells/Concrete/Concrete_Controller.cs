using UnityEngine;
using System.Collections;

public class Concrete_Controller : MonoBehaviour 
{
	public GameObject mouseMarker;		// mouse marker from gameBrain
	public float spawnHeight = 0.05f;	// offset for y-axis
	public float duration = 4.0f;		// how long for spell to last
	public float rotateSpeed = 90.0f;	// how much spin to give the spell
	public float recoveryCost = 2.0f;	// how long for spell to cooldown

	void Start () 
	{
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		Vector3 spawn = mouseMarker.transform.position;
		spawn.y = spawnHeight;
		transform.position = spawn;
		transform.Rotate(Vector3.right, 270.0f);
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryCost);
	}
	
	void Update () 
	{
		transform.RotateAround(transform.position, Vector3.up, rotateSpeed * Time.deltaTime);

		duration -= Time.deltaTime;
		if (duration <= 0.0f)
			Destroy(gameObject);
	}
}
