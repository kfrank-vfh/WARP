﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilingProcessor : MonoBehaviour {

	private bool processed = false;

	private Vector2 defaultOffset = new Vector2(0, 0);

	public Vector2 tiling = new Vector2(1, 1);
	public Vector2 offset = new Vector2(0, 0);

	// Use this for initialization
	void Start () {
		if(!processed) {
			processed = true;
			recursiveProcessTiling(GameObject.Find("Rooms").transform);
			recursiveProcessTiling(GameObject.Find("Corridors").transform);
		}
	}

	private void recursiveProcessTiling(Transform transform) {
		GameObject obj = transform.gameObject;
		Renderer renderer = obj.GetComponent<Renderer>();
		if(renderer != null) {
			TilingProcessor props = obj.GetComponent<TilingProcessor>();
			Material mat = renderer.material;
			mat.SetTextureScale("_MainTex", props == null ? getDefaultTiling(obj.transform.localScale) : props.tiling);
			mat.SetTextureOffset("_MainTex", props == null ? defaultOffset : props.offset);
		}
		foreach (Transform child in transform) {
			recursiveProcessTiling(child);
		}
	}

	private Vector2 getDefaultTiling(Vector3 scale) {
		return new Vector2(scale.x / 0.25f, scale.z / 0.25f);
	}
}