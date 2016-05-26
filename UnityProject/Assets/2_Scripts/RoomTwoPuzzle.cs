using UnityEngine;
using System.Collections;

public class RoomTwoPuzzle : Room {

    private bool[] lastFrame;
    private int[] code;
    private float resetTimer = 0f;
    private bool resetting = false;

    // Use this for initialization
    void Start () {
        plates = GetComponentsInChildren<PressurePad>();
        code = new int[] { 1, -1, -1, -1 };
        RoomTwoSetup();
    }
	
	// Update is called once per frame
	void Update () {
        if (!doorOpen && roomUnlocked) {
            RoomActive = true;
            if (PowerTree[0].plate.Activate) {
                Reset();
            }

            if (resetting) {
                resetTimer -= Time.deltaTime;
                if(resetTimer <= 0) {
                    Reset();
                }
            }


            for (int i = 1; i < PowerTree.Length; i++) {
                if (PowerTree[i].plate.Activate && !lastFrame[i] && code[0] <= 3) {
                    code[code[0]] = PowerTree[i].ID;
                    code[0]++;
                }
                if (PowerTree[i].plate != null) {
                    if (PowerTree[i].parents == null) {
                        PowerTree[i].plate.Powered = true;
                    } else {
                        PowerTree[i].plate.Powered = CheckParentsForPower(i);
                    }
                } 
                lastFrame[i] = PowerTree[i].plate.Activate;
            }
            if (code[0] == 4 && !resetting) {
                if (code[1] == 3 && code[2] == 1 && code[3] == 4) {
                    doorOpen = true;
                } else {
                    resetTimer = 1;
                    resetting = true;
                }
            }
            
        } else {
            RoomActive = false;
        }
    }

    void Reset() {
        resetting = false;
        resetTimer = 0f;
        code = new int[] { 1, -1, -1, -1 };
        PowerTree[1].plate.Reset();
        PowerTree[2].plate.Reset();
        PowerTree[3].plate.Reset();
        PowerTree[4].plate.Reset();
        PowerTree[5].plate.Reset();
        lastFrame[1] = false;
        lastFrame[2] = false;
        lastFrame[3] = false;
        lastFrame[4] = false;
        lastFrame[5] = false;
    }

    void RoomTwoSetup() {
        PowerTree = new Plate[6];

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
        PowerTree[3].parents = null;

        PowerTree[4].ID = 4;
        PowerTree[4].plate = GetPlateWithID(PowerTree[4].ID);
        PowerTree[4].parents = null;

        PowerTree[5].ID = 5;
        PowerTree[5].plate = GetPlateWithID(PowerTree[5].ID);
        PowerTree[5].parents = null;

        lastFrame = new bool[PowerTree.Length];
    }
}
