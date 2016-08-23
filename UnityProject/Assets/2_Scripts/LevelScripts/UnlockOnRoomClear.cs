using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof (Room))]

public class UnlockOnRoomClear : MonoBehaviour {

    private Room myRoom;
    [SerializeField] private int connectionID;

	// Use this for initialization
	void Start () {
        myRoom = GetComponent<Room>();
	}
	
	// Update is called once per frame
	void Update () {
        if(myRoom.roomUnlocked && myRoom.Enemies().Count <= 0) {
            Megamanager.MM.UnlockConnection(connectionID);
        }
    }
}
