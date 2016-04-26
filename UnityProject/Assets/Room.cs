using UnityEngine;
using System.Collections;

public abstract class Room : MonoBehaviour {

    public int ID;
    public bool doorOpen;
    public bool RoomActive;
    public bool roomUnlocked;
    public PressurePad[] plates;

    public struct Plate {
        public PressurePad plate;
        public int ID;
        public int[] parents;
    }
    public Plate[] PowerTree;

    public PressurePad GetPlateWithID(int u) {
        foreach (PressurePad p in plates) {
            if (p.ID == u) return p;
        }
        return null;
    }

    public bool CheckParentsForPower(int p) {
        foreach (int i in PowerTree[p].parents) {
            if (!PowerTree[i].plate.Powered || !PowerTree[i].plate.Activate) return false;
        }
        return true;
    }
}
