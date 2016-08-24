using UnityEngine;
using System.Collections;

public class Eruption : MonoBehaviour {
    [HideInInspector]
    public float range;
    public float duration = 0.3f;
    private float timer;
    public ParticleSystem p;

    void Start()
    {
        timer = Time.time + duration;
        range *= 2;
        //transform.localScale = new Vector3(range, transform.localScale.y, range);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timer) {
            p.transform.parent = null;
            p.enableEmission = false;
            p.loop = false;
            Destroy(p.gameObject, 5);
            Destroy(gameObject); }
    }
}
