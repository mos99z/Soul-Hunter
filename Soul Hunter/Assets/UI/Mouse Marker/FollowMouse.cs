using UnityEngine;
using System.Collections;

public class FollowMouse : MonoBehaviour {

	public LayerMask CollisionLayers;
	private Behaviour Projector = null;

	// Use this for initialization
	void Start () {
		Projector = (Behaviour)GetComponentInChildren<Projector>();
	}

	
	// Update is called once per frame
	void Update () {

		Ray MouseAim = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit MousePosition;
		Physics.Raycast(MouseAim,out MousePosition, float.MaxValue, CollisionLayers);
		Vector3 newPosition = MousePosition.point;
		newPosition.y += 0.1f;
		transform.position = newPosition;
		if (MousePosition.collider != null && MousePosition.collider.tag == "VOID") {
			GetComponent<Renderer> ().enabled = false;
			Projector.enabled = false;
		} else {
			GetComponent<Renderer> ().enabled = true;
			Projector.enabled = true;
		}
	}
}
