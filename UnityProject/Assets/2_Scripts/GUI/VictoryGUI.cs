using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets.Network;

public class VictoryGUI : NetworkBehaviour {

    public void ReturnToMenu()
    {
        FindObjectOfType<LobbyManager>().ReturnToLobby();
    }
}
