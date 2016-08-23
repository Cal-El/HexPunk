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
    private List<Character> enemys;

    void Start() {
        enemys = new List<Character>();
    }

    void Update() {
        foreach(Character c in enemys) {
            if (c == null) enemys.Remove(c);
        }
    }

    public void UnlockRoom() {
        if (!roomUnlocked) {
            if(message.Length > 0) {
                FindObjectOfType<TextMessage>().SendText(message);
            }
            roomUnlocked = true;
            roomActive = true;
            foreach (Spawner s in spawners) {
                Character c = s.ActivateSpawner(roomAlignment).GetComponent<Character>();
                if(c != null) {
                    enemys.Add(c);
                }
            }
        }
    }

    public void RemoveCharacter(Character c) {
        enemys.Remove(c);
    }

    public void AddSpawner(Spawner s) {
        if(spawners == null) {
            spawners = new List<Spawner>();
        }
        spawners.Add(s);
    }

    public List<Character> Enemies() {
        return enemys;
    }
}
