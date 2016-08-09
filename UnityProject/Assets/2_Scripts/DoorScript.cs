using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour {

    public bool open = false;
    public float timeToOpen = 2.0f;
    public Transform[] doors;
    private Vector3[] openedPositions = new Vector3[2] {
        new Vector3(0.0151f, -0.0006f, 0),
        new Vector3(-0.065f, -0.0006f, 0.05f)
    };
    private Vector3[] closedPositions = new Vector3[2] {
        new Vector3(-0.0049f, -0.0006f, 0),
        new Vector3(-0.04503f, -0.0006f, 0.05f)
    };
    private float openess = 0.0f;
	
	// Update is called once per frame
	void Update () {
        if (open)
            openess = Mathf.Clamp(openess + Time.deltaTime / timeToOpen, 0, 1);
        else
            openess = Mathf.Clamp(openess - Time.deltaTime / timeToOpen, 0, 1);

        for (int i = 0; i < doors.Length; i++)
            doors[i].transform.localPosition = Vector3.Lerp(closedPositions[i], openedPositions[i], openess);
	}
}
