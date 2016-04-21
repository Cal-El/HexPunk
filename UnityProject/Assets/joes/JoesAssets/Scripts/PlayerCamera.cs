using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

    public GameObject myPlayer;
    public float timeToShift = 1;
    public Vector3 viewAngle = new Vector3(20, 45, 0);
    public Camera cam;
    public bool topDownView = false;
    private float lerpVal = 1;

	// Use this for initialization
	void Start () {
        cam = GetComponentInChildren<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.B))
            topDownView = !topDownView;

        if (topDownView)
            lerpVal = Mathf.Clamp(lerpVal + Time.deltaTime / timeToShift, 0, 1);
        else
            lerpVal = Mathf.Clamp(lerpVal - Time.deltaTime / timeToShift, 0, 1);

        transform.rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(viewAngle), lerpVal);
        
        //Find Target Location
        Vector3 targetPos = myPlayer.transform.position;
        //transform.position = targetPos;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime*5);
	}
}
