using UnityEngine;
using System.Collections;

public class PressurePad : Pad {

    public int ID;
    private bool powered = false;

    void Start() {
        Footprint();
    }

    // Update is called once per frame
    void Update() {
        if(transform.parent.GetComponent<Room>().RoomActive)
            ShouldBeActive();
    }

    public bool Powered {
        get {
            return powered;
        }
        set {
            powered = value;
        }
    }

    public override void Reset() {
        base.Reset();
        powered = false;
    }
}
