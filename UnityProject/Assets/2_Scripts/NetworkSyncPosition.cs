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
	
	void FixedUpdate () {
        TransmitPosition();
        LerpPosition();
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
            CmdProvidePositionToServer(myTransform.position);
        }
    }
}
