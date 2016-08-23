using UnityEngine;
using System.Collections;

public class EmissiveColorChanger : MonoBehaviour {

    public Color targetColour = Color.black;
    public float cycleOffset = 0;
    public float speed = 1;
    private Color currentColour = Color.white;
    public Material[] targetMats;

	// Use this for initialization
	void Start () {
	    if(targetMats.Length > 0) {
            currentColour = this.GetComponent<Renderer>().material.GetColor("_EmissionColor");
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (targetMats.Length > 0) {
            if (targetColour != Color.black) {
                currentColour = Color.Lerp(currentColour, targetColour, Time.deltaTime);
                this.GetComponent<Renderer>().material.SetColor("_EmissionColor", currentColour);
            } else {
                Color rainbow = new Color(
                    Mathf.Sin(speed*Time.timeSinceLevelLoad + cycleOffset*Mathf.PI) / 2 + 0.5f,
                    Mathf.Sin(speed * Time.timeSinceLevelLoad + Mathf.PI * (2 / 3.0f)+cycleOffset * Mathf.PI) / 2 + 0.5f,
                    Mathf.Sin(speed * Time.timeSinceLevelLoad + Mathf.PI * (4 / 3.0f)+cycleOffset * Mathf.PI) / 2 + 0.5f
                    );
                currentColour = Color.Lerp(currentColour, rainbow, Time.deltaTime * 10);
                this.GetComponent<Renderer>().material.SetColor("_EmissionColor", currentColour);
            }
        }
        
	}
}
