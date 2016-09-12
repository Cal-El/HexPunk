using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Network;

public class NetworkSyncPosition : NetworkBehaviour {

    [HideInInspector]
    public string currentSceneName;

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
        if (isLocalPlayer) CmdClientSetServerPos(LobbyManager.FindSpawnPoint(gameObject));
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
        if (isLocalPlayer)
        {
            if (Vector3.Distance(myTransform.position, lastPos) > threshold)
            {
                CmdProvidePositionToServer(myTransform.position);
                lastPos = myTransform.position;
            }
        }
    }

    [Command]
    public void CmdClientSetServerPos(Vector3 pos)
    {
        if (!isClient)
        {
            myTransform.position = pos;
            CmdProvidePositionToServer(pos);
            lastPos = pos;
        }
        RpcServerSetPosition(pos);
    }

    [ClientRpc]
    public void RpcServerSetPosition(Vector3 pos)
    {
        myTransform.position = pos;
        CmdProvidePositionToServer(pos);
        lastPos = pos;
    }
}
