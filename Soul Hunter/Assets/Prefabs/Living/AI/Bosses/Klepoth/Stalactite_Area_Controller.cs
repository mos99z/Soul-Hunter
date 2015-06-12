using UnityEngine;
using System.Collections;

public class Stalactite_Area_Controller : MonoBehaviour {

	public int NumStalactites = 10;
	public float Duration = 10.0f;
	public GameObject Stalactite = null;
	private BoxCollider SpawnArea = null;
	private int Spawned = 0;
	private float TimePassed = 0.0f;
	// Use this for initialization
	void Start () 
	{
		SpawnArea = GetComponent<BoxCollider> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		
		TimePassed += Time.deltaTime;
		
		if (TimePassed > Duration / (float)NumStalactites) 
		{
			TimePassed = 0.0f;
			Vector3 SpawnPoint = Vector3.zero;
			SpawnPoint.y = SpawnArea.transform.position.y;
			SpawnPoint.x = Random.Range(SpawnArea.transform.position.x - SpawnArea.size.x * 0.5f, SpawnArea.transform.position.x + SpawnArea.size.x * 0.5f);
			SpawnPoint.z = Random.Range(SpawnArea.transform.position.z - SpawnArea.size.z * 0.5f, SpawnArea.transform.position.z + SpawnArea.size.z * 0.5f);
			
			Instantiate(Stalactite, SpawnPoint, Quaternion.identity);
			
			Spawned++;
			if (Spawned >= NumStalactites)
			{
				Spawned = 0;
				this.enabled = false;
			}
		}
	}
}
