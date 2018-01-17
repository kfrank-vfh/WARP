using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TilingProcessor : MonoBehaviour {

	private bool processed = false;

	private Vector2 defaultOffset = new Vector2(0, 0);

	public Vector2 tiling = new Vector2(1, 1);
	public Vector2 offset = new Vector2(0, 0);

	// Use this for initialization
	void Start () {
		if(!processed) {
			if(SceneManager.GetActiveScene().name.Equals("MainMenu")) {
				recursiveProcessTiling(GameObject.Find("/Background").transform);
			} else if (SceneManager.GetActiveScene().name.Equals("LevelScene")) {
				recursiveProcessTiling(GameObject.Find("Rooms").transform);
				recursiveProcessTiling(GameObject.Find("Corridors").transform);
			}
			processed = true;
		}
	}

	private void recursiveProcessTiling(Transform transform) {
		GameObject obj = transform.gameObject;
		Renderer renderer = obj.GetComponent<Renderer>();
		if(renderer != null && obj.layer != LayerMask.NameToLayer("DoNotTile")) {
			TilingProcessor props = obj.GetComponent<TilingProcessor>();
			Material mat = renderer.material;
			Vector2 tiling = props == null ? getDefaultTiling(obj.transform.localScale) : props.tiling;
			Vector2 offset = props == null ? defaultOffset : props.offset;
			mat.SetTextureScale("_MainTex", tiling);
			mat.SetTextureScale("_BumpMap", tiling);
			mat.SetTextureOffset("_MainTex", offset);
			mat.SetTextureOffset("_BumpMap", offset);
		}
		foreach (Transform child in transform) {
			recursiveProcessTiling(child);
		}
	}

	private Vector2 getDefaultTiling(Vector3 scale) {
		return new Vector2(scale.x / 0.25f, scale.z / 0.25f);
	}
}
