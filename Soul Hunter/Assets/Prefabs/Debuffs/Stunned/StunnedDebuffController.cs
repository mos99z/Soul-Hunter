using UnityEngine;
using System.Collections;

public class StunnedDebuffController : MonoBehaviour {

	public Vector3 LockPosition = Vector3.zero;
	public MonoBehaviour[] Scripts;
	public float Duration = 0.5f;

	private float timeAlive = 0.0f;
	private bool checkOnce = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (checkOnce) {
			if(transform.parent.transform.FindChild("Stunned(Clone)")
			   && transform.parent.transform.FindChild("Stunned(Clone)").gameObject != gameObject){
				Destroy(transform.parent.transform.FindChild("Stunned(Clone)").gameObject);
			}
			for (int i = 0; i < Scripts.Length; i++) {
				Scripts[i].enabled = false;
			}
			checkOnce = false;
		}

		timeAlive += Time.deltaTime;
		if (timeAlive >= Duration) {
			for (int i = 0; i < Scripts.Length; i++) {
				Scripts[i].enabled = true;
			}
			Destroy(gameObject);
		}

		transform.parent.transform.position = LockPosition;
	}
}
