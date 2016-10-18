using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets.Network;

public class DefeatGUI : NetworkBehaviour
{
    public void TryAgain()
    {
        ReturnToLobby();
    }

    [ServerCallback]
    void ReturnToLobby()
    {
        LobbyManager.s_Singleton.ReturnToLobby();
    }
}
