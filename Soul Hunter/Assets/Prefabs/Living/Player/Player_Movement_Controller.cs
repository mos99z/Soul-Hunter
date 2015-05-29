﻿#define MOVEPLAYER
#define ROTATEPLAYER

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player_Movement_Controller : MonoBehaviour {

	public float Speed = 0.25f;
	public GameObject DirectionIndicator = null;
	public Animator Animations = null;
	public LayerMask CursorColliders;

	public Camera ScreenCamera = null;
	private Vector3 currentVelocity = Vector3.zero;
	public GameObject GameOver;

	void Start () {
		//Cursor.visiblpie = false;
		ScreenCamera = Camera.main;
	}

	void Update()
	{
		#if (ROTATEPLAYER)
		if (ScreenCamera != null)
		{
			Ray MouseAim = ScreenCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit MousePosition;
			Physics.Raycast(MouseAim,out MousePosition, 100.0f, CursorColliders);
			DirectionIndicator.transform.LookAt(new Vector3(MousePosition.point.x, DirectionIndicator.transform.position.y, MousePosition.point.z));
			
			if(DirectionIndicator.transform.forward.z >= 0.0f)
				Animations.Play("Player_Up_Idle");
			else
				Animations.Play("Player_Down_Idle");
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
			enemyList[i].SendMessage("PlayerDead");
		}

		//GameOver.SendMessage ("PlayerDead");
		GameOver.SetActive (true);
	}
}
