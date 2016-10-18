using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets.Network;

public class VictoryGUI : NetworkBehaviour {

    public PlayerGUICanvas myGui;

    public void ReturnToMenu()
    {
        myGui.myPlayer.GetComponent<PlayerCommands>().ReturnToMenu();
    }
}
