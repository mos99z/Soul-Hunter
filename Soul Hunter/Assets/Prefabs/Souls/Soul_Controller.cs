using UnityEngine;
using System.Collections;

public class Soul_Controller : MonoBehaviour {

	public int SoulValue = 100;
	public float AliveTime = 10.0f;
	public string CollectorTag = "Player";
	public float MoveSpeed = 0.3f;
	private float currentSpeed = 0.0f;
	private GameObject target = null;
	public GameObject SFXCollectSoul = null;
	private bool once = true;

	void Start ()
	{
	}

	void FixedUpdate ()
	{

	if (target != null)
		{
			Vector3 targetLoc = target.transform.position;
			targetLoc.y += 1.5f;
			Vector3 moveDirection = targetLoc - transform.position;
			float distance = moveDirection.magnitude;
			moveDirection.Normalize();
			moveDirection *= currentSpeed;
			transform.position += moveDirection;
			if (distance <= 2.25f && once)
			{
				once = false;
				//GameObject gameBrain = GameObject.Find("GameBrain");
				GameBrain.Instance.SendMessage("ModSouls", SoulValue, SendMessageOptions.RequireReceiver);
				if (SFXCollectSoul != null) {
					GameObject soundeffect = Instantiate(SFXCollectSoul);
					soundeffect.transform.parent = GameObject.FindWithTag("Player").transform;
					soundeffect.transform.localPosition = new Vector3(0,0,0);
					float killtimer = 2.0f;
					if (soundeffect.GetComponent<AudioSource>().clip != null)
						killtimer = soundeffect.GetComponent<AudioSource>().clip.length;
					Destroy(soundeffect, killtimer);
				}
			}
			else if (distance <= 0.25f)
				Destroy(gameObject);
		}
	}

	void Update(){
		if (target == null) {
			AliveTime -= Time.deltaTime;
			if (AliveTime <= 0.0f)
				Destroy (gameObject);
		} else if (currentSpeed < MoveSpeed) {
			currentSpeed += MoveSpeed * Time.deltaTime * 0.5f;
			if (currentSpeed > MoveSpeed)
				currentSpeed = MoveSpeed;
		}
	}

	void OnTriggerEnter(Collider _object){
		if (_object.tag == CollectorTag)
		{
			target = _object.gameObject;
		}
	}

	void SetValue(int _value){
		SoulValue = _value;
	}
}
