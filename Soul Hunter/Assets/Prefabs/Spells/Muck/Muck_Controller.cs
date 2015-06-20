using UnityEngine;
using System.Collections;

public class Muck_Controller : MonoBehaviour 
{
	public GameObject mouseMarker;		// mouse marker from gameBrain
	public float spawnHeight = 0.05f;	// offset for y-axis
	public float duration = 4.0f;		// how long for spell to last
	public float recoveryCost = 2.0f;	// how long for spell to cooldown
	public float scaleShrink = 0.5f;	// how much to shrink spell

	Vector3 origScale;		// original scale
	Vector3 shrinkScale;	// shrink scale

	void Start () 
	{
		duration += (float)GameBrain.Instance.EarthLevel;


		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		Vector3 spawn = mouseMarker.transform.position;
		spawn.y = spawnHeight;
		transform.position = spawn;

		origScale = transform.localScale;
		shrinkScale = new Vector3(origScale.x * scaleShrink, origScale.y * scaleShrink, origScale.z * scaleShrink);

		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryCost);
		StartCoroutine("ShrinkOverTime", duration/2.0f);
	}

	void Update()
	{
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			GameObject slowDebuff = Instantiate(GameBrain.Instance.GetComponent<DebuffMasterList>().slowed);
			slowDebuff.transform.parent = other.transform;
			slowDebuff.transform.localPosition = Vector3.zero;
			slowDebuff.GetComponent<Slowed_Controller>().duration = 15.0f;
			slowDebuff.GetComponent<Slowed_Controller>().slowSpeedModifier = 0.5f + 0.05f * (float)GameBrain.Instance.WaterLevel;

			GameObject wetDebuff = Instantiate(GameBrain.Instance.GetComponent<DebuffMasterList>().wet);
			wetDebuff.transform.parent = other.transform;
			wetDebuff.transform.localPosition = Vector3.zero;
			wetDebuff.GetComponent<Wet_Controller>().Duration = 30.0f;
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
