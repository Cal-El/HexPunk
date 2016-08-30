using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerStats : NetworkBehaviour {

    public bool isBetrayer = false; 
    public float damageTaken = 0;
    public float damageDealt = 0;
    public float kills = 0;
    public float deaths = 0;
    public float alliesRevived = 0;

    [Command]
    public void CmdAddDamageTaken(float value)
    {
        damageTaken += value;
    }

    [Command]
    public void CmdAddDamageDealt(float value)
    {
        damageDealt += value;
    }

    [Command]
    public void CmdAddKills(float value)
    {
        kills += value;
    }

    [Command]
    public void CmdAddDeaths(float value)
    {
        deaths += value;
    }

    [Command]
    public void CmdAddRevives(float value)
    {
        alliesRevived += value;
    }
}
