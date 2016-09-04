using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GeneralNetworking : NetworkBehaviour {

    /// <summary>
    /// Sets a syncvar on the server and then syncs it.
    /// </summary>
    [ServerCallback]
    public static void SetSyncVar<T>(T syncVar, T value)
    {
        syncVar = value;
    }

    /// <summary>
    /// Instantiates a gameobject and spawns it across the network.
    /// </summary>
    /// <returns>The GameObject spawned by the server.</returns>
    [ServerCallback]
    public static GameObject ServerSpawn(GameObject o, Vector3 pos, Quaternion rot)
    {
        GameObject spawn = Instantiate(o, pos, rot) as GameObject;

        NetworkServer.Spawn(spawn);
        return spawn;
    }
}
