using UnityEngine;
using System.Collections;

public class SummonWall : MonoBehaviour 
{
	public GameObject[] walls;		// resize to how many walls will be summoned
	public float yPos = -2.0f;	// for where to move wall when "inactive"
	void Start () 
	{
		float tempY = walls[0].transform.position.y;
		for (int i = 0; i < walls.Length; i++)
		{
			if(walls[i] == null)
				continue;
			Vector3 pos = walls[i].transform.position;
			pos.y = yPos;
			walls[i].transform.position = pos;
		}
		yPos = tempY;
	}
	
	void ActivateWalls() 
	{
		for (int i = 0; i < walls.Length; i++)
		{
			if(walls[i] == null)
				continue;
			Vector3 pos = walls[i].transform.position;
			pos.y = yPos;
			walls[i].transform.position = pos;
		}
	}

	public void DestroyWalls()
	{
		for (int i = 0; i < walls.Length; i++)
		{
			if(walls[i] == null)
				continue;
			Destroy (walls [i]);
		}
	}
}
