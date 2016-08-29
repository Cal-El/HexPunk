using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerCommands : NetworkBehaviour {

    private GameObject playerCamera;

    private bool isBetrayer = false;
    [SyncVar(hook = "OnVictory")]
    public bool Victory = false;
    [SyncVar (hook = "OnDefeat")]
    public bool Defeat = false;
    
    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (playerCamera == null)
            playerCamera = gameObject.GetComponent<PlayerMovement>().playerCamera;
    }

    //Used for betrayer setup
    public bool IsBetrayer
    {
        get
        {
            return isBetrayer;
        }

        set
        {
            if (isLocalPlayer)
            {
                playerCamera.GetComponentInChildren<PlayerGUICanvas>().IsBetrayer = value;
                isBetrayer = value;
            }
        }
    }

    private void OnDefeat(bool value)
    {
        if (isLocalPlayer)
        {
            playerCamera.GetComponentInChildren<PlayerGUICanvas>().Defeat = value;
        }
    }

    private void OnVictory(bool value)
    {
        if (isLocalPlayer)
        {
            playerCamera.GetComponentInChildren<PlayerGUICanvas>().Victory = value;
        }
    }
}
