﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Megamanager : MonoBehaviour {

    public static Megamanager MM;
    public ClassAbilities[] players;
    public List<Character> characters;
    [System.Serializable]
    public class RoomConnection {
        public Room r1;
        public Room r2;
        public DoorScript door;

        public void UnlockConnection() {
            door.open = true;
            r1.UnlockRoom();
            r2.UnlockRoom();
        }
    }
    public RoomConnection[] roomTree;

	// Use this for initialization
	void Start () {
        if (MM != null) {
            Destroy(this);
        } else {
            MM = this;
        }
        players = FindObjectsOfType<ClassAbilities>();
    }
	
	// Update is called once per frame
	void Update () {
        players = FindObjectsOfType<ClassAbilities>();

        if (characters.Count == 0 && AllRoomsUnlocked()) Victory();

        int deadPlayers = 0;
        foreach (ClassAbilities player in players)
        {
            if (!player.IsAlive) deadPlayers++;
        }

        if (deadPlayers == players.Length) Defeated();

    }
    
    private void Defeated()
    {
        foreach(ClassAbilities player in players)
        {
            player.GetComponent<PlayerCommands>().Defeat = true;
        }
    }

    private void Victory()
    {
        foreach (ClassAbilities player in players)
        {
            player.GetComponent<PlayerCommands>().Victory = true;
        }
    }

    public void UnlockConnection(int ID) {
        roomTree[ID].UnlockConnection();
    }

    void OnDestroy() {
        MM.roomTree = this.roomTree;
    }

    bool AllRoomsUnlocked() {
        return false;
    }

    public static Character FindClosestAttackable(Character toMe, int passNum) {
        SortedDictionary<float, Character> listOfThings = new SortedDictionary<float, Character>();
        foreach (Character g in GetAllCharacters()) {
            if (g != toMe) {
                float newValue = Vector3.Distance(toMe.transform.position, g.transform.position);
                if(!listOfThings.ContainsKey(newValue))
                listOfThings.Add(newValue,g);
            }
        }
       
        int passes = 1;
        foreach (Character g in listOfThings.Values) {
            if (passNum == passes)
                return g;
            passes++;
        }
        return null;
    }

    public static void AddCharacterToList(Character c) {
        if (MM.characters == null) MM.characters = new List<Character>();
        MM.characters.Add(c);
    }

    public static void RemoveCharacterFromList(Character c) {
        MM.characters.Remove(c);
    }

    public static Character[] GetAllCharacters() {
        return MM.characters.ToArray() ;
    }
}
