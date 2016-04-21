using UnityEngine;
using System.Collections;

public class PressurePadGraphic : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.parent.GetComponent<PressurePad>().Activated) {
            this.GetComponent<Renderer>().material.color = Color.green;
        } else {
            this.GetComponent<Renderer>().material.color = Color.red;
        }
	}
}
