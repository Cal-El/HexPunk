using UnityEngine;
using System.Collections;

public class RoomOnePuzzle : MonoBehaviour {

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
        RoomOneSetup();
    }
	
	// Update is called once per frame
	void Update () {
	    for(int i = 0; i < PowerTree.Length; i++) {
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
        if(door != null && PowerTree[PowerTree.Length - 1].plate.Powered) {
            Debug.Log("Beep boop. Door open");
        }
	}

    void RoomOneSetup() {
        PowerTree = new Plate[6];

        PowerTree[0].ID = 0;
        PowerTree[0].plate = GetPlateWithID(PowerTree[0].ID);
        PowerTree[0].parents = null;

        PowerTree[1].ID = 1;
        PowerTree[1].plate = GetPlateWithID(PowerTree[1].ID);
        PowerTree[1].parents = new int[1];
        PowerTree[1].parents[0] = PowerTree[0].ID;

        PowerTree[2].ID = 2;
        PowerTree[2].plate = GetPlateWithID(PowerTree[2].ID);
        PowerTree[2].parents = new int[1];
        PowerTree[2].parents[0] = PowerTree[1].ID;

        PowerTree[3].ID = 3;
        PowerTree[3].plate = GetPlateWithID(PowerTree[3].ID);
        PowerTree[3].parents = new int[1];
        PowerTree[3].parents[0] = PowerTree[2].ID;

        PowerTree[4].ID = 4;
        PowerTree[4].plate = GetPlateWithID(PowerTree[4].ID);
        PowerTree[4].parents = new int[1];
        PowerTree[4].parents[0] = PowerTree[3].ID;

        PowerTree[5].ID = 5;
        PowerTree[5].plate = GetPlateWithID(PowerTree[5].ID);
        PowerTree[5].parents = new int[1];
        PowerTree[5].parents[0] = PowerTree[3].ID;
    }

    PressurePad GetPlateWithID(int u) {
        foreach(PressurePad p in plates) {
            if (p.ID == u) return p;
        }
        return null;
    }

    bool CheckParentsForPower(int p) {
        foreach(int i in PowerTree[p].parents) {
            if (!PowerTree[i].plate.Powered || !PowerTree[i].plate.Activated) return false;
        }
        return true;
    }
}
