using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStatsController : MonoBehaviour {

	private static string playerName;
	private static float playStartTime;

	// Use this for initialization
	void Start () {
		if(SceneManager.GetActiveScene().name.Equals("LevelScene")) {
			playStartTime = Time.time;
			if(playerName == null || playerName.Length == 0) {
				playerName = "Unbekannt";
			}
		} else {
			DontDestroyOnLoad(gameObject);
		}
	}
	
	public static void setPlayerName(string name) {
		playerName = name;
	}
}
