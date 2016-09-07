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
        GameObject obj = null;
        switch (alignment) {
            case Room.ALIGNMENTS.Good:
                obj = ServerSpawn(goodSpawn, transform.position, transform.rotation) as GameObject;
                break;
            case Room.ALIGNMENTS.Neutral:
                obj = ServerSpawn(neutralSpawn, transform.position, transform.rotation) as GameObject;
                break;
            case Room.ALIGNMENTS.Bad:
                obj = ServerSpawn(evilSpawn, transform.position, transform.rotation) as GameObject;
                break;
            default:
                obj = null;
                break;
        }
        return obj;
    }

    [ServerCallback]
    public GameObject ServerSpawn(GameObject o, Vector3 pos, Quaternion rot)
    {
        GameObject spawn = Instantiate(o, pos, rot) as GameObject;
        NetworkServer.Spawn(spawn);
        RpcAddMotion(spawn, pos, rot);
        return spawn;
    }

    [ClientRpc]
    void RpcAddMotion(GameObject o, Vector3 pos, Quaternion rot)
    {
        var move = o.AddComponent<NetworkObjectSyncMotion>();
        move.SetTransform(pos, rot);
    }
}
