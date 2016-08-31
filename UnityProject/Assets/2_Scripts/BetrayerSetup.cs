using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class BetrayerSetup : NetworkBehaviour
{
    public float percentChanceToBeBetrayer = 5;
    private List<GameObject> playersOnTrigger = new List<GameObject>();
    [SyncVar]
    private bool betrayerChosen = false;

    // Update is called once per frame
    void Update()
    {
        if (!betrayerChosen)
        {
            if (playersOnTrigger.Count >= 4) StartBetrayerSetup();
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
            var betrayer = playersOnTrigger[Random.Range(0, playersOnTrigger.Count)];
            betrayer.GetComponent<PlayerStats>().isBetrayer = true;
            betrayer.GetComponent<PlayerCommands>().IsBetrayer = true;
            betrayerChosen = true;
        }
    }
}
