using UnityEngine;
using System.Collections;

public class ExplosivePadGraphic : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.parent.GetComponent<ExplosivePad>().Activate) {
            if (transform.parent.GetComponent<ExplosivePad>().fake) {
                this.GetComponent<Renderer>().material.color = Color.green;
            } else {
                this.GetComponent<Renderer>().material.color = Color.red;
            }

        } else {
            this.GetComponent<Renderer>().material.color = Color.black;
        }
    }
}
