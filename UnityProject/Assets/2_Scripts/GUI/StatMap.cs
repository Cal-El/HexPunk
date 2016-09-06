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
        for(int i = 0; i < playerStatGUIs.Length; i++)
        {
            if (i == 0)
            {
                SetStatGUI(player.name, player.GetComponent<PlayerCommands>().IsBetrayer, playerStatGUIs[i]);
            }
            else
            {
                if (players[currentPlayer] != player)
                {
                    SetStatGUI(players[currentPlayer].name, players[currentPlayer].GetComponent<PlayerCommands>().IsBetrayer, playerStatGUIs[i]);
                }
                else
                {
                    currentPlayer++;
                }
            }
        }
    }

    void SetStatGUI(string playerName, bool isBetrayer, PlayerStatsGUI gui)
    {
        gui.PlayerName = playerName;
        gui.IsBetrayer = isBetrayer;
    }
}
