using UnityEngine;
using System.Collections;

public class EmissiveColorChanger : MonoBehaviour {

    public Color targetColour = Color.black;
    private Color currentColour = Color.white;
    public Material targetMat;

	// Use this for initialization
	void Start () {
	    if(targetMat != null) {
            currentColour = targetMat.GetColor("_EmissionColor");
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (targetMat != null) {
            if (targetColour != Color.black) {
                currentColour = Color.Lerp(currentColour, targetColour, Time.deltaTime);
                targetMat.SetColor("_EmissionColor", currentColour);
            } else {
                Color rainbow = new Color(
                    Mathf.Sin(Time.timeSinceLevelLoad) / 2 + 0.5f,
                    Mathf.Sin(Time.timeSinceLevelLoad + Mathf.PI * (2 / 3.0f)) / 2 + 0.5f,
                    Mathf.Sin(Time.timeSinceLevelLoad + Mathf.PI * (4 / 3.0f)) / 2 + 0.5f
                    );
                currentColour = Color.Lerp(currentColour, rainbow, Time.deltaTime * 10);
                targetMat.SetColor("_EmissionColor", currentColour);
            }
        }
	}
}
