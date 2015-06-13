using UnityEngine;
using System.Collections;

public class EnemyWhirlWind : MonoBehaviour
{
	public GameObject player;
	private float whirlWindTicker;
	private float damageTicker;
	private bool startDamage;
	public GameObject whirlPart;
	public SphereCollider whirlCol;
	private int speedUp = 1;

	// Use this for initialization
	void Start ()
	{
		whirlPart.SetActive(true);
		whirlPart.GetComponent<ParticleSystem>().startLifetime = 0.5f;
		whirlCol.enabled = false;
		startDamage = false;
		player = GameBrain.Instance.Player;
	}
	
	// Update is called once per frame
	void Update ()
	{
		whirlWindTicker += Time.deltaTime;
		this.transform.Rotate(Vector3.forward, Time.deltaTime * 90 * speedUp);
		if (whirlWindTicker > 2)
		{
			whirlPart.GetComponent<ParticleSystem>().startLifetime = 2;
			whirlPart.GetComponent<ParticleSystem>().startColor = new Color32(240, 200, 160, 255);
			startDamage = true;
			speedUp = 2;
		}
		if (startDamage)
		{
			damageTicker += Time.deltaTime;
			if (damageTicker > 1)
			{
				whirlCol.enabled = true;
				damageTicker = 0;
			}
			Vector3 tempVec = player.transform.position - this.gameObject.transform.position;
			if (tempVec.magnitude < 7)
			{
				tempVec = new Vector3(tempVec.x * -1000, 0, tempVec.z * -1000);
				player.GetComponent<Rigidbody>().AddForce(tempVec);
			}
			if (whirlWindTicker > 5)
			{
				Destroy(this.gameObject);
			}
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") 
		{
			col.SendMessage("TakeDamage", 50);
			whirlCol.enabled = false;
		}
	}
}
