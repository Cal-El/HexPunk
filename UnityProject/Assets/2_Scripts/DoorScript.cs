using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour {

    public bool open = false;
    public float timeToOpen = 2.0f;

    [System.Serializable] public struct DOOR {
        public Transform obj;
        public Vector3 openedPos;
        public Vector3 closedPos;
    }

    public DOOR[] doors;
    
    private float openess = 0.0f;
	
	// Update is called once per frame
	void Update () {
        if (open)
            openess = Mathf.Clamp(openess + Time.deltaTime / timeToOpen, 0, 1);
        else
            openess = Mathf.Clamp(openess - Time.deltaTime / timeToOpen, 0, 1);

        for (int i = 0; i < doors.Length; i++)
            doors[i].obj.localPosition = Vector3.Lerp(doors[i].closedPos, doors[i].openedPos, openess);
	}
}
