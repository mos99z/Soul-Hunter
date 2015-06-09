using UnityEngine;
using System.Collections;

public class Magma_Controller : MonoBehaviour 
{
	//public float Damage = 50.4f;
	//public float RecoveryCost = 1.0f;
	//public float StartHeight = 1.5f;
	//public float Speed = 1.5f;
	//public float Range = 10.0f;
	//public float MaxSpreadAngle = 0.0f;
	//public GameObject SpellEffect = null;
	//public GameObject ImpactEffect = null;
	//public GameObject Stunned = null;
	//public AudioSource SFXMoving = null;
	//public AudioSource SFXImpact = null;
	//public float StunDuration = 0.5f;
	//
	//private Vector3 StartLocation = Vector3.zero;
	//private Vector3 ForwardDirection = Vector3.zero;
	//private bool dieing = false;
	//private float killSwitch = 5.0f;


	public float damage = 5.1f;			// how much damage is done
	public float tickRate = 0.2f;		// how long to wait between dealing damage
	public float recoveryTime = 2.0f;	// how long for spell to recharge
	public float growSpeed = 2.0f;		// how long for spell to grow
	public float scaleValue = 4.0f;		// how much the magma circle will grow
	public GameObject mouseMarker;		// make this the mouse marker object in gamebrain
	
	public GameObject burning;			// debuff to apply
	public float burnChance = 0.9f;		// percent chance that debuff will apply
	public float burningDuration = 5.0f;// how long debuff lasts
	public float burningTick = 1.0f;	// how long to wait between dealing damage
	public float burningDamage = 5.1f;	// how much damage to deal each tick

	Vector3 origScale;			// keep a single scale variable for changing x and y at same time
	Vector3 finalScale;		// keep for proportional reference
	void Start () 
	{
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		transform.position = new Vector3( mouseMarker.transform.position.x, mouseMarker.transform.position.y + 0.1f, mouseMarker.transform.position.z);
		transform.Rotate(Vector3.right, 90.0f);
		origScale = transform.localScale;
		finalScale = new Vector3 (origScale.x * scaleValue, origScale.y * scaleValue, origScale.z * scaleValue);

		StartCoroutine("ScaleOverTime", growSpeed);
	}
	
	IEnumerator ScaleOverTime(float time)
	{
		float currentTime = 0.0f;
		
		do
		{
			transform.localScale = Vector3.Lerp(origScale, finalScale, currentTime / time);
			currentTime += Time.deltaTime;
			yield return null;
		} while (currentTime <= time);
		
		Destroy(gameObject);
	}
}
