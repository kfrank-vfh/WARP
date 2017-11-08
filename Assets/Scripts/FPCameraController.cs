using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCameraController : MonoBehaviour {

	public float speedX = 3.0f;
	public float speedY = 2.0f;

	public float rotationX = 0.0f;
	public float rotationY = 0.0f;

	public float maxFieldView = 60.0f;
	public float minFieldView = 45.0f;
	public float zoomTime = 0.3f;

	private GameObject player;
	private GameObject _camera;
	private Camera cameraComponent;
	private bool zoomed = false;

	void Start() {
		player = GameObject.Find("Player");
		_camera = player.transform.Find("FirstPerson/Camera").gameObject;
		cameraComponent = _camera.GetComponent<Camera>();
		rotationX = player.transform.eulerAngles.y;
	}

	// Update is called once per frame
	void Update () {
		// process rotation X
		rotationX += speedX * Input.GetAxis("Mouse X");
		this.player.transform.eulerAngles = new Vector3(0.0f, rotationX, 0.0f);

		// process rotation Y
		rotationY -= speedY * Input.GetAxis("Mouse Y");
		float _rotationY = rotationY > 90.0f ? 90.0f : (rotationY < -90.0f ? -90.0f : rotationY);
		this._camera.transform.eulerAngles = new Vector3(_rotationY, rotationX, 0.0f);

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
}
