using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour {

	public float speed = 6.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;

	private Vector3 moveDirection = Vector3.zero;

	private CharacterController controller;

	// Use this for initialization
	void Start () {
		this.controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("escape")) {
			Application.Quit();
		}
		if(controller.isGrounded) {
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if(Input.GetButton("Jump")) {
				moveDirection.y = jumpSpeed;
			}
		}
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
	}
}
