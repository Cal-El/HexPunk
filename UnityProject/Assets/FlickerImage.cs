using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlickerImage : MonoBehaviour {

    public float timeToTurnOn = 1.5f;
    private float startTime;
    private Image img;
    private bool isOn = true;

	// Use this for initialization
	void Start () {
        img = GetComponent<Image>();
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (isOn) {
            img.color = new Color(1, 1, 1, Random.Range(-0.2f, 0.2f) * Mathf.Sin(Time.time) + ((Time.time - startTime > timeToTurnOn) ? 1 : 0));
        } else {
            img.color = Color.clear;
        }

        if(MainMenuManager.GetState() == MainMenuManager.MENUSTATES.MainMenu && !isOn) {
            isOn = true;
            startTime = Time.time;
        } else if(MainMenuManager.GetState() != MainMenuManager.MENUSTATES.MainMenu && isOn) {
            isOn = false;
            startTime = Time.time;
        }
	}
}
