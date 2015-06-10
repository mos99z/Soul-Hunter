using UnityEngine;
using System.Collections;

public class Muck_Controller : MonoBehaviour 
{
	public GameObject mouseMarker;		// mouse marker from gameBrain
	public float spawnHeight = 0.05f;	// offset for y-axis
	public float duration = 4.0f;		// how long for spell to last
	public float rotateSpeed = 135.0f;	// how much spin to give the spell
	public float recoveryCost = 2.0f;	// how long for spell to cooldown
	public float scaleShrink = 0.5f;	// how much to shrink spell
	
	public GameObject slow;						// slowed debuff
	public Element weakness = Element.Water;	// element to assign to affected enemies

	Vector3 origScale;		// original scale
	Vector3 shrinkScale;	// shrink scale

	void Start () 
	{
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		Vector3 spawn = mouseMarker.transform.position;
		spawn.y = spawnHeight;
		transform.position = spawn;
		transform.Rotate(Vector3.right, 270.0f);

		origScale = transform.localScale;
		shrinkScale = new Vector3(origScale.x * scaleShrink, origScale.y * scaleShrink, origScale.z * scaleShrink);

		if (slow == null)
			slow = GameBrain.Instance.GetComponent<DebuffMasterList>().slowed;

		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryCost);
		StartCoroutine("ShrinkOverTime", duration/2.0f);
	}

	void Update()
	{
		transform.RotateAround (transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			GameObject debuff = Instantiate(slow);
			debuff.transform.parent = other.transform;
			debuff.transform.localPosition = Vector3.zero;
			if (other.GetComponent<Living_Obj>().entType == Living_Obj.EntityType.Minion)
				other.GetComponent<Living_Obj>().ElementType = weakness;
		}
	}
	
	IEnumerator ShrinkOverTime(float time)
	{
		float currentTime = 0.0f;
		
		do
		{
			transform.localScale = Vector3.Lerp(origScale, shrinkScale, currentTime / time);
			currentTime += Time.deltaTime;
			yield return null;
		} while (currentTime <= time);

		StartCoroutine("GrowOverTime", duration/2.0f);
	}

	IEnumerator GrowOverTime(float time)
	{
		float currentTime = 0.0f;
		
		do
		{
			transform.localScale = Vector3.Lerp(shrinkScale, origScale, currentTime / time);
			currentTime += Time.deltaTime;
			yield return null;
		} while (currentTime <= time);
		
		Destroy(gameObject);
	}
}
