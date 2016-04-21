using UnityEngine;
using System.Collections;

public class DestructibleObject : MonoBehaviour {

    public GameObject particleEffect;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TakeDmg(float dmg) {
        if (particleEffect != null) {
            GameObject.Instantiate(particleEffect, this.transform.position, this.transform.rotation);
        }
        Destroy(this.gameObject);
    }
}
