using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Damage_Message : MonoBehaviour
{
	public int Damage = 100;
	public bool ApplyDebuff = false;
	public float Duration = 0.0f;
	public string FunctionName = "TakeDamage";
	public string CollidableTag = "Enemy";
	public bool IsTagReciever = true;
	public string AlternateReciever;
	public Debuff DebuffType = Debuff.NONE;
	public GameObject Debuffs = null;

	// Use this for initialization
	void Start (){

	}
	
	// Update is called once per frame
	void Update (){

	}

	void OnTriggerEnter(Collider _object){
        if (CollidableTag.Length > 0 && _object.tag == CollidableTag){
			if (IsTagReciever){
				if (ApplyDebuff){
					switch (DebuffType)
					{
					case Debuff.Burning:
						GameObject onFire = Instantiate(Debuffs.transform.Find("Burning").gameObject);
						onFire.transform.parent = _object.transform;
						onFire.transform.localPosition = new Vector3(0,0,0);
						Destroy(onFire, Duration);
						break;

					case Debuff.Crippled:
						
						break;

					case Debuff.Slowed:
						
						break;

					case Debuff.Stunned:
						
						break;

					case Debuff.Wet:
						GameObject wet = Instantiate(Debuffs.transform.Find("Wet").gameObject);
						wet.transform.parent = _object.transform;
						wet.transform.localPosition = new Vector3(0,0,0);
						Destroy(wet, Duration);
						break;

					 default:
					 break;
					 }
				}
				_object.SendMessage (FunctionName, Damage, SendMessageOptions.DontRequireReceiver);
			} else if (AlternateReciever != null){
				if (ApplyDebuff){
					GameObject onFire = Instantiate(Debuffs.transform.Find("Burning").gameObject);
					onFire.transform.parent = _object.transform;
					onFire.transform.localPosition = new Vector3(0,1,0);
					Destroy(onFire, Duration);
				}

				GameObject.FindGameObjectWithTag(AlternateReciever).SendMessage (FunctionName, Damage, SendMessageOptions.DontRequireReceiver);
			}
			if (GetComponent<AudioSource>()){
				GetComponent<AudioSource>().Play();
			}
		}
	}

	void OnTriggerExit(Collider _object)
	{
        
	}
}