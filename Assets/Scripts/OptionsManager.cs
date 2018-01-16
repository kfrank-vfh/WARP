using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour {

	private static string OPTIONS_FILE;
	private static int[] REFLECTION_QUALITIES = new int[] { 128, 256, 512, 1024, 2048 };
	private static Options CURRENT_OPTIONS;

	private MenuAnimatorController menuAnimatorController;

	private Dropdown resolutionDropdown;
	private Dropdown framerateDropdown;
	private Dropdown graphicsQualityDropdown;
	private Dropdown reflectionQualityDropdown;
	private Slider volumeSlider;

	void Start () {
		OPTIONS_FILE = Application.persistentDataPath + "/options.dat";
		menuAnimatorController = GameObject.Find("MenuCanvas").GetComponent<MenuAnimatorController>();
		determineUiElements();
		initDropdownValues();
		determineCurrentOptions();
		AdoptCurrentOptions();
		setCurrentOptionsInUI();
	}

	private void determineUiElements() {
		resolutionDropdown = transform.Find("ResolutionGroup").Find("Dropdown").GetComponent<Dropdown>();
		framerateDropdown = transform.Find("FramerateGroup").Find("Dropdown").GetComponent<Dropdown>();
		graphicsQualityDropdown = transform.Find("GraphicsQualityGroup").Find("Dropdown").GetComponent<Dropdown>();
		reflectionQualityDropdown = transform.Find("ReflectionQualityGroup").Find("Dropdown").GetComponent<Dropdown>();
		volumeSlider = transform.Find("VolumeGroup").Find("Slider").GetComponent<Slider>();
	}

	private void initDropdownValues() {
		// init resolution and framerate values
		List<string> resolutions = new List<string>();
		List<string> framerates = new List<string>();
		for (int i = 0; i < Screen.resolutions.Length; i++) {
			Resolution res = Screen.resolutions[i];
			string resolution = res.width + " x " + res.height + "px";
			if(!resolutions.Contains(resolution)) {;
				resolutions.Add(resolution);
			}
			string framerate = res.refreshRate + " Hz";
			if(!framerates.Contains(framerate)) {;
				framerates.Add(framerate);
			}
		}
		resolutionDropdown.ClearOptions();
		resolutionDropdown.AddOptions(resolutions);
		framerateDropdown.ClearOptions();
		framerateDropdown.AddOptions(framerates);
		// init graphics quality values
		graphicsQualityDropdown.ClearOptions();
		graphicsQualityDropdown.AddOptions(new List<string>(QualitySettings.names));
		// init reflection quality values
		string[] reflectionQualities = new string[REFLECTION_QUALITIES.Length];
		for (int i = 0; i < reflectionQualities.Length; i++) {
			reflectionQualities[i] = REFLECTION_QUALITIES[i].ToString();
		}
		reflectionQualityDropdown.ClearOptions();
		reflectionQualityDropdown.AddOptions(new List<string>(reflectionQualities));
	}

	private void determineCurrentOptions() {
		CURRENT_OPTIONS = (Options) PersistenceManager.loadData(OPTIONS_FILE);
		if(CURRENT_OPTIONS == null) {
			CURRENT_OPTIONS = getDefaultOptions();
		}
	}

	private Options getDefaultOptions() {
		Options options = new Options();
		Resolution res = Screen.resolutions[Screen.resolutions.Length - 1];
		Debug.Log("Default Resolution: " + res);
		options.resolutionWidth = res.width;
		options.resolutionHeight = res.height;
		options.framerate = res.refreshRate;
		options.graphicsQuality = QualitySettings.names[QualitySettings.names.Length/2];
		options.reflectionQuality = 1024;
		options.volume = 1f;
		return options;
	}

	private void setCurrentOptionsInUI() {
		Options opts = CURRENT_OPTIONS;
		// set selected resolution
		int framerateCount = framerateDropdown.options.Count;
		for(int i = 0; i < Screen.resolutions.Length; i++) {
			Resolution res = Screen.resolutions[i];
			if(res.width == opts.resolutionWidth && res.height == opts.resolutionHeight && res.refreshRate == opts.framerate) {
				resolutionDropdown.value = (int)(i / framerateCount);
				framerateDropdown.value = i % framerateCount;
				break;
			}
		}
		// set selected graphics quality
		for(int i = 0; i < graphicsQualityDropdown.options.Count; i++) {
			if(graphicsQualityDropdown.options[i].text.Equals(opts.graphicsQuality)) {
				graphicsQualityDropdown.value = i;
				break;
			}
		}
		// set selected reflection quality
		for(int i = 0; i < reflectionQualityDropdown.options.Count; i++) {
			if(reflectionQualityDropdown.options[i].text.Equals(opts.reflectionQuality.ToString())) {
				reflectionQualityDropdown.value = i;
				break;
			}
		}
		// set volume slider value
		volumeSlider.value = opts.volume;
	}

	private Options getOptionsFromUI() {
		Options opts = new Options();
		int resValue = resolutionDropdown.value;
		int frValue = framerateDropdown.value;
		Resolution res = Screen.resolutions[resValue * framerateDropdown.options.Count + frValue];
		opts.resolutionWidth = res.width;
		opts.resolutionHeight = res.height;
		opts.framerate = res.refreshRate;
		opts.graphicsQuality = graphicsQualityDropdown.options[graphicsQualityDropdown.value].text;
		opts.reflectionQuality = REFLECTION_QUALITIES[reflectionQualityDropdown.value];
		opts.volume = volumeSlider.value;
		return opts;
	}

	public void AdoptOptions() {
		// set and save current options
		Options uiOptions = getOptionsFromUI();
		CURRENT_OPTIONS = uiOptions;
		PersistenceManager.saveData(OPTIONS_FILE, uiOptions);
		// adopt options to unity engine
		AdoptCurrentOptions();
		// show title menu
		menuAnimatorController.showMenu("title");
	}

	public void AdoptCurrentOptions() {
		Screen.SetResolution(CURRENT_OPTIONS.resolutionWidth, CURRENT_OPTIONS.resolutionHeight, true);
		Application.targetFrameRate = CURRENT_OPTIONS.framerate;
		for(int i = 0; i < QualitySettings.names.Length; i++) {
			if(QualitySettings.names[i].Equals(CURRENT_OPTIONS.graphicsQuality)) {
				QualitySettings.SetQualityLevel(i);
				break;
			}
		}
		setReflectionQualityOptions();
		AudioListener.volume = CURRENT_OPTIONS.volume;
	}

	public static void setReflectionQualityOptions() {
		if(!SceneManager.GetActiveScene().name.Equals("LevelScene")) {
			return;
		}
		GameObject[] mirrors = GameObject.FindGameObjectsWithTag("Mirrors");
		foreach (GameObject mirror in mirrors) {
			Transform reflectionProbe = mirror.transform.Find("ReflectionProbe");
			if(reflectionProbe != null) {
				ReflectionProbe probe = reflectionProbe.GetComponent<ReflectionProbe>();
				probe.resolution = CURRENT_OPTIONS.reflectionQuality;
			}
		}
	}

	public static void setVolumeSettings() {
		AudioController.setMasterVolume(CURRENT_OPTIONS.volume);
	}

	public void DiscardOptions() {
		setCurrentOptionsInUI();
		menuAnimatorController.showMenu("title");
	}
}

[Serializable]
class Options {
	public int resolutionWidth;
	public int resolutionHeight;
	public int framerate;
	public String graphicsQuality;
	public int reflectionQuality;
	public float volume;
}