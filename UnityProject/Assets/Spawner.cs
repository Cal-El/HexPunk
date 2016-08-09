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
	
	public void ActivateSpawner(Room.ALIGNMENTS alignment) {
        switch (alignment) {
            case Room.ALIGNMENTS.Good:
                Instantiate(goodSpawn, transform.position, transform.rotation);
                break;
            case Room.ALIGNMENTS.Neutral:
                Instantiate(neutralSpawn, transform.position, transform.rotation);
                break;
            case Room.ALIGNMENTS.Bad:
                Instantiate(evilSpawn, transform.position, transform.rotation);
                break;
        }
    }
}
