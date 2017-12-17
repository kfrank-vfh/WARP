using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour {

	private static string OPTIONS_FILE;
	private static int[] REFLECTION_QUALITIES = new int[] { 128, 256, 512, 1024, 2048 };
	private static Options CURRENT_OPTIONS;

	private MenuAnimatorController menuAnimatorController;

	private Dropdown resolutionDropdown;
	private Dropdown graphicsQualityDropdown;
	private Dropdown reflectionQualityDropdown;
	private Slider volumeSlider;

	void Start () {
		OPTIONS_FILE = Application.persistentDataPath + "/options.dat";
		menuAnimatorController = GameObject.Find("MenuCanvas").GetComponent<MenuAnimatorController>();
		determineUiElements();
		initDropdownValues();
		determineCurrentOptions();
		setCurrentOptionsInUI();
	}

	private void determineUiElements() {
		resolutionDropdown = transform.Find("ResolutionGroup").Find("Dropdown").GetComponent<Dropdown>();
		graphicsQualityDropdown = transform.Find("GraphicsQualityGroup").Find("Dropdown").GetComponent<Dropdown>();
		reflectionQualityDropdown = transform.Find("ReflectionQualityGroup").Find("Dropdown").GetComponent<Dropdown>();
		volumeSlider = transform.Find("VolumeGroup").Find("Slider").GetComponent<Slider>();
	}

	private void initDropdownValues() {
		// init resolution values
		string[] resolutions = new string[Screen.resolutions.Length];
		for (int i = 0; i < resolutions.Length; i++) {
			Resolution res = Screen.resolutions[i];
			resolutions[i] = res.width + " x " + res.height + "px";
		}
		resolutionDropdown.ClearOptions();
		resolutionDropdown.AddOptions(new List<string>(resolutions));
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
		options.resolutionWidth = res.width;
		options.resolutionHeight = res.height;
		options.graphicsQuality = QualitySettings.names[QualitySettings.names.Length/2];
		options.reflectionQuality = 1024;
		options.volume = 1f;
		return options;
	}

	private void setCurrentOptionsInUI() {
		Options opts = CURRENT_OPTIONS;
		// set selected resolution
		for(int i = 0; i < Screen.resolutions.Length; i++) {
			Resolution res = Screen.resolutions[i];
			if(res.width == opts.resolutionWidth && res.height == opts.resolutionHeight) {
				resolutionDropdown.value = i;
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
		Resolution res = Screen.resolutions[resolutionDropdown.value];
		opts.resolutionWidth = res.width;
		opts.resolutionHeight = res.height;
		opts.graphicsQuality = graphicsQualityDropdown.options[graphicsQualityDropdown.value].text;
		opts.reflectionQuality = REFLECTION_QUALITIES[reflectionQualityDropdown.value];
		opts.volume = volumeSlider.value;
		return opts;
	}

	public void AdoptOptions() {
		Options uiOptions = getOptionsFromUI();
		CURRENT_OPTIONS = uiOptions;
		PersistenceManager.saveData(OPTIONS_FILE, uiOptions);
		menuAnimatorController.showMenu("title");
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
	public String graphicsQuality;
	public int reflectionQuality;
	public float volume;
}