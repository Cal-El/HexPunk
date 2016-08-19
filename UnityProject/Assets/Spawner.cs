using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public Room myRoom;
    [Header("Good")]
    public GameObject goodSpawn;
    [Header("Neutral")]
    public GameObject neutralSpawn;
    [Header("Evil")]
    public GameObject evilSpawn;

    // Use this for initialization
    void Start () {
        myRoom.AddSpawner(this);

    }
	
	public GameObject ActivateSpawner(Room.ALIGNMENTS alignment) {
        switch (alignment) {
            case Room.ALIGNMENTS.Good:
                return Instantiate(goodSpawn, transform.position, transform.rotation) as GameObject;
                break;
            case Room.ALIGNMENTS.Neutral:
                return Instantiate(neutralSpawn, transform.position, transform.rotation) as GameObject;
                break;
            case Room.ALIGNMENTS.Bad:
                return Instantiate(evilSpawn, transform.position, transform.rotation) as GameObject;
                break;
            default:
                return null;
                break;
        }
    }
}
