using UnityEngine;
using System.Collections;

public class SpellCaster : MonoBehaviour {

	public GameObject SpellList = null;
	public GameObject ChargeBar = null;
	private GameObject CurrentSpell = null;
	private string SpellName;
	public float CoolDown = 0.0f;
	private float CoolDownTime = 0.0f;

	// Use this for initialization
	void Start () {
		//SpellList = GameObject.FindGameObjectWithTag ("Spells");
	}
	
	// Update is called once per frame
	void Update () {

		if (CoolDown > 0.0f) {
			ChargeBar.transform.Find("Charged").transform.localScale = new Vector3(1 - (CoolDown / CoolDownTime),1,1);
			CoolDown -= Time.deltaTime;

			if (CoolDown <= 0.0f){
				CoolDown = 0.0f;
				ChargeBar.SetActive(false);
			}
		}
	
		if (Input.GetMouseButtonDown(0) && CoolDown <= 0.0f) {
			Instantiate(CurrentSpell, transform.position, transform.rotation);
//			gameObject.GetComponent<AudioSource>().Play();
		}
	}

	public void SetCurrentSpellName(string _name){
		SpellName = _name;
		CurrentSpell = SpellList.transform.Find(SpellName).gameObject;
	}

	void SetCoolDown(float _cooldown){
		ChargeBar.SetActive(true);
		CoolDownTime = _cooldown;
		CoolDown = _cooldown;
	}
}
