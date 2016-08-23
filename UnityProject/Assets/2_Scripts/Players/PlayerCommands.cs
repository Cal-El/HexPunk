using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerCommands : NetworkBehaviour {

    private GameObject playerCamera;

    private bool isBetrayer = false;
    private bool victory = false;
    private bool defeat = false;
    
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

    public bool Victory
    {
        get
        {
            return victory;
        }

        set
        {
            if (isLocalPlayer)
            {
                playerCamera.GetComponentInChildren<PlayerGUICanvas>().Victory = value;
                victory = value;
            }
        }
    }

    public bool Defeat
    {
        get
        {
            return defeat;
        }

        set
        {
            if (isLocalPlayer)
            {
                playerCamera.GetComponentInChildren<PlayerGUICanvas>().Defeat = value;
                defeat = value;
            }
        }
    }
}
