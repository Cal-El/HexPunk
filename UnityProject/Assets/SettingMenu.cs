using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SettingMenu : MonoBehaviour {

	[SerializeField]private Dropdown QualityList;
	[SerializeField]private Dropdown ResolutionList;
	[SerializeField]private Toggle fullscreenOnOff;

	// Use this for initialization
	void Start () {
		QualityList.options = new List<Dropdown.OptionData> ();
		foreach(string level in QualitySettings.names){
			QualityList.options.Add (new Dropdown.OptionData (level));
		}
		QualityList.value = QualitySettings.GetQualityLevel ();
		QualityList.RefreshShownValue ();

		ResolutionList.options = new List<Dropdown.OptionData> ();
		int currentRes = 0;
		for(int i = 0; i < Screen.resolutions.Length; i++){
			ResolutionList.options.Add (new Dropdown.OptionData (Screen.resolutions[i].ToString()));
			if (Screen.resolutions [i].height == Screen.height) {
				currentRes = i;
			}

		}
		ResolutionList.value = currentRes;
		ResolutionList.RefreshShownValue ();

		fullscreenOnOff.isOn = Screen.fullScreen;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeQualitySettings(int q){
		QualitySettings.SetQualityLevel (q);
	}

	public void SetRes(int q){
		Screen.SetResolution(Screen.resolutions[q].width, Screen.resolutions[q].height, Screen.fullScreen);
	}
		
	public void SetFullscreen(bool q){
		Screen.SetResolution(Screen.width, Screen.height, q);
	}
}
