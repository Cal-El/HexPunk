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
        lifeTimer += Time.deltaTime*50;
        transform.localScale = new Vector3(lifeTimer, 1, lifeTimer);
        if(lifeTimer >= lifeTime) {
            Destroy(this.gameObject);
        }
	}
}
