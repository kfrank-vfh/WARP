using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCameraController : MonoBehaviour {

	public float speedX = 3.0f;
	public float speedY = 2.0f;

	public float rotationX = 0.0f;
	public float rotationY = 0.0f;

	private GameObject player;
	private GameObject _camera;

	void Start() {
		player = GameObject.Find("Player");
		_camera = player.transform.Find("FirstPerson/Camera").gameObject;
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
	}
}
