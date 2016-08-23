using UnityEngine;
using System.Collections;

public class BubbleScript : MonoBehaviour {
    public float lifetime = 5;
	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, lifetime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
