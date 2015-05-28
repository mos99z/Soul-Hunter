using UnityEngine;
using System.Collections;

public class Player_Caster_Controller : MonoBehaviour {

	private GameObject CurrSpell = null;
	private string SpellName;
	private float Recovering = 0.0f;
	private bool CanCast = true;
	public GameObject GameBrain = null;
	public GameObject RecoveryBar = null;
	
	void Start ()
	{
		if (RecoveryBar == null)
		{
			Debug.Log("To Reduce CPU Load Assign \"Recovery Back\" from Main Camera to the Parameter RecoveryBar in the Prefab Player:Player_Caster_Controller.");
			RecoveryBar = GameObject.Find("Main Camera/RecoveryBack");
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
		} else if (Input.GetMouseButtonDown (0))
		{
			CastSpell();
		}
	}

	void CastSpell()
	{
		Instantiate(CurrSpell, transform.position, transform.rotation);
		GameBrain.SendMessage ("SpellWasCast", CurrSpell);
		if (GameBrain != null)
			GameBrain.BroadcastMessage ("SpellCasted", CurrSpell.name, SendMessageOptions.RequireReceiver);
		else
			Debug.LogError ("GameBrain in script Player Caster Controller can not be null!");
	}

	void ChangeSpell(GameObject _spell)
	{
		CurrSpell = _spell;
	}

	void SetRecoverTime(float _recovering)
	{
		if (_recovering > 0.0f)
			CanCast = false;
		Recovering = _recovering;

		RecoveryBar.SetActive (true);
		RecoveryBar.SendMessage ("SetCoolDown", _recovering);
	}
}
