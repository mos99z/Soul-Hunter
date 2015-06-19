using UnityEngine;
using System.Collections;

public class DeathAn : MonoBehaviour
{
	public ParticleSystem outerShell;
	public ParticleSystem innerShell;
	public ParticleSystem rays;
	public GameObject innerSphere;


	private float minOSSize = 1.2f;
	private float minISSize = 1;
	private float minISize = 0;

	public float maxSize = 0;

	private float expanshion = 0;
	private float selfDestruct = 2;
	private bool die = false;

	// Use this for initialization
	void Start ()
	{
		Vector3 tempVec = new Vector3(minISize, minISize, minISize);
		innerSphere.transform.localScale = tempVec;
		outerShell.startSize = minOSSize;
		innerShell.startSize = minISSize;
		rays.enableEmission = false;
		if (maxSize < 4)
		{
			maxSize = 4;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (selfDestruct < 2)
		{
			expanshion -= Time.deltaTime * 0.5f;
		}
		else
		{
			expanshion += Time.deltaTime * 1.5f;
		}
		if (expanshion >= (maxSize - 3) && selfDestruct < 2)
		{
			rays.enableEmission = true;
		}
		if (expanshion > maxSize)
		{
			expanshion = 9;
			die = true;
		}
		if (die)
		{
			selfDestruct -= Time.deltaTime * 0.75f;
		}
		Vector3 tempVec = new Vector3(minISize + expanshion, minISize + expanshion, minISize + expanshion);
		innerSphere.transform.localScale = tempVec;
		outerShell.startSize = minOSSize + expanshion;
		innerShell.startSize = minOSSize + expanshion;
		if (selfDestruct <= 1f)
		{
			innerSphere.SetActive(false);
		}
		if (selfDestruct <= 0.75f)
		{
			outerShell.enableEmission = false;
		}
		if (selfDestruct <= 0.5f)
		{
			rays.enableEmission = false;
		}
		if (selfDestruct <= 0.25f)
		{
			innerShell.enableEmission = false;
		}
		if (selfDestruct <= 0)
		{
			Destroy(this.gameObject);
		}
	}
}
