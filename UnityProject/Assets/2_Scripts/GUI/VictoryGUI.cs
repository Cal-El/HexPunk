using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets.Network;

public class VictoryGUI : MonoBehaviour {

    public void ReturnToMenu()
    {
        FindObjectOfType<LobbyManager>().ServerChangeScene("Lobby");
    }
}
