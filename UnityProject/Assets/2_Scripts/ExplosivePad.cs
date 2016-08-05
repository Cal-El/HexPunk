using UnityEngine;
using System.Collections;

public class ExplosivePad : Pad {

    public bool fake;
    private bool activatedLast = false;
    public float blastDamage = 5f;

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
                    Character ch = hit.transform.GetComponent<Character>();
                    if (ch != null) {

                        ch.TakeDmg(blastDamage);
                    }
                }
            }
        }
    }
}
