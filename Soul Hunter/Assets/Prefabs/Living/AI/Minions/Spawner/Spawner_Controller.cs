using UnityEngine;
using System.Collections;

public class Spawner_Controller : MonoBehaviour {

	public GameObject EnemyToSpawn = null;
	public int NumberOfSpawns = 10;
	public float SpawnDelay = 5.0f;
	float numSpawned = 0.0f;

	float currentTimer = 0.0f;
	int numSpawns;


	// Use this for initialization
	void Start ()
	{
		currentTimer = SpawnDelay;
		numSpawns = NumberOfSpawns;
	}
	
	// Update is called once per frame
	void Update () 
	{
		currentTimer -= Time.deltaTime;

		if (currentTimer <= 0.0f) 
		{
			Instantiate (EnemyToSpawn, gameObject.transform.position, gameObject.transform.rotation);
			NumberOfSpawns--;
			numSpawned++;
			currentTimer = SpawnDelay + ((SpawnDelay * numSpawned) / numSpawns);
		}

		if (NumberOfSpawns == 0) 
		{
			Destroy (gameObject);
		}
	}
}
