﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCameraController : MonoBehaviour {

	private static bool disabled = false;

	public float speedX = 3.0f;
	public float speedY = 2.0f;

	public float maxFieldView = 60.0f;
	public float minFieldView = 45.0f;
	public float zoomTime = 0.3f;

	private GameObject playerObject;
	private GameObject cameraObject;
	private Camera cameraComponent;
	private bool zoomed = false;

	void Start() {
		playerObject = GameObject.Find("Player");
		cameraObject = playerObject.transform.Find("FirstPerson/Camera").gameObject;
		cameraComponent = cameraObject.GetComponent<Camera>();
	}

	// Update is called once per frame
	void Update () {
		if(disabled) {
			return;
		}
		// get current rotation
		Vector3 rotation = cameraObject.transform.eulerAngles;
		float rotationX = rotation.y;
		float rotationY = rotation.x;

		// process rotation X
		rotationX += speedX * Input.GetAxis("Mouse X");
		this.playerObject.transform.eulerAngles = new Vector3(0.0f, rotationX, 0.0f);

		// process rotation Y
		rotationY -= speedY * Input.GetAxis("Mouse Y");
		if(90.0f < rotationY && rotationY < 270.0f) {
			rotationY = rotationY < 180.0f ? 90.0f : 270.0f;
		}

		this.cameraObject.transform.eulerAngles = new Vector3(rotationY, rotationX, 0.0f);

		// process right click
		bool rightMouseButtonClicked = Input.GetAxis("Fire2") == 1.0f;
		if(!rightMouseButtonClicked && !zoomed) {
			return;
		}

		zoomed = true;
		float fieldOfView = cameraComponent.fieldOfView;
		fieldOfView += Time.deltaTime * (1 / zoomTime) * (maxFieldView - minFieldView) * (rightMouseButtonClicked ? -1.0f : 1.0f);

		fieldOfView = fieldOfView < minFieldView ? minFieldView : fieldOfView;
		if(fieldOfView > maxFieldView) {
			fieldOfView = maxFieldView;
			zoomed = false;
		}

		cameraComponent.fieldOfView = fieldOfView;
	}

	public void lookAt(Vector3 position) {
		// orientate player to position
		playerObject.transform.LookAt(position);
		// reset player x and z rotation
		Vector3 rotation = new Vector3(0.0f, playerObject.transform.eulerAngles.y, 0.0f);
		playerObject.transform.eulerAngles = rotation;
		// orientate camera to position
		cameraObject.transform.LookAt(position);
	}

	public void pause(bool pause) {
		disabled = pause;
	}
}
