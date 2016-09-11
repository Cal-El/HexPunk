using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

    public GameObject myPlayer;
    public float timeToShift = 1;
    public Vector3[] viewAngle;
    public Camera cam;
    public int viewAngleIndex = 0;
    public bool useTopDown = false;
    private float lerpVal = 1;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        cam = GetComponentInChildren<Camera>();

        viewAngle = new Vector3[] {
        new Vector3(-45, 45, 0),
        new Vector3(-45, 90, 0),
        new Vector3(-45, 135, 0),
        new Vector3(-45, 180, 0),
        new Vector3(-45, 225, 0),
        new Vector3(-45, 270, 0),
        new Vector3(-45, 315, 0),
        new Vector3(-45, 0, 0),
        };
    }
	
	// Update is called once per frame
	void Update () {
        if (myPlayer == null)
        {
            Destroy(gameObject);
            return;
        }

        /*
        if (Input.GetKeyDown(KeyCode.UpArrow))
            useTopDown = true;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            useTopDown = false;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            viewAngleIndex = (viewAngleIndex + 1) % (viewAngle.Length);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            viewAngleIndex = (viewAngleIndex - 1) % (viewAngle.Length);
            if(viewAngleIndex < 0) {
                viewAngleIndex = viewAngle.Length - 1;
            }
        }
        */

        if (!useTopDown)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(viewAngle[viewAngleIndex]), Time.deltaTime*10);
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3 (0, viewAngle[viewAngleIndex].y, 0)), Time.deltaTime*10);
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0));

        //Find Target Location
        Vector3 targetPos = myPlayer.transform.position;
        //transform.position = targetPos;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime*5);
	}
}
