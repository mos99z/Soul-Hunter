using UnityEngine;
using System.Collections;

public class EnemyDouse : MonoBehaviour
{
	private float douseTicker;
	private float damageTicker;
	public GameObject dousePart;
	public BoxCollider douseCol;
	private GameObject WetDebuff;
	
	// Use this for initialization
	void Start ()
	{
		dousePart.SetActive(true);
		douseCol.enabled = false;
		WetDebuff = GameBrain.Instance.GetComponent<DebuffMasterList>().wet;
	}
	
	void Update ()
	{
		douseTicker += Time.deltaTime;
		damageTicker += Time.deltaTime;
		if (damageTicker > 1)
		{
			douseCol.enabled = true;
			damageTicker = 0;
		}
		if (douseTicker > 7.5f)
		{
			Destroy(this.gameObject);
		}
	}
	
	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
		{
			col.SendMessage("TakeDamage", 5);
			float roll = Random.Range(0.0f, 1.0f);
			if (roll <= 0.5f)
			{
				GameObject debuff = Instantiate(WetDebuff);
				debuff.transform.parent = col.transform;
				debuff.transform.localPosition = Vector3.zero;
				debuff.GetComponent<Wet_Controller>().Duration = 10;
			}
			douseCol.enabled = false;
		}
	}
}
