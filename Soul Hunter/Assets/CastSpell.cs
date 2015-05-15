using UnityEngine;
using System.Collections;

public class CastSpell : MonoBehaviour {

	public GameObject MessageReciever = null;

	void Start () {
		if (MessageReciever == null)
			Debug.LogError ("Message Reciever on Script Cast Spell on Object Bounding Floor can not be null!");
	}
	
	void OnMouseDown()
	{
		MessageReciever.SendMessage ("CastSpell");
	}
}
