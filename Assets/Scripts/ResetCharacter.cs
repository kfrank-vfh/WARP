using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCharacter : MonoBehaviour {

	public Vector3 resetToPosition;
	public float resetToRotation;

	private GameObject player;
	private FPCameraController cameraController;
	private AudioController audioController;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("/Player");
		cameraController = player.transform.Find("FirstPerson/Camera").GetComponent<FPCameraController>();
		audioController = player.transform.Find("ThirdPerson/DefaultAvatar").GetComponent<AudioController>();
	}

	void OnTriggerEnter(Collider other) {
		player.transform.position = resetToPosition;
		cameraController.setRotation(Quaternion.Euler(new Vector3(0f, resetToRotation, 0f)));
		audioController.playDropWoundedSound();
	}
}
