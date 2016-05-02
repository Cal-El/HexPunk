using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour {

    public char triggerMessage;
    public enum TYPES { Door, Aggro}
    public TYPES triggerType = TYPES.Aggro;

    void OnTriggerEnter(Collider e) {
        if (e.tag == "Player") {
            if (triggerType == TYPES.Aggro)
                foreach (MeleeAIBehaviour enemy in FindObjectsOfType<MeleeAIBehaviour>())
                    enemy.ReceiveMessage(triggerMessage);
            else if (triggerType == TYPES.Door)
                foreach (DoorScript door in FindObjectsOfType<DoorScript>())
                    door.ReceiveMessage(triggerMessage);
            Destroy(this.gameObject);
        }
    }
}
