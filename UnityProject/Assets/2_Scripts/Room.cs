using UnityEngine;
using System;
using System.Collections;

public class Room : MonoBehaviour {

    public int ID;
    public bool roomActive;
    public bool roomUnlocked;
    public enum ALIGNMENTS { Good, Neutral, Bad}
    public ALIGNMENTS roomAlignment = ALIGNMENTS.Neutral;
    public Action<ALIGNMENTS> activateSpawners; 

    void Start() {

    }

    void Update() {

    }

    public void UnlockRoom() {
        if (!roomUnlocked) {
            roomUnlocked = true;
            roomActive = true;
            activateSpawners(roomAlignment);
        }
    }
}
