using UnityEngine;
using System.Collections;

public class WaterDebuffController : MonoBehaviour {

	private bool checkOnce = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (checkOnce && transform.parent != null) {
			if(transform.parent.transform.FindChild("Burning(Clone)")){
				Destroy(transform.parent.transform.FindChild("Burning(Clone)").gameObject);
				Destroy(gameObject);
				return;
			}else if (transform.parent.transform.FindChild("Wet(Clone)") 
			          && transform.parent.transform.FindChild("Wet(Clone)").gameObject != gameObject){
				Destroy(transform.parent.transform.FindChild("Wet(Clone)").gameObject);
			}
			checkOnce = false;
		}
	}
}
