using UnityEngine;
using System.Collections;

public class RecoveryBar : MonoBehaviour
{
	public GameObject RecoverBar = null;

	public float CoolDown = 0.0f;
	public float CoolDownTime = 1.0f;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (CoolDown > 0.0f)
		{
			RecoverBar.transform.localScale = new Vector3(1 - (CoolDown/CoolDownTime),1,1f);
			CoolDown -= Time.deltaTime;
			
			if (CoolDown <= 0.0f)
			{
				CoolDown = 0.0f;
				this.gameObject.SetActive(false);
			}
		}
	}

	void SetCoolDown(float _cooldown)
	{
		RecoverBar.transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
		this.gameObject.SetActive(true);
		CoolDownTime = _cooldown;
		CoolDown = _cooldown;
	}
}
