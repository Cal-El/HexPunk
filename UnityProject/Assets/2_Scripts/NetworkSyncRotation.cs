using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkSyncRotation : NetworkBehaviour
{

    [SyncVar]
    private float syncPlayerRotation;

    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private float lerpRate = 25;

    private float lastRot;
    private float threshold = 1;

    void Update()
    {
        LerpRotation();
    }

    void FixedUpdate()
    {
        TransmitRotations();
    }

    void LerpRotation()
    {
        if (!isLocalPlayer)
        {
            LerpPlayerRotation(syncPlayerRotation);
        }
    }

    void LerpPlayerRotation(float rotAngle)
    {
        Vector3 playerNewRot = new Vector3(0, rotAngle, 0);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, Quaternion.Euler(playerNewRot), lerpRate * Time.deltaTime);
    }

    [Command]
    void CmdProvideRotationToServer(float playerRot)
    {
        syncPlayerRotation = playerRot;
    }

    [Client]
    void TransmitRotations()
    {
        if (isLocalPlayer)
        {
            if (CheckIfBeyondThreshold(playerTransform.localEulerAngles.y, lastRot))
            {
                lastRot = playerTransform.localEulerAngles.y;
                CmdProvideRotationToServer(lastRot);
            }
        }
    }

    bool CheckIfBeyondThreshold(float rot1, float rot2)
    {
        if (Mathf.Abs(rot1 - rot2) > threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [Client]
    void OnPlayerRotSync(float latestPlayerRot)
    {
        syncPlayerRotation = latestPlayerRot;
    }

    [Command]
    public void CmdSetStartRot()
    {
        if (!isClient)
        {
            syncPlayerRotation = 90;
            playerTransform.rotation = Quaternion.Euler(0, 90, 0);
            lastRot = 90;
        }
        RpcSetStartRot();
    }

    [ClientRpc]
    public void RpcSetStartRot()
    {
        syncPlayerRotation = 90;
        playerTransform.rotation = Quaternion.Euler(0, 90, 0);
        lastRot = 90;
    }
}
