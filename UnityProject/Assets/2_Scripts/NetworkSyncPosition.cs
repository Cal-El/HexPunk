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

    [ClientRpc]
    public void RpcServerSetPosition(Vector3 pos)
    {
        transform.position = pos;
        syncPos = pos;
    }
}
