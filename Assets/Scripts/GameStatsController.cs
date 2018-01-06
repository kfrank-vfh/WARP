using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStatsController : MonoBehaviour {

	private static string playerName;
	private static float playStartTime = -1f;
	private static float playEndTime = -1f;

	private static GameObject INSTANCE;
	private static string HIGHSCORE_FILE;
	private static List<HighscoreEntry> CURRENT_ENTRIES;
	private static string PLAYTHROUGHS_FILE;
	private static int PLAYTHROUGHS = -1;

	// Use this for initialization
	void Start () {
		INSTANCE = gameObject;
		// dont destroy the game stats
		DontDestroyOnLoad(gameObject);
		// load playthroughs data if not laoded yet
		if(PLAYTHROUGHS_FILE == null) {
			PLAYTHROUGHS_FILE = Application.persistentDataPath + "/playthroughs.dat";
		}
		if(PLAYTHROUGHS < 0) {
			int[] plays = (int[]) PersistenceManager.loadData(PLAYTHROUGHS_FILE);
			PLAYTHROUGHS = plays == null ? 0 : plays[0];
		}
		// load hichscore data if not loaded yet
		// check if highscore file created
		if(HIGHSCORE_FILE == null) {
			HIGHSCORE_FILE = Application.persistentDataPath + "/highscore.dat";
		}
		// check if current entries exist
		if(CURRENT_ENTRIES == null) {
			CURRENT_ENTRIES = (List<HighscoreEntry>) PersistenceManager.loadData(HIGHSCORE_FILE);
			CURRENT_ENTRIES = CURRENT_ENTRIES == null ? new List<HighscoreEntry>() : CURRENT_ENTRIES;
		}
		// sort entries
		CURRENT_ENTRIES.Sort();
	}

	void OnLevelWasLoaded() {
		if(SceneManager.GetActiveScene().name.Equals("LevelScene")) {
			playStartTime = Time.time;
			if(playerName == null || playerName.Length == 0) {
				playerName = "Unbekannt";
			}
		}
	}

	public static bool isIntro() {
		return playStartTime < 0;
	}
	
	public static void setPlayerName(string name) {
		playerName = name;
	}

	public static void setPlayEndTime(float time) {
		playEndTime = time;
	}

	public static int getPlaythroughs() {
		return PLAYTHROUGHS;
	}

	public static void addPlaythrough() {
		PLAYTHROUGHS += 1;
		PersistenceManager.saveData(PLAYTHROUGHS_FILE, new int[] {PLAYTHROUGHS});
	}

	public static HighscoreEntry getHighscoreEntry() {
		return new HighscoreEntry(DateTime.Now, playerName, playEndTime - playStartTime);
	}

	public static List<HighscoreEntry> getHighscoreEntries() {
		return CURRENT_ENTRIES;
	}

	public static void addHighscoreEntry(HighscoreEntry entry) {
		CURRENT_ENTRIES.Add(entry);
		CURRENT_ENTRIES.Sort();
		if(CURRENT_ENTRIES.Count > 10) {
			CURRENT_ENTRIES.RemoveAt(10);
		}
		PersistenceManager.saveData(HIGHSCORE_FILE, CURRENT_ENTRIES);
	}

	public static void reset() {
		playerName = null;
		playStartTime = -1f;
		playEndTime = -1f;
		Destroy(INSTANCE);
		INSTANCE = null;
	}
}
