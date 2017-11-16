using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionRenderer : MonoBehaviour {

	private GameObject fpCameraObject;
	private GameObject plane;
	private GameObject reflectionProbeObject;

	private Camera fpCamera;
	private MeshRenderer planeRenderer;
	private ReflectionProbe reflectionProbe;

	// Use this for initialization
	void Start () {
		fpCameraObject = GameObject.Find("Player/FirstPerson/Camera");
		plane = transform.Find("Plane").gameObject;
		reflectionProbeObject = transform.Find("ReflectionProbe").gameObject;
		fpCamera = fpCameraObject.GetComponent<Camera>();
		planeRenderer = plane.GetComponent<MeshRenderer>();
		reflectionProbe = reflectionProbeObject.GetComponent<ReflectionProbe>();
	}
	
	// Update is called once per frame
	void Update () {
		// do not refresh if mirror is not visible
		if(!isVisible()) {
			return;
		}
		// reposition the reflection probe
		Vector3 planeNormal = plane.transform.up;
		Vector3 planePosition = plane.transform.position;
		Vector3 cameraPosition = fpCameraObject.transform.position;

		Plane _plane = new Plane(planeNormal, planePosition);
		float distance = _plane.GetDistanceToPoint(cameraPosition);
		Vector3 direction = planeNormal * -1;
		Vector3 newPosition = cameraPosition + (direction * distance * 2);

		reflectionProbeObject.transform.position = newPosition;
		// redraw the reflection
		reflectionProbe.RenderProbe();
	}

	private bool isVisible() {
		Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(fpCamera);
		return GeometryUtility.TestPlanesAABB(frustumPlanes, planeRenderer.bounds);
	}

	private bool isPointVisible(Vector3 point) {
		point = fpCamera.WorldToViewportPoint(point);
		return point.x > 0 && point.x < 1 && point.y > 0 && point.y < 1 && point.z > 0;
	}
}
