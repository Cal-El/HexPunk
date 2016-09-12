using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets.Network;

public class DefeatGUI : MonoBehaviour {

    public void TryAgain()
    {
        FindObjectOfType<LobbyManager>().ServerChangeScene("Lobby");
    }
}
