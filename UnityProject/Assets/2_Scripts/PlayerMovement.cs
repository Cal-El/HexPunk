using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerMovement : NetworkBehaviour {

	private CharacterController controller;
    private ClassAbilities myPlayer;
    public GameObject playerCamera;
    public Camera myCam;
    private bool isCasting = false;
    private bool controlEnabled = true;
    
    private float xAxis = 0;
	private float yAxis = 0;
	public float speed = 6.0f;
	public float leftThumbstickAngle = 0;
	private Vector3 direction = Vector3.zero;
    private bool isMoving = false;
    //private float yStart;

    void Start () {
		controller = GetComponent<CharacterController>();
        if(myCam == null) {
            myCam = Camera.main;
        }
        myPlayer = GetComponent<ClassAbilities>();
    }
	


	void FixedUpdate() {

        if (!isLocalPlayer || !controlEnabled || isCasting || !myPlayer.rb.isKinematic)
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
        
        controller.Move(direction * speed * Time.fixedDeltaTime);
    }

    public override void OnStartLocalPlayer()
    {
        playerCamera = Instantiate(playerCamera);
        playerCamera.GetComponent<PlayerCamera>().myPlayer = gameObject;
        myCam = playerCamera.GetComponent<PlayerCamera>().cam;
    }

    private void Aim(Vector3 point)
    {
        transform.LookAt(point, Vector3.up);
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));
    }

    public bool ControlEnabled
    {
        get
        {
            return controlEnabled;
        }
        set
        {
            controlEnabled = value;
        }
    }

    public bool IsCasting
    {
        get
        {
            return isCasting;
        }
        set
        {
            if (value != isCasting)
            {
                if (myCam != null)
                {
                    Quaternion currRot = transform.rotation;
                    Ray sh = myCam.ScreenPointToRay(Input.mousePosition);
                    Vector3 point = sh.origin + sh.direction * Mathf.Abs(sh.origin.y / sh.direction.y);
                    Aim(point);
                }
                isCasting = value;
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
