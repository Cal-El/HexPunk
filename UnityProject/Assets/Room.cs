using UnityEngine;
using System.Collections;

public abstract class Room : MonoBehaviour {

    public PressurePad[] plates;
    public bool doorOpen;
    public bool RoomActive;


    public struct Plate {
        public PressurePad plate;
        public int ID;
        public int[] parents;
    }

}
