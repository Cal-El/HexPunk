using UnityEngine;
using System.Collections;

public class IceField : MonoBehaviour {
    [HideInInspector]
    public float range;
    public float duration = 0.1f;
    private float halfLife;
    private float timer;

    void Start()
    {
        halfLife = Time.time + duration / 20;
        timer = Time.time + duration;
        range *= 2;
        transform.localScale = new Vector3(range, transform.localScale.y, range);
    }

    // Update is called once per frame
    void Update ()
    {
        if (Time.time < halfLife) transform.localScale += Vector3.up * Time.deltaTime * 50;
        if (Time.time > timer) Destroy(gameObject);
	}
}
