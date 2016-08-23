using UnityEngine;
using System.Collections;

public class StaticStompVisual : MonoBehaviour {

    public float lifeTime = 1;
    private float lifeTimer = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        lifeTimer += Time.deltaTime;
        //transform.localScale = new Vector3(lifeTimer, lifeTimer, lifeTimer);
        if (lifeTimer >= lifeTime) {
            Destroy(this.gameObject);
        }
	}
}
