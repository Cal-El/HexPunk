using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityStandardAssets.Network;

/// <summary>
/// Used to send information to and from the local player camera.
/// </summary>
public class PlayerCommands : NetworkBehaviour {

    private GameObject playerCamera;
    private PlayerGUICanvas gui;

    [SyncVar (hook = "OnBetrayerChanged")]
    public bool IsBetrayer = false;
    [SyncVar(hook = "OnVictory")]
    public bool Victory = false;
    [SyncVar (hook = "OnDefeat")]
    public bool Defeat = false;

    void Start()
    {
        var movement = gameObject.GetComponent<PlayerMovement>();
        if (!isLocalPlayer) return;
        if (movement != null) playerCamera = movement.playerCamera;
        if(playerCamera != null) gui = playerCamera.GetComponentInChildren<PlayerGUICanvas>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer) return;
        if (playerCamera == null)
            playerCamera = gameObject.GetComponent<PlayerMovement>().playerCamera;
        if (gui == null)
        {
            gui = playerCamera.GetComponentInChildren<PlayerGUICanvas>();            
        }
        else
        {
            if (IsBetrayer && !gui.IsBetrayer)
            {
                gui.IsBetrayer = true;
            }
        }
    }

    private void OnBetrayerChanged(bool value)
    {
        if (isLocalPlayer)
        {
            if(gui != null) gui.IsBetrayer = value;
        }
        IsBetrayer = value;
    }

    private void OnDefeat(bool value)
    {
        if (isLocalPlayer)
        {
            gui.Defeat = value;
        }
        Defeat = value;
    }

    private void OnVictory(bool value)
    {
        if (isLocalPlayer)
        {
            gui.Victory = value;
        }
        Victory = value;
    }

    public void ReturnToMenu()
    {
        CmdReturnToLobby();
    }

    [Command]
    void CmdReturnToLobby()
    {
        if(isServer) FindObjectOfType<LobbyManager>().ReturnToLobby();
    }
}
