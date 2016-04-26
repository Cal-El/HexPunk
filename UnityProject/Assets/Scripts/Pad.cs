using UnityEngine;
using System.Collections;

public abstract class Pad : MonoBehaviour {

    public bool inverted = false;
    public bool singlePress = false;
    private bool beenPressed = false;
    private bool activate = false;
    private Rect footprint;

    protected void Footprint() {
        footprint = new Rect(
            this.transform.position.x - this.transform.localScale.x/2,
            this.transform.position.z - this.transform.localScale.z/2,
            this.transform.localScale.x,
            this.transform.localScale.z
            );
    }

    protected bool BeenPressed {
        get {
            return beenPressed;
        }
        set {
            beenPressed = value;
        }
    }

    public bool Activate {
        get {
            return activate;
        }
        set {
            activate = value;
        }
    }

    protected Rect FootPrint {
        get {
            return footprint;
        }
    }

    protected void ShouldBeActive() {
        if (!(singlePress && BeenPressed)) {
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

    protected bool SomethingOnMe() {
        RaycastHit[] hits = Physics.BoxCastAll(this.transform.position, new Vector3(transform.localScale.x/2, 1, transform.localScale.y/2), Vector3.up);
        foreach(RaycastHit hit in hits) {
            if (hit.transform.tag == "Player" || hit.transform.tag == "Character" || hit.transform.tag == "Destructible")
                return true;
        }
        return false;
    }

    public virtual void Reset() {
        Activate = false;
        BeenPressed = false;
    }
}
