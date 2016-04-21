using UnityEngine;
using System.Collections;

public class PressurePad : MonoBehaviour {

    public int ID;
    public bool inverted = false;
    public bool singlePress = false;
    private bool beenPressed = false;
    private bool activate = false;
    private bool powered = false;
    

    private Rect footprint;

    void Start() {
        footprint = new Rect(
            this.transform.position.x - this.transform.localScale.x / 2, 
            this.transform.position.z - this.transform.localScale.z / 2, 
            this.transform.localScale.x, 
            this.transform.localScale.z
            );
    }

    // Update is called once per frame
    void Update() {
        if (!(singlePress && beenPressed) && transform.parent.GetComponent<Room>().RoomActive) {
            bool beingPressed = SomethingOnMe();
            if (beingPressed) {
                beenPressed = true;
                if (!inverted) {
                    activate = true;
                } else {
                    activate = false;
                }
            } else if (inverted) {
                activate = true;
            } else {
                activate = false;
            }
        }
    }

    public bool Powered {
        get {
            return powered;
        }
        set {
            powered = value;
        }
    }

    public bool Activated {
        get {
            return activate;
        }
        set {

        }
    }

    public void Reset() {
        activate = false;
        powered = false;
        beenPressed = false;
    }

    private bool SomethingOnMe() {
        Ray ray = new Ray(transform.position, Vector3.up);
        Debug.DrawRay(ray.origin, ray.direction);
        if(Physics.Raycast(ray, 2)) {
            return true;
        }
        ray = new Ray(new Vector3(footprint.xMin + footprint.width/2, this.transform.position.y, footprint.yMin), Vector3.up);
        Debug.DrawRay(ray.origin, ray.direction);
        if (Physics.Raycast(ray, 2)) {
            return true;
        }
        ray = new Ray(new Vector3(footprint.xMin +footprint.width/2, this.transform.position.y, footprint.yMax), Vector3.up);
        Debug.DrawRay(ray.origin, ray.direction);
        if (Physics.Raycast(ray, 2)) {
            return true;
        }
        ray = new Ray(new Vector3(footprint.xMin, this.transform.position.y, footprint.yMax - footprint.height/2), Vector3.up);
        Debug.DrawRay(ray.origin, ray.direction);
        if (Physics.Raycast(ray, 2)) {
            return true;
        }
        ray = new Ray(new Vector3(footprint.xMax, this.transform.position.y, footprint.yMax - footprint.height/2), Vector3.up);
        Debug.DrawRay(ray.origin, ray.direction);
        if (Physics.Raycast(ray, 2)) {
            return true;
        }

        return false;
    }
}
