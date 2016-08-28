using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class BetrayerSetup : NetworkBehaviour
{
    public float percentChanceToBeBetrayer = 5;
    private GameObject[] players = new GameObject[1];
    private List<GameObject> playersOnTrigger = new List<GameObject>();
    private bool betrayerChosen = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!betrayerChosen)
        {
            if (players.Length < 4)
            {
                players = GameObject.FindGameObjectsWithTag("Player");
            }
            else
            {
                if (playersOnTrigger.Count >= players.Length) StartBetrayerSetup();
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject != null && col.gameObject.tag == "Player")
        {
            if(!playersOnTrigger.Contains(col.gameObject)) playersOnTrigger.Add(col.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject != null && col.gameObject.tag == "Player")
        {
            if(playersOnTrigger.Contains(col.gameObject)) playersOnTrigger.Remove(col.gameObject);
        }
    }
    
    [ServerCallback]
    private void StartBetrayerSetup()
    {
        //Roll to see if there will be a betrayer
        var betrayerRoll = Random.Range(1, 100);

        if (betrayerRoll > percentChanceToBeBetrayer) //5% chance of there not to be a betrayer
        {
            var betrayer = players[Random.Range(0, players.Length - 1)];
            RpcSetup(betrayer);
        }
    }

    [ClientRpc]
    void RpcSetup(GameObject betrayer)
    {

        var betrayerBehaviour = betrayer.AddComponent<BetrayerBehaviour>();
        betrayer.GetComponent<PlayerCommands>().IsBetrayer = true;
        foreach (var player in players)
        {
            if (player != betrayer)
            {
                betrayerBehaviour.players.Add(player);
            }
        }
        betrayerChosen = true;
    }
}
