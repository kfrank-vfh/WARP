using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StorySceneController : MonoBehaviour {

	private float lettersPerSecond = 15;

	// Use this for initialization
	void Start () {
		// check if intro or outro
		if(GameStatsController.isIntro()) {
			showIntro();
		} else {
			showOutro();
		}
	}

	private void showIntro() {
		GameObject introPanel = GameObject.Find("/Canvas/IntroPanel");
		Text text1 = introPanel.transform.Find("Text1").GetComponent<Text>();
		Text text2 = introPanel.transform.Find("Text2").GetComponent<Text>();
		Text text3 = introPanel.transform.Find("Text3").GetComponent<Text>();
		StartCoroutine(introCoroutine(text1, text2, text3));
	}

	private IEnumerator introCoroutine(Text text1, Text text2, Text text3) {
		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(showTextCoroutine(text1));
		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(showTextCoroutine(text2));
		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(showTextCoroutine(text3));
		yield return new WaitForSeconds(1f);
		yield return StartCoroutine(fadeOutTextCoroutine(new Text[] {text1, text2, text3}, 1f));
		SceneManager.LoadScene("LevelScene");
	}

	private void showOutro() {
		// change game stats
		GameStatsController.addPlaythrough();
		int playthroughs = GameStatsController.getPlaythroughs();
		HighscoreEntry newEntry = GameStatsController.getHighscoreEntry();
		List<HighscoreEntry> currentEntries = GameStatsController.getHighscoreEntries();
		GameStatsController.addHighscoreEntry(newEntry);
		GameStatsController.reset();
		Transform highscorePanel = transform.Find("HighscorePanel");
		highscorePanel.GetComponent<HighscoreManager>().setCurrentEntriesInUI(false);
		// determine story texts of outro panel
		GameObject outroPanel = GameObject.Find("/Canvas/OutroPanel");
		Text text1 = outroPanel.transform.Find("Text1").GetComponent<Text>();
		text1.text = getPreText(newEntry.timestamp, playthroughs);
		Text text2 = outroPanel.transform.Find("Text2").GetComponent<Text>();
		text2.text = getMainText(currentEntries, newEntry);
		Text text3 = outroPanel.transform.Find("Text3").GetComponent<Text>();
		Text[] storyTexts = new Text[] { text1, text2, text3};
		// determine texts and images for highscore panel
		List<Text> highscoreTexts = new List<Text>();
		List<Image> highscoreImages = new List<Image>();
		highscoreImages.Add(highscorePanel.GetComponent<Image>());
		Transform headerPanel = highscorePanel.transform.Find("HeaderPanel");
		for(int i = 0; i < headerPanel.childCount; i++) {
			highscoreTexts.Add(headerPanel.GetChild(i).GetComponent<Text>());
		}
		Transform tablePanel = highscorePanel.transform.Find("TablePanel");
		for(int i = 0; i < tablePanel.childCount; i++) {
			Transform child = tablePanel.GetChild(i);
			for(int j = 0; j < child.childCount; j++) {
				highscoreTexts.Add(child.GetChild(j).GetComponent<Text>());
			}
		}
		Transform backButton = highscorePanel.transform.Find("BackButton");
		highscoreImages.Add(backButton.GetComponent<Image>());
		highscoreTexts.Add(backButton.GetChild(0).GetComponent<Text>());
		// start coroutine
		StartCoroutine(outroCoroutine(storyTexts, highscoreTexts.ToArray(), highscoreImages.ToArray()));

	}

	private string getPreText(DateTime time, int playthroughs) {
		return "Bericht " + playthroughs + " vom " + time.ToString("dd.MM.yyyy") + " um " + time.ToString("HH:mm") + " Uhr:";
	}

	private string getMainText(List<HighscoreEntry> currentEntries, HighscoreEntry newEntry) {
		int _case = 0;
		if(currentEntries != null && currentEntries.Count > 0) {
			HighscoreEntry bestRun = currentEntries[0];
			_case = newEntry.CompareTo(bestRun) < 0 ? 1 : 2;
		}
		int minutes = (int)(newEntry.playTime % 3600); minutes = (int)(minutes / 60);
		int seconds = (int)(newEntry.playTime % 60);
		string text = "Die künstliche Intelligenz hat die Simulation ";
		text += _case == 0 ? "das erste Mal " : "";
		text += "mit einer Zeit von " + minutes + " Minuten und " + seconds + " Sekunden ";
		text += _case == 0 ? "erfolgreich " : "";
		text += "abgeschlossen";
		if(_case == 0) {
			text += ". Es werden weitere Simulationen durchgeführt, um die Effizienz weiter zu steigern.";
		} else if (_case == 1) {
			text += " und sich dabei im Vergleich zuvor weiterentwickelt. Diese Daten werden genutzt, um neue Simulationen durchzuführen und die Effizienz weiter zu steigern.";
		} else if (_case == 2) {
			text += " und ist dabei im Vergleich zuvor degeneriert. Die Daten werden zurückgesetzt, um weiteren Schaden zu vermeiden.";
		}
		return text;
	}

	private IEnumerator outroCoroutine(Text[] storyTexts, Text[] highscoreTexts, Image[] highscoreImages) {
		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(showTextCoroutine(storyTexts[0]));
		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(showTextCoroutine(storyTexts[1]));
		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(showTextCoroutine(storyTexts[2]));
		yield return new WaitForSeconds(1f);
		yield return StartCoroutine(fadeOutTextCoroutine(storyTexts, 1f));
		yield return new WaitForSeconds(1f);
		StartCoroutine(fadeInHighscorePanel(highscoreTexts, highscoreImages, 1f));
	}

	private IEnumerator showTextCoroutine(Text text) {
		float timePerLetter = 1 / lettersPerSecond;
		string value = text.text;
		text.text = "";
		if(text.color.a < 1f) {
			text.color = changeAlpha(text.color, 1f);
		}
		foreach (char c in value.ToCharArray()) {
			text.text += c;
			yield return new WaitForSeconds(timePerLetter);
		}
	}

	private IEnumerator fadeOutTextCoroutine(Text[] texts, float duration) {
		for(float passedTime = 0f; passedTime < duration; passedTime += Time.fixedDeltaTime) {
			float alpha = 1f - (passedTime / duration);
			alpha = alpha < 0f ? 0f : alpha;
			foreach(Text text in texts) {
				text.color = changeAlpha(text.color, alpha);
			}
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}

	private IEnumerator fadeInHighscorePanel(Text[] texts, Image[] images, float duration) {
		for(float passedTime = 0f; passedTime < duration; passedTime += Time.fixedDeltaTime) {
			float alpha = passedTime / duration;
			alpha = alpha > 1f ? 1f : alpha;
			foreach(Text text in texts) {
				text.color = changeAlpha(text.color, alpha);
			}
			foreach(Image image in images) {
				image.color = changeAlpha(image.color, alpha);
			}
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}

	public void toMainMenu() {
		SceneManager.LoadScene("MainMenu");
	}

	public static Color changeAlpha(Color color, float alpha) {
		return new Color(color.r, color.g, color.b, alpha);
	}
}
