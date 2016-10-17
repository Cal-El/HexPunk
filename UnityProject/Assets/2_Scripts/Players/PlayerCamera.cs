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
    [SerializeField]
    private bool isTrailerCam = false;
    private Vector3 positionOffset = Vector3.zero;

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

        if (isTrailerCam) {
            if (Input.GetKey(KeyCode.UpArrow)) {
                transform.Rotate(transform.right, Time.deltaTime*30, Space.World);
            }
            if (Input.GetKey(KeyCode.DownArrow)) {
                transform.Rotate(transform.right, -Time.deltaTime * 30, Space.World);
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
                transform.Rotate(Vector3.up, Time.deltaTime * 30, Space.World);
            }
            if (Input.GetKey(KeyCode.LeftArrow)) {
                transform.Rotate(Vector3.up, -Time.deltaTime * 30, Space.World);
            }
            if (Input.GetKey(KeyCode.PageUp)) {
                cam.transform.localPosition += Vector3.forward* Time.deltaTime * 5;
            }
            if (Input.GetKey(KeyCode.PageDown)) {
                cam.transform.localPosition -= Vector3.forward * Time.deltaTime * 5;
            }
            if (Input.GetKey(KeyCode.Keypad7)) {
                positionOffset += Vector3.right * Time.deltaTime * 5;
            }
            if (Input.GetKey(KeyCode.Keypad4)) {
                positionOffset -= Vector3.right * Time.deltaTime * 5;
            }
            if (Input.GetKey(KeyCode.Keypad8)) {
                positionOffset += Vector3.up * Time.deltaTime * 5;
            }
            if (Input.GetKey(KeyCode.Keypad5)) {
                positionOffset -= Vector3.up * Time.deltaTime * 5;
            }
            if (Input.GetKey(KeyCode.Keypad9)) {
                positionOffset += Vector3.forward * Time.deltaTime * 5;
            }
            if (Input.GetKey(KeyCode.Keypad6)) {
                positionOffset -= Vector3.forward * Time.deltaTime * 5;
            }

        } else {
            if (!useTopDown)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(viewAngle[viewAngleIndex]), Time.deltaTime * 10);
            else
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, viewAngle[viewAngleIndex].y, 0)), Time.deltaTime * 10);
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0));
        }
        //Find Target Location
        Vector3 targetPos = myPlayer.transform.position + positionOffset;
        //transform.position = targetPos;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime*5);
	}
}
