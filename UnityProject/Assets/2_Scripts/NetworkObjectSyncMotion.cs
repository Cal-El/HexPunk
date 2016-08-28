using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkObjectSyncMotion : NetworkBehaviour {

    [SyncVar]
    private Vector3 syncPos;
    [SyncVar]
    private float syncYRot;

    private Vector3 lastPos;
    private Quaternion lastRot;
    private Transform myTransform;
    private float lerpRate = 15;
    private float posThreshold = 0.1f;
    private float rotThreshold = 5;

    // Use this for initialization
    void Start () {
        myTransform = transform;
	}

    // Update is called once per frame
    void Update()
    {
        TransmitMotion();
        LerpMotion();
    }

    [ServerCallback]
    void TransmitMotion()
    {
        if(Vector3.Distance(myTransform.position, lastPos) > posThreshold || Quaternion.Angle(myTransform.rotation, lastRot) > rotThreshold)
        {
            lastPos = myTransform.position;
            lastRot = myTransform.rotation;

            syncPos = myTransform.position;
            syncYRot = myTransform.localEulerAngles.y;
        }
    }

    [ClientCallback]
    void LerpMotion()
    {
        myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);

        Vector3 newRot = new Vector3(0, syncYRot, 0);
        myTransform.rotation = Quaternion.Lerp(myTransform.rotation, Quaternion.Euler(newRot), Time.deltaTime * lerpRate);
    }
}
