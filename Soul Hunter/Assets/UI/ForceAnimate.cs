using UnityEngine;
using System.Collections;

public class ForceAnimate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		this.gameObject.GetComponent<Animator>().StartPlayback();
	}
}
