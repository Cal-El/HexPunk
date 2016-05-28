﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Megamanager : MonoBehaviour {

    public static Megamanager MM;
    public GameObject[] players;
    public struct RoomStruct {
        public Room room;
        public int ID;
        public int[] parents;
    }
    public RoomStruct[] roomTree;

	// Use this for initialization
	void Start () {
        MM = this;
        players = GameObject.FindGameObjectsWithTag("Player");
        RoomTreeSetup();
    }
	
	// Update is called once per frame
	void Update () {
        players = GameObject.FindGameObjectsWithTag("Player");

        /*foreach (RoomStruct r in roomTree) {
            if(r.parents != null) {
                r.room.roomUnlocked = IsRoomUnlocked(r);
            } else {
                r.room.roomUnlocked = true;
            }
        }*/
	}

    bool IsRoomUnlocked(RoomStruct r) {
        foreach(int i in r.parents) {
            if (!roomTree[r.parents[i]].room.doorOpen) return false;
        }
        return true;
    }

    void RoomTreeSetup() {
        Room[] rooms = FindObjectsOfType<Room>();
        roomTree = new RoomStruct[rooms.Length];
        foreach (Room r in rooms) {
            roomTree[r.ID].room = r;
            roomTree[r.ID].ID = r.ID;
        }

        //roomTree[0].parents = null;
        //roomTree[1].parents = new int[] { 0 };
    }

    public static GameObject FindClosestAttackable(GameObject toMe, int passNum) {
        SortedDictionary<float, GameObject> listOfThings = new SortedDictionary<float, GameObject>();
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Character")) {
            if (g != toMe) {
                float newValue = Vector3.Distance(toMe.transform.position, g.transform.position);
                if(!listOfThings.ContainsKey(newValue))
                listOfThings.Add(newValue,g);
            }
        }
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")) {
            if (g != toMe) {
                float newValue = Vector3.Distance(toMe.transform.position, g.transform.position);
                if (!listOfThings.ContainsKey(newValue))
                    listOfThings.Add(newValue, g);
            }
        }
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Destructible")) {
            if (g != toMe) {
                float newValue = Vector3.Distance(toMe.transform.position, g.transform.position);
                if (!listOfThings.ContainsKey(newValue))
                    listOfThings.Add(newValue, g);
            }
        }
        int passes = 1;
        foreach (GameObject g in listOfThings.Values) {
            if (passNum == passes)
                return g;
            passes++;
        }
        return null;
    }
}
