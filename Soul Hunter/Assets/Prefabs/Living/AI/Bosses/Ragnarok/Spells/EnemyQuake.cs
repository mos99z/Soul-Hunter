using UnityEngine;
using System.Collections;

public class EnemyQuake : MonoBehaviour
{
	private float quakeTicker;
	private float damageTicker;
	private bool startDamage;
	public GameObject quakePart;
	public SphereCollider quakeCol;

	// Use this for initialization
	void Start ()
	{
		quakePart.SetActive(true);
		quakePart.GetComponent<ParticleSystem>().startSize = 0.5f;
		quakePart.GetComponent<ParticleSystem>().startLifetime = 0.5f;
		quakePart.GetComponent<ParticleSystem>().startSpeed = 1;
		quakeCol.enabled = false;
		startDamage = false;
	}

	void Update ()
	{
		quakeTicker += Time.deltaTime;
		if (quakeTicker > 2)
		{
			quakePart.GetComponent<ParticleSystem>().startSize = 1;
			quakePart.GetComponent<ParticleSystem>().startSpeed = 2;
			quakePart.GetComponent<ParticleSystem>().startColor = new Color32(122,96, 0, 255);
			startDamage = true;
		}
		if (startDamage)
		{
			damageTicker += Time.deltaTime;
			if (damageTicker > 1)
			{
				quakeCol.enabled = true;
				damageTicker = 0;
			}
			if (quakeTicker > 5)
			{
				Destroy(this.gameObject);
			}
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") 
		{
			col.SendMessage("TakeDamage", 100);
			quakeCol.enabled = false;
		}
	}
}
