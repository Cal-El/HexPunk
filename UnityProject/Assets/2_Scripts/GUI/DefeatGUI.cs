using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets.Network;

public class DefeatGUI : NetworkBehaviour
{
    public PlayerGUICanvas myGui;

    public void TryAgain()
    {
        myGui.myPlayer.GetComponent<PlayerCommands>().ReturnToMenu();
    }
}
