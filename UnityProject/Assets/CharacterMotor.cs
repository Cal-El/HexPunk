using UnityEngine;
using System.Collections;

public class CharacterMotor : MonoBehaviour {

    public float speed = 1.0f;
    public Camera myCam;

    private CharacterController cc;
    private bool grounded;


	// Use this for initialization
	void Start () {
        cc = this.GetComponent<CharacterController>();
        myCam = Camera.main;

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Move();
        Aim();
    }

    private void Move() {

        Vector3 direction = Vector3.Normalize(myCam.transform.right*Input.GetAxis("Horizontal") +  Vector3.Cross(myCam.transform.right,Vector3.up) * Input.GetAxis("Vertical"));
        cc.Move(direction * speed * Time.deltaTime);
        Debug.Log("Input: " + direction + ", Position: " + transform.position);
    }

    private void Aim() {
        Quaternion currRot = transform.rotation;
        Ray sh = myCam.ScreenPointToRay(Input.mousePosition);
        Vector3 point = sh.origin + sh.direction *Mathf.Abs(sh.origin.y/sh.direction.y);
        Debug.DrawLine(sh.origin, point);
        transform.LookAt(point, Vector3.up);
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));
        transform.rotation = Quaternion.Lerp(currRot, transform.rotation, Time.deltaTime * 5);
        
    }


}
