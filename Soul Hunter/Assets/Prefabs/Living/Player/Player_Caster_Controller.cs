using UnityEngine;
using System.Collections;

public class Player_Caster_Controller : MonoBehaviour {

	private GameObject CurrSpell = null;
	private string SpellName;
	private float Recovering = 0.0f;
	private bool CanCast = true;
	public GameObject GameBrain = null;
	
	void Start ()
	{
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
	}
}
