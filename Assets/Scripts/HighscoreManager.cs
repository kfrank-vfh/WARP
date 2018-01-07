using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HighscoreManager : MonoBehaviour {

	public GameObject highscoreEntryPrefab;
	private Transform tablePanel;

	// Use this for initialization
	void Start () {
		tablePanel = transform.Find("TablePanel");
		if(SceneManager.GetActiveScene().name.Equals("MainMenu")) {
			setCurrentEntriesInUI(true);
		}
	}

	public void setCurrentEntriesInUI(bool visibleEntries) {
		// remove all entries in table panel
		while (tablePanel.childCount > 0) {
			Transform child = tablePanel.GetChild(0);
			child.SetParent(null);
			child.gameObject.SetActive(false);
			Destroy(child.gameObject);
		}
		// check if current entries exist
		List<HighscoreEntry> entries = GameStatsController.getHighscoreEntries();
		if(entries == null || entries.Count == 0) {
			return;
		}
		// if entries exist, create gameobject of prefab for each entry
		var alpha = visibleEntries ? 1f : 0f;
		for(int i = 0; i < entries.Count; i++) {
			HighscoreEntry entry = entries[i];
			GameObject goEntry = Instantiate(highscoreEntryPrefab);
			Text nameText = goEntry.transform.Find("NameText").GetComponent<Text>();
			nameText.text = entry.name;
			nameText.color = StorySceneController.changeAlpha(nameText.color, alpha);
			Text dateText = goEntry.transform.Find("DateText").GetComponent<Text>();
			dateText.text = entry.timestamp.ToString("dd.MM.yyyy");
			dateText.color = StorySceneController.changeAlpha(dateText.color, alpha);
			Text timeText = goEntry.transform.Find("TimeText").GetComponent<Text>();
			timeText.text = entry.timestamp.ToString("HH:mm");
			timeText.color = StorySceneController.changeAlpha(timeText.color, alpha);
			Text playthroughTimeText = goEntry.transform.Find("PlaythroughTimeText").GetComponent<Text>();
			playthroughTimeText.text = getPlayTimeString(entry.playTime);
			playthroughTimeText.color = StorySceneController.changeAlpha(playthroughTimeText.color, alpha);
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
}

[Serializable]
public class HighscoreEntry : IComparable<HighscoreEntry> {

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