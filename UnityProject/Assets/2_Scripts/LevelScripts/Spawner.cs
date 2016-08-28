using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Spawner : NetworkBehaviour {

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
                return ServerSpawn(goodSpawn, transform.position, transform.rotation) as GameObject;
                break;
            case Room.ALIGNMENTS.Neutral:
                return ServerSpawn(neutralSpawn, transform.position, transform.rotation) as GameObject;
                break;
            case Room.ALIGNMENTS.Bad:
                return ServerSpawn(evilSpawn, transform.position, transform.rotation) as GameObject;
                break;
            default:
                return null;
                break;
        }
    }

    [ServerCallback]
    private GameObject ServerSpawn(GameObject o, Vector3 pos, Quaternion rot)
    {
        GameObject spawn = Instantiate(goodSpawn, transform.position, transform.rotation) as GameObject;

        NetworkServer.Spawn(spawn);
        return spawn;
    }
}
