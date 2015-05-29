using UnityEngine;
using System.Collections;

public class Camera_Follow_Object : MonoBehaviour {

	public GameObject FollowTarget = null;
	public float DistanceOffset = 14.0f;
	public float HeightOffset = 14.0f;
	public LayerMask ObjectsToHide;
	
	private GameObject lastCollided = null;

	void Start () {
		
	}

	void Update () {

		if (FollowTarget == null) 
		{
			FollowTarget = GameObject.FindGameObjectWithTag ("Player");
			if(FollowTarget != null)
				FollowTarget.GetComponent<Player_Movement_Controller>().ScreenCamera = transform.GetComponent<Camera>();
		}
		else
		{
			Vector3 newPosition = transform.position;
			newPosition.x = FollowTarget.transform.position.x;
			newPosition.y = FollowTarget.transform.position.y + HeightOffset;
			newPosition.z = FollowTarget.transform.position.z - DistanceOffset;
			
			transform.position = newPosition;
			
			Ray CameraToPlayer = new Ray(transform.position, (FollowTarget.transform.position - transform.position));
			//Ray toPlayer = Camera.main.ScreenPointToRay(new Vector2(Camera.main.pixelWidth * 0.5f, Camera.main.pixelHeight * 0.5f));
			RaycastHit ObjectInBetween;
			Physics.Raycast(CameraToPlayer, out ObjectInBetween, (FollowTarget.transform.position - transform.position).magnitude, ObjectsToHide);
			if (ObjectInBetween.collider)
			{
				if (ObjectInBetween.collider != lastCollided){
					if (lastCollided != null)
						lastCollided.GetComponent<Renderer>().enabled = true;
					ObjectInBetween.collider.gameObject.GetComponent<Renderer>().enabled = false;
					lastCollided = ObjectInBetween.collider.gameObject;
				}
			}
			else if (lastCollided != null)
			{
				lastCollided.GetComponent<Renderer>().enabled = true;
				lastCollided = null;
			}
		}
	}
}
