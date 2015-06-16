using UnityEngine;
using System.Collections;

public class ExploreBug : MonoBehaviour
{
	private NavMeshAgent navigation;
	public Transform[] rooms;
	public int currentRoom = 0;

	// Use this for initialization
	void Start ()
	{
		navigation = GetComponent<NavMeshAgent>();
		navigation.updateRotation = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (currentRoom >= rooms.Length)
		{
			Destroy(this.gameObject);
			return;
		}
		navigation.SetDestination(rooms[currentRoom].position);
		Vector3 tempVec = this.transform.position - rooms[currentRoom].position;
		if (tempVec.magnitude <= 0.3f)
		{
			currentRoom++;
		}
	}
}
