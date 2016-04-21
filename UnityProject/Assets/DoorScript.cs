using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour {

    public bool open = false;
    public float timeToOpen = 2.0f;
    public Vector3 openPosition;
    private Vector3 closedPosition;
    private float openess = 0.0f;

	// Use this for initialization
	void Start () {
        closedPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        if (transform.parent.GetComponent<Room>().doorOpen)
            openess = Mathf.Clamp(openess + Time.deltaTime / timeToOpen, 0, 1);
        else
            openess = Mathf.Clamp(openess - Time.deltaTime / timeToOpen, 0, 1);
        transform.position = Vector3.Lerp(closedPosition, openPosition, openess);
	}
}
