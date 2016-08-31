using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerStats : NetworkBehaviour {

    public bool isBetrayer = false; 
    [SyncVar (hook = "OnDamageTakenChanged")]
    public float damageTaken = 0;
    [SyncVar(hook = "OnDamageDealtChanged")]
    public float damageDealt = 0;
    [SyncVar(hook = "OnKillsChanged")]
    public float kills = 0;
    [SyncVar(hook = "OnPlayerKillsChanged")]
    public float playerKills = 0;
    [SyncVar(hook = "OnDeathsChanged")]
    public float deaths = 0;
    [SyncVar(hook = "OnAlliesRevivedChanged")]
    public float alliesRevived = 0;

    [Command]
    public void CmdAddDamageTaken(float value)
    {
        damageTaken += value;
    }

    private void OnDamageTakenChanged(float value)
    {
        damageTaken = value;
    }

    [Command]
    public void CmdAddDamageDealt(float value)
    {
        damageDealt += value;
    }

    private void OnDamageDealtChanged(float value)
    {
        damageDealt = value;
    }

    [Command]
    public void CmdAddKills(float value)
    {
        kills += value;
    }

    private void OnKillsChanged(float value)
    {
        kills = value;
    }

    [Command]
    public void CmdAddPlayerKills(float value)
    {
        playerKills += value;
    }

    private void OnPlayerKillsChanged(float value)
    {
        playerKills = value;
    }

    [Command]
    public void CmdAddDeaths(float value)
    {
        deaths += value;
    }
    
    public void OnDeathsChanged(float value)
    {
        deaths = value;
    }

    [Command]
    public void CmdAddRevives(float value)
    {
        alliesRevived += value;
    }

    private void OnAlliesRevivedChanged(float value)
    {
        alliesRevived = value;
    }
}
