using UnityEngine;
using System.Collections;

public class LittleTrapRoom : Room {

    BinaryPad[] Pads = new BinaryPad[3];

	// Use this for initialization
	void Start () {
        Pads = GetComponentsInChildren<BinaryPad>();
	}
	
	// Update is called once per frame
	void Update () {
        bool allInput = true;
        foreach (BinaryPad bp in Pads) {
            if(bp.Inp == 0) {
                allInput = false;
            }
        }
        if (allInput)
        {
            bool success = true;
            foreach (BinaryPad bp in Pads){
                if (bp.Inp <= 0) {
                    success = false;
                }
            }
            if (success){
                doorOpen = true;
            }
        }
	}
}
