﻿using UnityEngine;
using System.Collections;

public class RoomTwoPuzzle : MonoBehaviour {

    public PressurePad[] plates;
    public GameObject door;
    public bool RoomActive;


    public struct Plate {
        public PressurePad plate;
        public int ID;
        public int[] parents;
    }
    private Plate[] PowerTree;

    // Use this for initialization
    void Start () {
        plates = GetComponentsInChildren<PressurePad>();
        RoomTwoSetup();
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < PowerTree.Length; i++) {
            if (PowerTree[i].plate != null) {
                if (PowerTree[i].parents == null) {
                    PowerTree[i].plate.Powered = true;
                } else {
                    PowerTree[i].plate.Powered = CheckParentsForPower(i);
                }
            } else {
                Debug.Log("PlateID " + i + " not set");
            }
        }
        if (PowerTree[0].plate.Activated) {
            PowerTree[1].plate.Reset();
            PowerTree[2].plate.Reset();
            PowerTree[3].plate.Reset();
        }
    }

    void RoomTwoSetup() {
        PowerTree = new Plate[4];

        PowerTree[0].ID = 0;
        PowerTree[0].plate = GetPlateWithID(PowerTree[0].ID);
        PowerTree[0].parents = null;

        PowerTree[1].ID = 1;
        PowerTree[1].plate = GetPlateWithID(PowerTree[1].ID);
        PowerTree[1].parents = null;

        PowerTree[2].ID = 2;
        PowerTree[2].plate = GetPlateWithID(PowerTree[2].ID);
        PowerTree[2].parents = null;

        PowerTree[3].ID = 3;
        PowerTree[3].plate = GetPlateWithID(PowerTree[3].ID);
        PowerTree[2].parents = null;
    }

    PressurePad GetPlateWithID(int u) {
        foreach (PressurePad p in plates) {
            if (p.ID == u) return p;
        }
        return null;
    }

    bool CheckParentsForPower(int p) {
        foreach (int i in PowerTree[p].parents) {
            if (!PowerTree[i].plate.Powered || !PowerTree[i].plate.Activated) return false;
        }
        return true;
    }
}
