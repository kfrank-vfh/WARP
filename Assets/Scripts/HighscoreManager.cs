﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreManager : MonoBehaviour {

	private static string HIGHSCORE_FILE;
	private static List<HighscoreEntry> CURRENT_ENTRIES;

	public GameObject highscoreEntryPrefab;

	private Transform tablePanel;

	// Use this for initialization
	void Start () {
		HIGHSCORE_FILE = Application.persistentDataPath + "/highscore.dat";
		tablePanel = transform.Find("TablePanel");
		determineCurrentEntries();
		setCurrentEntriesInUI();
	}

	private void determineCurrentEntries() {
		CURRENT_ENTRIES = (List<HighscoreEntry>) PersistenceManager.loadData(HIGHSCORE_FILE);
		if(CURRENT_ENTRIES == null) {
			CURRENT_ENTRIES = getTestEntries();
		}
		CURRENT_ENTRIES.Sort();
	}

	private void setCurrentEntriesInUI() {
		// remove all entries in table panel
		while (tablePanel.childCount > 0) {
			Transform child = tablePanel.GetChild(0);
			child.parent = null;
			child.gameObject.SetActive(false);
			Destroy(child.gameObject);
		}
		// check if current entries exist
		if(CURRENT_ENTRIES == null || CURRENT_ENTRIES.Count == 0) {
			return;
		}
		// if entries exist, create gameobject b< prefab for each entry
		for(int i = 0; i < CURRENT_ENTRIES.Count; i++) {
			HighscoreEntry entry = CURRENT_ENTRIES[i];
			GameObject goEntry = Instantiate(highscoreEntryPrefab);
			goEntry.transform.Find("NameText").GetComponent<Text>().text = entry.name;
			goEntry.transform.Find("DateText").GetComponent<Text>().text = entry.timestamp.ToString("dd.MM.yyyy");
			goEntry.transform.Find("TimeText").GetComponent<Text>().text = entry.timestamp.ToString("HH:mm");
			goEntry.transform.Find("PlaythroughTimeText").GetComponent<Text>().text = getPlayTimeString(entry.playTime);
			goEntry.transform.SetParent(tablePanel, false);
		}
	}

	private string getPlayTimeString(float playTime) {
		int hours = (int)(playTime / 3600);
		int minutes = (int)(playTime % 3600); minutes = (int)(minutes / 60);
		int seconds = (int)(playTime % 60);
		string hoursString = hours > 0 ? hours + ":" : "";
		string minutesString = hours > 0 || minutes > 0 ? minutes + ":" : "";
		minutesString = minutesString.Length == 2 ? "0" + minutesString : minutesString;
		string secondsString = seconds < 10 ? "0" + seconds : seconds + "";
		return hoursString + minutesString + secondsString;
	}

	private List<HighscoreEntry> getTestEntries() {
		List<HighscoreEntry> list = new List<HighscoreEntry>();
		list.Add(new HighscoreEntry(DateTime.Now, "TestName1", 11f));
		list.Add(new HighscoreEntry(DateTime.Now, "TestName2", 671f));
		list.Add(new HighscoreEntry(DateTime.Now, "TestName3", 4271f));
		return list;
	}
}

[Serializable]
class HighscoreEntry : IComparable<HighscoreEntry> {

	public DateTime timestamp;
	public string name;
	public float playTime;

	public HighscoreEntry() {}

	public HighscoreEntry(DateTime timestamp, string name, float playTime) {
		this.timestamp = timestamp;
		this.name = name;
		this.playTime = playTime;
	}

	public int CompareTo(HighscoreEntry entry) {
		return (int)(playTime - entry.playTime);
	}
}