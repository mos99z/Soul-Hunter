using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Damage_Message : MonoBehaviour
{
	public string FunctionName = "TakeDamage";
	[Header ("Tag must contain this word")]
	public string TargetTag = "Enemy";
	public int InitialDamage = 0;
	public AudioSource DamageSFXPlayer = null;

	public bool ApplyDebuff = false;

	public Debuff DebuffType = Debuff.NONE;
	[Header ("Range between 0 and 1")]
	public float DebuffChance = 0.0f;
	public float DebuffDuration = 0.0f;
	public float DebuffTickRate = 0.0f;
	[Header ("Don't forget elemental damage type, 0.1 - 0.5")]
	public float DebuffTickDamage = 0.0f;

	void OnTriggerEnter(Collider _object){
		if (TargetTag.Length > 0 && _object.tag.Contains(TargetTag))
		{
			if (ApplyDebuff)
			{
				float roll = Random.Range(0.0f, 1.0f);
				if (roll <= DebuffChance)
				{
					switch (DebuffType)
					{
					case Debuff.Burning:
						GameObject onFire = Instantiate(GameBrain.Instance.GetComponent<DebuffMasterList>().burning);
						onFire.transform.parent = _object.transform;
						onFire.transform.localPosition = new Vector3(0,0,0);
						onFire.GetComponent<Burning_Controller>().Duration = DebuffDuration;
						onFire.GetComponent<Burning_Controller>().Damage = DebuffTickDamage;
						onFire.GetComponent<Burning_Controller>().TickCycle = DebuffTickRate;
						break;

				case Debuff.Crippled:
						
						break;

				case Debuff.Slowed:
						
						break;

				case Debuff.Stunned:
						
						break;

				case Debuff.Wet:
						GameObject wet = Instantiate(GameBrain.Instance.GetComponent<DebuffMasterList>().wet);
						wet.transform.parent = _object.transform;
						wet.transform.localPosition = new Vector3(0,0,0);
						wet.GetComponent<Wet_Controller>().Duration = DebuffDuration;
						break;

				 default:
					 break;
					 }
				}
			}

			_object.SendMessage (FunctionName, InitialDamage);

			if (DamageSFXPlayer != null)
			{
				DamageSFXPlayer.Play();
			}
		}
	}
}