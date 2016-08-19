using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour {

    public int ID;
    public bool roomActive;
    public bool roomUnlocked;
    public enum ALIGNMENTS { Good, Neutral, Bad}
    public ALIGNMENTS roomAlignment = ALIGNMENTS.Neutral;
    public List<Spawner> spawners;
    [TextArea(2,10)]
    public string message;

    void Start() {

    }

    void Update() {

    }

    public void UnlockRoom() {
        if (!roomUnlocked) {
            if(message.Length > 0) {
                FindObjectOfType<TextMessage>().SendText(message);
            }
            roomUnlocked = true;
            roomActive = true;
            foreach (Spawner s in spawners)
                s.ActivateSpawner(roomAlignment);
        }
    }

    public void AddSpawner(Spawner s) {
        if(spawners == null) {
            spawners = new List<Spawner>();
        }
        spawners.Add(s);
    }
}
