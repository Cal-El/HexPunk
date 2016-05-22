using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerMovement : NetworkBehaviour {

	private CharacterController controller;
    public GameObject playerCamera;
    public Camera myCam;
    private bool controlEnabled =true;
    
    private float xAxis = 0;
	private float yAxis = 0;
	public float speed = 6.0f;
	public float leftThumbstickAngle = 0;
	private Vector3 direction = Vector3.zero;
    private bool isMoving = false;
    //private float yStart;

    void Start () {
		controller = GetComponent<CharacterController>();
        //yStart = gameObject.transform.position.y;
        if(myCam == null) {
            myCam = Camera.main;
        }
    }
	
	void Update() {

        if (!isLocalPlayer || !controlEnabled)
        {
            return;
        }

        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");
        if(xAxis == 0 && yAxis == 0)
        {
            isMoving = false;
        } else
        {
            isMoving = true;
        }

        direction = ((myCam.transform.right * xAxis) + (Vector3.Cross(myCam.transform.right, Vector3.up) * yAxis));
        Aim(direction + transform.position);
        if (direction.magnitude >= 1)
        {
            direction = direction.normalized;
        }
        controller.Move(direction * speed * Time.deltaTime);
        
        //gameObject.transform.position = new Vector3(gameObject.transform.position.x, yStart, gameObject.transform.position.z);
        //Debug.Log("Input: " + direction + ", Position: " + transform.position);

        //// Find the angle the thumstick is pointed
        //if (xAxis != 0.0f || yAxis != 0.0f) {
        //	leftThumbstickAngle = Mathf.Atan2(yAxis, xAxis) * Mathf.Rad2Deg;
        //} else {
        //	leftThumbstickAngle = 0;
        //}
    }

    public override void OnStartLocalPlayer()
    {
        //GetComponent<MeshRenderer>().material.color = Color.blue;

        playerCamera = Instantiate(playerCamera);
        playerCamera.GetComponent<PlayerCamera>().myPlayer = gameObject;
        myCam = playerCamera.GetComponent<PlayerCamera>().cam;
    }

    private void Aim(Vector3 point)
    {
        transform.LookAt(point, Vector3.up);
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));
        //transform.rotation = Quaternion.Lerp(currRot, transform.rotation, Time.deltaTime * 5);
    }

    public bool ControlEnabled
    {
        get
        {
            return controlEnabled;
        }
        set
        {
            if (value != controlEnabled)
            {
                Quaternion currRot = transform.rotation;
                Ray sh = myCam.ScreenPointToRay(Input.mousePosition);
                Vector3 point = sh.origin + sh.direction * Mathf.Abs(sh.origin.y / sh.direction.y);
                Aim(point);
                controlEnabled = value;
            }
        }
    }

    public bool IsMoving
    {
        get
        {
            return isMoving;
        }
    }
}
