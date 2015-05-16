using UnityEngine;
using System.Collections;

public class Player_Caster_Controller : MonoBehaviour {

	private GameObject CurrSpell = null;
	private string SpellName;
	private float Recovering = 0.0f;
	private float RecoverTime = 0.0f;
	private bool CanCast = true;
	
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
		}
	}

	void CastSpell()
	{
		if (CanCast)
		{
			Instantiate(CurrSpell, transform.position, transform.rotation);
		}
	}

	void ChangeSpell(GameObject _spell)
	{
		CurrSpell = _spell;
	}
}
