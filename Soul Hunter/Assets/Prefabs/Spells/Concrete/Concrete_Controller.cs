using UnityEngine;
using System.Collections;

public class Concrete_Controller : MonoBehaviour 
{
	public GameObject mouseMarker;		// mouse marker from gameBrain
	public float spawnHeight = 0.05f;	// offset for y-axis
	public float duration = 4.0f;		// how long for spell to last
	public float rotateSpeed = 90.0f;	// how much spin to give the spell
	public float recoveryCost = 2.0f;	// how long for spell to cooldown

	public GameObject slow;				// slowed debuff

	void Start () 
	{
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		Vector3 spawn = mouseMarker.transform.position;
		spawn.y = spawnHeight;
		transform.position = spawn;
		transform.Rotate(Vector3.right, 270.0f);
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryCost);

		if (slow == null)
			slow = GameBrain.Instance.GetComponent<DebuffMasterList>().slowed;

		transform.localScale *= 1.0f + (float)GameBrain.Instance.EarthLevel / (float)GameBrain.Instance.NumberOfLevels;
	}
	
	void Update () 
	{
		transform.RotateAround(transform.position, Vector3.up, rotateSpeed * Time.deltaTime);

		duration -= Time.deltaTime;
		if (duration <= 0.0f)
			Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			GameObject debuff = Instantiate(slow);
			debuff.transform.parent = other.transform;
			debuff.transform.localPosition = Vector3.zero;
			debuff.GetComponent<Slowed_Controller>().fromConcrete = true;
		}
	}
}
