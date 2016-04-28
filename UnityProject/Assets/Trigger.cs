using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour {

    public char triggerMessage;

    void OnTriggerEnter(Collider e) {
        if (e.tag == "Player")
            foreach (MeleeAIBehaviour enemy in FindObjectsOfType<MeleeAIBehaviour>())
                enemy.ReceiveMessage(triggerMessage);

    }
}
