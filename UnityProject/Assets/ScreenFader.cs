using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour {

    private float startTime;
    private float finishTime;
    private bool toBlack;
    private Image im;

	// Use this for initialization
	void Start () {
        im = GetComponent<Image>();
        Fade(1, false);
	}
	
	// Update is called once per frame
	void Update () {
	    if(Time.time < finishTime) {
            if (toBlack)
                im.color = new Color(0, 0, 0, (Time.time - startTime) / (finishTime - startTime));
            else
                im.color = new Color(0, 0, 0, 1 - (Time.time - startTime) / (finishTime - startTime));
        }
	}

    public void Fade (float _timeToFade, bool _toBlack) {
        startTime = Time.time;
        finishTime = startTime + _timeToFade;
        toBlack = _toBlack;
    }
}
