using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPController : MonoBehaviour {

	private GameObject fpCamera;

	public float speedX = 2.0f;
	public float speedY = 2.0f;

	public float rotationX = 0.0f;
	public float rotationY = 0.0f;

	// Use this for initialization
	void Start () {
		fpCamera = GameObject.Find("Player/FirstPerson/FP-Camera");
	}
	
	// Update is called once per frame
	void Update () {
		rotationX += speedX * Input.GetAxis("Mouse X");
		rotationY -= speedY * Input.GetAxis("Mouse Y");
		float _rotationY = rotationY > 90.0f ? 90.0f : (rotationY < -90.0f ? -90.0f : rotationY);
		fpCamera.transform.eulerAngles = new Vector3(_rotationY, rotationX, 0.0f);
	}
}
