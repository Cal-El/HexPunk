using UnityEngine;
using System.Collections;

public class BetrayerGUI : MonoBehaviour {

    public GameObject myPlayer;
    public OtherPlayerGUI[] guiList;

    public void SetBetrayerGUI()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        var x = 0;
        foreach (var player in players)
        {
            if (player != null && player != myPlayer)
            {
                if (guiList[x] != null)
                {
                    if (guiList[x].player == null)
                        guiList[x].player = player;
                    if (x < players.Length - 1) x++;
                }
            }
        }
    }
}
