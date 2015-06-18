using UnityEngine;
using System.Collections;

public class Torch_Controller : MonoBehaviour 
{

	public GameObject mouseMarker;		// mouse marker from game brain
	public float duration = 2.0f;		// how long for spell to last
	public float damage = 5.0f;			// how much damage to deal
	public float recoveryTime = 1.5f;	// how long to recover from spell

	public GameObject BurningDebuff = null;
	public float BurningDamage = 10.0f;
	public float DebuffChance = 0.9f;
	public float DebuffDuration = 10.0f;

	float timer = 0.0f;
	
	void Start () 
	{
		if (mouseMarker == null)
			mouseMarker = GameBrain.Instance.MouseMarker;
		transform.LookAt(mouseMarker.transform.position);
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);
		if (BurningDebuff == null) 
			BurningDebuff = GameBrain.Instance.GetComponent<DebuffMasterList>().burning;
	}
	
	void Update () 
	{
		transform.position = GameBrain.Instance.Player.transform.position;
		transform.LookAt(mouseMarker.transform.position);
		duration -= Time.deltaTime;
		if (duration <= 0.0f)
			Destroy (gameObject);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
			other.transform.SendMessage("TakeDamage", damage);
	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Enemy")
		{
			timer += Time.deltaTime;
			if (timer >= 0.5f)
			{
				timer = 0.0f;
				other.transform.SendMessage("TakeDamage", damage);

				float roll = Random.Range(0.0f, 1.0f);
				if (roll <= DebuffChance)
				{
					GameObject debuff = Instantiate(BurningDebuff);
					debuff.transform.parent = other.transform;
					debuff.transform.localPosition = Vector3.zero;
					debuff.GetComponent<Burning_Controller>().Duration = DebuffDuration;
					debuff.GetComponent<Burning_Controller>().Damage = BurningDamage;
				}
			}
		}
	}}
