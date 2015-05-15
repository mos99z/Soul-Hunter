using UnityEngine;
using System.Collections;

public class FireDebuffController : MonoBehaviour {

	public float TickCycle = 0.5f;
	public int Damage = 10;
	private float timmer = 0.0f;
	private bool checkOnce = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (checkOnce && transform.parent != null) {
			if(transform.parent.transform.FindChild("Wet(Clone)")){
				Destroy(transform.parent.transform.FindChild("Wet(Clone)").gameObject);
				Destroy(gameObject);
				return;
			}else if (transform.parent.transform.FindChild("Burning(Clone)") 
			          && transform.parent.transform.FindChild("Burning(Clone)").gameObject != gameObject){
				Destroy(transform.parent.transform.FindChild("Burning(Clone)").gameObject);
			}
			checkOnce = false;
		}

		timmer += Time.deltaTime;
		if (timmer >= TickCycle) {
			timmer = 0.0f;
			transform.parent.SendMessage("TakeDamage", Damage, SendMessageOptions.DontRequireReceiver);
		}
	}
}