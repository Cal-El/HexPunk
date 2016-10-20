using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class EndGameGUI : MonoBehaviour
{
    public PlayerGUICanvas myGui;
    public Text missionStatus;
    public string victoryText;
    public string defeatText;
    public List<Image> alliesWinImgs = new List<Image>();
    public List<Image> betrayerWinImgs = new List<Image>();
    private string betrayerName;

    public void BackToLobby()
    {
        myGui.myPlayer.GetComponent<PlayerCommands>().ReturnToMenu();
    }

    public void Victory()
    {
        betrayerName = FindBetrayerClass();
        missionStatus.text = victoryText;
        if (myGui.myPlayer.GetComponent<PlayerCommands>().IsBetrayer)
        {
            FilterImagesByClass(betrayerWinImgs, betrayerName);
            DestroyImgs(alliesWinImgs);
        }
        else
        {
            FilterImagesByClass(alliesWinImgs, betrayerName);
            DestroyImgs(betrayerWinImgs);
        }
    }

    public void Defeat()
    {
        betrayerName = FindBetrayerClass();
        missionStatus.text = defeatText;
        if (myGui.myPlayer.GetComponent<PlayerCommands>().IsBetrayer)
        {
            FilterImagesByClass(alliesWinImgs, betrayerName);
            DestroyImgs(betrayerWinImgs);
        }
        else
        {
            FilterImagesByClass(betrayerWinImgs, betrayerName);
            DestroyImgs(alliesWinImgs);
        }
    }

    /// <summary>
    /// Filters img list with name of betrayer that won or lost
    /// </summary>
    private void FilterImagesByClass(List<Image> imgs, string className)
    {
        if (className.Contains("Conduit"))
        {
            FilterImages(imgs, "Conduit");
        }

        if (className.Contains("Aethersmith"))
        {
            FilterImages(imgs, "Aethersmith");
        }

        if (className.Contains("Caldera"))
        {
            FilterImages(imgs, "Caldera");
        }

        if (className.Contains("Shard"))
        {
            FilterImages(imgs, "Shard");
        }
    }

    private void FilterImages(List<Image> imgs, string className)
    {
        foreach (var img in imgs)
        {
            if (img != null && !img.gameObject.name.Contains(className))
            {
                Destroy(img.gameObject);
            }
        }
    }

    private void DestroyImgs(List<Image> imgs)
    {
        foreach (var img in imgs)
        {
            if(img != null) Destroy(img.gameObject);
        }
    }

    private string FindBetrayerClass()
    {
        foreach (var player in FindObjectsOfType<PlayerCommands>())
        {
            if (player.IsBetrayer)
            {
                var tempName = player.gameObject.name;
                if (tempName.Contains("Conduit")) return "Conduit";
                if (tempName.Contains("Aethersmith")) return "Aethersmith";
                if (tempName.Contains("Caldera")) return "Caldera";
                if (tempName.Contains("Shard")) return "Shard";
            }
        }
        return "";
    }
}
