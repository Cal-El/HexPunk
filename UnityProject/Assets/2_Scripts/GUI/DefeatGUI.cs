using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets.Network;

public class DefeatGUI : NetworkBehaviour
{
    public PlayerGUICanvas myGui;
    public Image[] images = new Image[4];

    public void TryAgain()
    {
        myGui.myPlayer.GetComponent<PlayerCommands>().ReturnToMenu();
        foreach(PlayerCommands pc in FindObjectsOfType<PlayerCommands>())
        {
            if (pc.IsBetrayer) FilterImages(pc.gameObject.name);
        }
    }

    private void FilterImages(string playerName)
    {
        if (playerName.Contains("Conduit")) DeleteImages("Conduit");
        if (playerName.Contains("Aethersmith")) DeleteImages("Aethersmith");
        if (playerName.Contains("Caldera")) DeleteImages("Caldera");
        if (playerName.Contains("Shard")) DeleteImages("Shard");
    }

    private void DeleteImages(string playerName)
    {
        foreach (Image img in images)
        {
            if (!img.gameObject.name.Contains(playerName)) Destroy(img.gameObject);
        }
    }
}
