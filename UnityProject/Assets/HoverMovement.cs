using UnityEngine;
using System.Collections;

public class HoverMovement : MonoBehaviour {

    [Range(0, 10)]
    public float amplitude = 1;
    [Range(0.1f, 5f)]
    public float speed = 1;
    private float timer;
    private Vector3 startPos;

	// Use this for initialization
	void Start () {
        startPos = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 0, Time.deltaTime * 120);
        transform.localPosition = startPos + Vector3.up * Mathf.Sin(Time.timeSinceLevelLoad/ speed) * amplitude;
	}
}
