using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkSyncRotation : NetworkBehaviour {

    [SyncVar]
    private Quaternion syncPlayerRotation;

    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private float lerpRate = 15;

	void FixedUpdate () {
        TransmitRotations();
        LerpRotation();
	}

    void LerpRotation()
    {
        if (!isLocalPlayer)
        {
            playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, syncPlayerRotation, Time.deltaTime * lerpRate);
        }
    }

    [Command]
    void CmdProvideRotationToServer(Quaternion playerRot)
    {
        syncPlayerRotation = playerRot;
    }

    [Client]
    void TransmitRotations()
    {
        if (isLocalPlayer)
        {
            CmdProvideRotationToServer(playerTransform.rotation);
        }
    }
}
