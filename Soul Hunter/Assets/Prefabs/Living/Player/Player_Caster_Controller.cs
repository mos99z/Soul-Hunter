using UnityEngine;
using System.Collections;

public class Player_Caster_Controller : MonoBehaviour {

	private GameObject CurrSpell = null;
	private string SpellName;
	private float Recovering = 0.0f;
	private bool CanCast = true;
	public GameObject RecoveryBar = null;
	
	void Start ()
	{
		if (RecoveryBar == null) {
			Debug.LogError ("Assign \"Recovery Back\" from Main Camera to the Parameter RecoveryBar in the Prefab Player:Player_Caster_Controller.");
		}
	}

	void Update ()
	{
		if (!CanCast)
		{
			Recovering -= Time.deltaTime;
			
			if (Recovering <= 0.0f)
			{
				Recovering = 0.0f;
				CanCast = true;
			}
		} else if (Input.GetMouseButtonDown (0) && Pause_Script.gamePaused == false)
		{
			this.gameObject.GetComponent<Player_Movement_Controller>().underMelee = true;
			CastSpell();
		}
	}

	void CastSpell()
	{
		Instantiate(CurrSpell, transform.position, transform.rotation);
		GameBrain.Instance.SpellWasCast(CurrSpell);
	}

	public void ChangeSpell(GameObject _spell)
	{
		CurrSpell = _spell;
	}

	public void SetRecoverTime(float _recovering)
	{
		CanCast = false;
		Recovering = _recovering;

		RecoveryBar.SetActive (true);
		RecoveryBar.GetComponent<RecoveryBar>().SetCoolDown(_recovering);
	}
}
