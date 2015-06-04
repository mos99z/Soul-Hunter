using UnityEngine;
using System.Collections;

public class RevampCoveredPitTrap : MonoBehaviour 
{
	public Sprite[] cracks;		// resize to number of crack sprites and fill with sprites
	public float crackTime;		// how long in seconds between adding more cracks

	int crackIndex;				// keep track of which crack to render
	float timer;				// keep track of when to change sprites
	//Sprite origSprite;			// keep track of original sprite, preserved if reset desired

	void Start () 
	{
		crackIndex = 0;
		timer = 0.0f;
		//origSprite = GetComponent<SpriteRenderer>().sprite;
		GetComponent<ParticleSystem>().Stop();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			GetComponent<SpriteRenderer>().sprite = cracks[crackIndex];
			GetComponent<ParticleSystem>().Play();
			crackIndex++;
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			timer += Time.deltaTime;
			if (timer >= crackTime)
			{
				timer = 0.0f;
				crackIndex++;
				if (crackIndex == cracks.Length)
				{
					Destroy(gameObject);
					return;
				}
				GetComponent<SpriteRenderer>().sprite = cracks[crackIndex];
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			timer = 0.0f;
			GetComponent<ParticleSystem>().Stop();
		}
	}
}
