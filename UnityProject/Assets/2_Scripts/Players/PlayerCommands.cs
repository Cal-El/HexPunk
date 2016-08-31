using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
/// <summary>
/// Used to send information to the local player camera.
/// </summary>
public class PlayerCommands : NetworkBehaviour {

    private GameObject playerCamera;

    [SyncVar (hook = "OnBetrayerChanged")]
    public bool IsBetrayer = false;
    [SyncVar(hook = "OnVictory")]
    public bool Victory = false;
    [SyncVar (hook = "OnDefeat")]
    public bool Defeat = false;
	
	// Update is called once per frame
	void Update () {
        if (playerCamera == null)
            playerCamera = gameObject.GetComponent<PlayerMovement>().playerCamera;
    }

    private void OnBetrayerChanged(bool value)
    {
        if (isLocalPlayer)
        {
            playerCamera.GetComponentInChildren<PlayerGUICanvas>().IsBetrayer = value;
            IsBetrayer = value;
        }
    }

    private void OnDefeat(bool value)
    {
        if (isLocalPlayer)
        {
            playerCamera.GetComponentInChildren<PlayerGUICanvas>().Defeat = value;
            Defeat = value;
        }
    }

    private void OnVictory(bool value)
    {
        if (isLocalPlayer)
        {
            playerCamera.GetComponentInChildren<PlayerGUICanvas>().Victory = value;
            Victory = value;
        }
    }
}
