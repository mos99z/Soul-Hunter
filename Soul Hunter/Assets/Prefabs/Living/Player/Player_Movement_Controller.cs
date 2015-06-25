#define MOVEPLAYER
#define ROTATEPLAYER

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player_Movement_Controller : MonoBehaviour {

	public float Speed = 0.25f;
	public GameObject DirectionIndicator = null;
	public Animator Animate = null;
	public LayerMask CursorColliders;

	public Camera ScreenCamera = null;
	private Vector3 currentVelocity = Vector3.zero;
	public bool isCrippled;		// used for checking cripple on player

	//animation stuff
	public bool underMelee = false;
	private float attackTicker = 0;
	public float attackLength = 0;
	public GameObject spriteImage;

	void Start ()
	{
		//Cursor.visiblpie = false;
		ScreenCamera = Camera.main;
	}

	void Update()
	{
		if (underMelee)
		{
			attackTicker += Time.deltaTime;
			if (attackTicker >= attackLength)
			{
				underMelee = false;
				attackTicker = 0;
			}
		}
		#if (ROTATEPLAYER)
		if (ScreenCamera != null)
		{
			Ray MouseAim = ScreenCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit MousePosition;
			Physics.Raycast(MouseAim,out MousePosition, 100.0f, CursorColliders);
			DirectionIndicator.transform.LookAt(new Vector3(MousePosition.point.x, DirectionIndicator.transform.position.y, MousePosition.point.z));

			Vector3 movementDirection = DirectionIndicator.transform.forward.normalized;
			if (movementDirection.magnitude >= 1.0f)
			{
				DirectionIndicator.transform.forward = movementDirection.normalized;
				float dotProd = Vector3.Dot (new Vector3 (0, 0, 1), movementDirection);
				Vector3 crossProd = Vector3.Cross (new Vector3 (0, 0, 1), movementDirection);
				if (underMelee)
				{
					Vector3 tempPos = this.gameObject.transform.position;
					tempPos.y = 2.21f;
					spriteImage.transform.position = tempPos;
				}
				else
				{
					Vector3 tempPos = this.gameObject.transform.position;
					tempPos.y = 2.4f;
					tempPos.z -= 0.5f;
					spriteImage.transform.position = tempPos;
				}
				if (dotProd >= 0.75f)
				{
					if (underMelee)
					{
						Animate.Play ("Player_Slash_Up");
					}
					else
					{
						Animate.Play ("Player_Move_Up");
					}
				}
				else if (dotProd <= -0.75f)
				{
					if (underMelee)
					{
						Animate.Play ("Player_Slash_Down");
					}
					else
					{
						Animate.Play ("Player_Move_Down");
					}
				}
				else if (dotProd > -0.25f && dotProd <= 0.25f)
				{
					if (crossProd.y < 0.0f)
					{
						if (underMelee)
						{
							Animate.Play ("Player_Slash_Left");
						}
						else
						{
							Animate.Play ("Player_Move_Left");
						}
					}
					else
					{
						if (underMelee)
						{
							Animate.Play ("Player_Slash_Right");
						}
						else
						{
							Animate.Play ("Player_Move_Right");
						}
					}
				}
				else if (dotProd > 0.25f && dotProd < 0.75f)
				{
					if (crossProd.y < 0.0f)
					{
						if (underMelee)
						{
							Animate.Play ("Player_Slash_UpLeft");
						}
						else
						{
							Animate.Play ("Player_Move_UpLeft");
						}
					}
					else
					{
						if (underMelee)
						{
							Animate.Play ("Player_Slash_UpRight");
						}
						else
						{
							Animate.Play ("Player_Move_UpRight");
						}
					}
				}
				else
				{
					if (crossProd.y < 0.0f)
					{
						if (underMelee)
						{
							Animate.Play ("Player_Slash_DownLeft");
						}
						else
						{
							Animate.Play ("Player_Move_DownLeft");
						}
					}
					else
					{
						if (underMelee)
						{
							Animate.Play ("Player_Slash_DownRight");
						}
						else
						{
							Animate.Play ("Player_Move_DownRight");
						}
					}
				}
			}
		}
		#endif


	}

	void FixedUpdate ()
	{
		#if (MOVEPLAYER)
		currentVelocity.x = Input.GetAxisRaw ("Horizontal");
		currentVelocity.z = Input.GetAxisRaw ("Vertical");
		currentVelocity.Normalize ();
		currentVelocity *= Speed;
		transform.position += currentVelocity;
		#endif
	}

	void OnDestroy()
	{
		GameObject[] enemyList = GameObject.FindGameObjectsWithTag ("Enemy");
		for (int i = 0; i < enemyList.Length; i++) 
		{
			enemyList[i].SendMessage("PlayerDead", SendMessageOptions.DontRequireReceiver);
		}
	}
}
