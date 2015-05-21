using UnityEngine;
using System.Collections;

public class Soul_Controller : MonoBehaviour {

	public uint SoulValue = 100;
	public float AliveTime = 10.0f;
	public string CollectorTag = "Player";
	public float MoveSpeed = 0.3f;
	private float currentSpeed = 0.0f;
	private GameObject target = null;
	public GameObject SoundEffect = null;
	private bool once = true;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {

	if (target != null) {
			Vector3 targetLoc = target.transform.position;
			targetLoc.y += 2.0f;
			Vector3 moveDirection = targetLoc - transform.position;
			float distance = moveDirection.magnitude;
			moveDirection.Normalize();
			moveDirection *= currentSpeed;
			transform.position += moveDirection;
			if (distance <= 2.25f && once){
				GameObject main = GameObject.FindWithTag("Main");
				main.SendMessage("CollectedSoul", SoulValue, SendMessageOptions.RequireReceiver);
				GameObject soundeffect = Instantiate(SoundEffect);
				soundeffect.transform.parent = GameObject.FindWithTag("Player").transform;
				soundeffect.transform.localPosition = new Vector3(0,0,0);
				Destroy(soundeffect, 1.7f);
				once = false;
			}else if (distance <= 0.1f)
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
		if (_object.tag == CollectorTag) {
			target = _object.gameObject;
		}
	}

	void SetValue(uint _value){
		SoulValue = _value;
	}
}
