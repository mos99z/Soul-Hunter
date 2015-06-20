using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TidalWave_Controller : MonoBehaviour 
{
	public float duration = 4.0f;		// how long in seconds for spell to travel
	public float MoveSpeed = 0.25f;
	public float damage = 25.5f;			// how much damage to deal
	public float recoveryTime = 2.0f;	// how long for spell to cooldwon
	public float PushBackPower = 50.0f;
	List<GameObject> HitObjects = new List<GameObject> ();
	Vector3 Direction = Vector3.zero;
	float UpDown = 5.5f;
	bool once = true;
	bool HitWall = false;
	bool MovingUp = true;
	
	void Start () 
	{
		transform.position = GameBrain.Instance.Player.transform.position - new Vector3(0.0f, UpDown, 0.0f);
		Direction = GameBrain.Instance.Player.transform.FindChild ("Direction Indicator").forward.normalized;
		transform.forward = -Direction;
		GameBrain.Instance.Player.SendMessage("SetRecoverTime", recoveryTime);
		StartCoroutine("MoveUp", duration);
		StartCoroutine("MoveSpell", duration);
	}

	IEnumerator MoveUp(float time)
	{
		do{
			if (!MovingUp)
				break;
			transform.position += new Vector3(0, UpDown * Time.deltaTime, 0);
			yield return null;
		} while (transform.position.y < 0.0f);
		MovingUp = false;
	}

	IEnumerator MoveSpell(float time)
	{
		float currentTime = 0.0f;
		do{
			transform.position += Direction * MoveSpeed * Time.deltaTime;
			currentTime += Time.deltaTime;
			if (once && currentTime > time * 0.875f)
			{
				once = false;
				StartCoroutine("MoveDown");
			}
			yield return null;
		} while (currentTime < time);
	}

	IEnumerator MoveDown()
	{
		do{
			transform.position -= new Vector3(0, UpDown * Time.deltaTime, 0);
			yield return null;
		} while (transform.position.y > -UpDown);
		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider _obj)
	{
		if (_obj.tag == "Enemy") {
			if (!HitObjects.Contains (_obj.gameObject)) {
				if (_obj.GetComponent<Living_Obj> ().entType != Living_Obj.EntityType.Boss) {
					HitObjects.Add (_obj.gameObject);
					Vector3 crossProd = Vector3.Cross ((_obj.transform.position - transform.position).normalized, transform.forward.normalized);
					GameObject pushDirection = new GameObject ();
					pushDirection.transform.forward = transform.forward;
					Vector3 pushBack;
					if (crossProd.y < 0)
						pushDirection.transform.RotateAround (pushDirection.transform.position, new Vector3 (0, 1, 0), 90.0f);
					else
						pushDirection.transform.RotateAround (pushDirection.transform.position, new Vector3 (0, 1, 0), -90.0f);
					pushBack = pushDirection.transform.forward.normalized * PushBackPower;

					if (_obj.GetComponent<Living_Obj> ().entType == Living_Obj.EntityType.Captain)
						pushBack *= 0.5f;

					_obj.transform.GetComponent<Rigidbody> ().velocity = pushBack;
				}
				float roll = Random.Range(0.0f, 1.0f);
				if (roll <= 0.9f)
				{
					GameObject WetDebuff = Instantiate(GameBrain.Instance.GetComponent<DebuffMasterList>().wet);
					WetDebuff.transform.parent = _obj.transform;
					WetDebuff.transform.localPosition = Vector3.zero;
					WetDebuff.GetComponent<Wet_Controller>().Duration = 20.0f;
				}
				_obj.transform.SendMessage ("TakeDamage", damage);
			}
		}
		else if (!HitWall && _obj.name.Contains("Wall"))
		{
			HitWall = true;
			once = false;
			MovingUp = false;
			transform.GetComponent<BoxCollider>().enabled = false;
			StartCoroutine("MoveDown");
		}
	}
}
