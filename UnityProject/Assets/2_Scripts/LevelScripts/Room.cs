using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

public class Room : NetworkBehaviour {

    public int ID;
    [SyncVar]
    public bool roomActive;
    [SyncVar]
    public bool roomUnlocked;
    public enum ALIGNMENTS { Good, Neutral, Bad}
    public ALIGNMENTS roomAlignment = ALIGNMENTS.Neutral;
    public List<Spawner> spawners;
    [TextArea(2,10)]
    public string message;
    public AudioClip bossTalk;
    private AudioSource ads;
    private List<Character> enemys;

    void Start() {
        if(isServer) enemys = new List<Character>();
        if(bossTalk != null)
        {
            ads = gameObject.AddComponent<AudioSource>();
            ads.clip = bossTalk;
            ads.loop = false;
            ads.spatialBlend = 0;
            ads.volume = 1;
            ads.playOnAwake = false;
        }
    }

    void Update() {
        if (isServer)
        {
            int i = 0;
            while (i < enemys.Count)
            {
                if (enemys[i] == null) enemys.Remove(enemys[i]);
                else { i++; }
            }
        }
    }
        
    public void UnlockRoom() {
        if (!roomUnlocked) {            
            if (isServer)
            {
                roomUnlocked = true;
                roomActive = true;
                foreach (Spawner s in spawners)
                {
                    Character c = s.ActivateSpawner(roomAlignment).GetComponent<Character>();
                    if (c != null)
                    {
                        enemys.Add(c);
                    }
                }
            }

            if (message.Length > 0)
            {
                FindObjectOfType<TextMessage>().SendText(message);
            }

            if (ads != null)
            {
                ads.Play();
            }
        }
    }

    [ServerCallback]
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
