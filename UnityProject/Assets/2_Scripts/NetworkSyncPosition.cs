using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkSyncPosition : NetworkBehaviour {

    [SyncVar][HideInInspector]
    public Vector3 syncPos;

    [SerializeField]
    private Transform myTransform;
    [SerializeField]
    private float lerpRate = 15;

    private Vector3 lastPos;
    private float threshold = 0.1f;

    void Start()
    {
        if (!isLocalPlayer) return;
        myTransform = transform;
        var pos = FindSpawnPoint();
        myTransform.position = pos;
        CmdProvidePositionToServer(pos);
        lastPos = pos;
    }

    void Update()
    {
        LerpPosition();
    }
	
	void FixedUpdate () {
        TransmitPosition();
    }

    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
        }
    }

    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
        syncPos = pos;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer && Vector3.Distance(myTransform.position, lastPos) > threshold)
        {
            CmdProvidePositionToServer(myTransform.position);
            lastPos = myTransform.position;
        }
    }

    private Vector3 FindSpawnPoint()
    {
        Vector3 spawnPoint = Vector3.zero;

        if (name.Contains("Conduit"))
        {
            spawnPoint = GameObject.Find("ConduitSpawn").transform.position;
        }
        else if (name.Contains("Aethersmith"))
        {
            spawnPoint = GameObject.Find("AethersmithSpawn").transform.position;
        }
        else if (name.Contains("Caldera"))
        {
            spawnPoint = GameObject.Find("CalderaSpawn").transform.position;
        }
        else if (name.Contains("Shard"))
        {
            spawnPoint = GameObject.Find("ShardSpawn").transform.position;
        }

        return spawnPoint;
    }
}
