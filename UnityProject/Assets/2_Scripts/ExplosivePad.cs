using UnityEngine;
using System.Collections;

public class ExplosivePad : Pad {

    public bool fake;
    private bool activatedLast = false;

    void Start() {
        Footprint();
    }

    // Update is called once per frame
    void Update() {
        if (transform.parent.GetComponent<Room>().RoomActive) {
            activatedLast = Activate;
            ShouldBeActive();
            if (!activatedLast && Activate && !fake) {
                //explode to be implemented when units have health
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, 5f, transform.forward, 0);
                foreach(RaycastHit hit in hits) {
                    Debug.Log("5 damage to " + hit.transform.name);
                    if (hit.transform.tag == "Player" || hit.transform.tag == "Character" || hit.transform.tag == "Destructible") {
                        
                        hit.transform.SendMessage("TakeDmg", 5f, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }
    }
}
