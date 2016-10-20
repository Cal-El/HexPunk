using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour {

    public char triggerMessage;
    public enum TYPES { Door, Aggro}
    public TYPES triggerType = TYPES.Aggro;
    public int connectionID;

    void OnTriggerEnter(Collider e) {
        if (e.tag == "Player") {
            if (triggerType == TYPES.Aggro)
                foreach (MeleeAIBehaviour enemy in FindObjectsOfType<MeleeAIBehaviour>())
                    enemy.ReceiveMessage(triggerMessage);
            else if (triggerType == TYPES.Door)
                if(Megamanager.MM != null) Megamanager.MM.UnlockConnection(connectionID);
            Destroy(this.gameObject);
        }
    }
}
