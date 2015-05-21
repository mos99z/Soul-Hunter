using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Floating_Text_Controller : MonoBehaviour {

	public float Duration = 3.0f;
	public float Speed = 0.02f;

	private float Timer = 0.0f;
	private float RandomX = 0.0f;
	private float RandomZ = 0.0f;

	private float Slope = 0.0f;

	void Start ()
	{
		RandomX = Random.Range (-1.0f, 1.0f) * Speed;
		RandomZ = Random.Range (-1.0f, 1.0f) * Speed;

		Slope = Speed / Duration ;
	}
	
	void Update ()
	{
		Color temp = transform.GetComponent<TextMesh> ().color;
		temp.a = (1.0f - Timer / Duration);
		transform.GetComponent<TextMesh> ().color = temp;
		Timer += Time.deltaTime;
		Speed -= Slope * Time.deltaTime;
		if (Timer >= Duration)
			Destroy (gameObject);
	}

	void FixedUpdate()
	{
		transform.position += new Vector3(RandomX, Speed, RandomZ);
		transform.forward = (transform.position - Camera.main.transform.position).normalized;
	}
}
