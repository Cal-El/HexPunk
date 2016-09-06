using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class StatMap : MonoBehaviour, IDragHandler
{
    public PlayerCamera playerCamera;
    private GameObject player;
    private GameObject[] players;
    public RectTransform myTransform;

    public PlayerStatsGUI[] playerStatGUIs;

    public void OnDrag(PointerEventData eventData)
    {
        myTransform.localPosition += new Vector3(eventData.delta.x, eventData.delta.y, 0);
    }

    void Start()
    {
        player = playerCamera.myPlayer;
        players = GameObject.FindGameObjectsWithTag("Player");

        int currentPlayer = 0;
        for (int i = 0; i < playerStatGUIs.Length; i++)
        {
            if (i == 0)
            {
                SetStatGUI(player.name, playerStatGUIs[i], player.GetComponent<PlayerStats>());
            }
            else
            {
                if (players[currentPlayer] == player)
                {
                    currentPlayer++;
                }

                SetStatGUI(players[currentPlayer].name, playerStatGUIs[i], players[currentPlayer].GetComponent<PlayerStats>());

                currentPlayer++;
            }
        }
    }

    void SetStatGUI(string playerName, PlayerStatsGUI gui, PlayerStats stats)
    {
        gui.PlayerName = playerName;
        gui.IsBetrayer = stats.isBetrayer;
        gui.damageDealt.text = string.Format("Damage Dealt: {0}", stats.damageDealt);
        gui.damageTaken.text = string.Format("Damage Taken: {0}", stats.damageTaken);
        gui.kills.text = string.Format("Enemies Killed: {0}", stats.kills);
        gui.playerKills.text = string.Format("Players Killed: {0}", stats.playerKills);
        gui.deaths.text = string.Format("Deaths: {0}", stats.deaths);
        gui.alliesRevived.text = string.Format("Players Revived: {0}", stats.alliesRevived);
    }
}
