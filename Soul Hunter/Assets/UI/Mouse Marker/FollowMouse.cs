using UnityEngine;
using System.Collections;

public class FollowMouse : MonoBehaviour {

	public LayerMask CollisionLayers;

	// Use this for initialization
	void Start () {
	}

	
	// Update is called once per framew
	void FixedUpdate ()
	{
		Ray MouseAim = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit MousePosition;
		Physics.Raycast (MouseAim, out MousePosition, float.MaxValue, CollisionLayers);
		Vector3 newPosition = MousePosition.point;
		newPosition.y += 0.005f;
		transform.position = newPosition;
		if (MousePosition.collider != null && MousePosition.collider.tag == "VOID") {
			GetComponent<Renderer> ().enabled = false;
		} else {
			GetComponent<Renderer> ().enabled = true;
		}
	}
}
