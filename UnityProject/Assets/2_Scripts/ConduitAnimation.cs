using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ConduitAnimation : NetworkBehaviour {

    ConduitAbilities ca;
    ConduitAbilities.ANIMATIONSTATES previousState;
    Animator ani;

	// Use this for initialization
	void Start () {
        ca = GetComponent<ConduitAbilities>();
        ani = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if(previousState != ca.currentState)
        {
            switch (ca.currentState)
            {
                case ClassAbilities.ANIMATIONSTATES.Idle:
                    CmdPlay("Armature|Idle");
                    break;
                case ClassAbilities.ANIMATIONSTATES.Running:
                    CmdPlay("Armature|Run Cycle");
                    break;
                case ClassAbilities.ANIMATIONSTATES.Ability1:
                    CmdPlay("Armature|Punch 1");
                    break;
                case ClassAbilities.ANIMATIONSTATES.Ability2:
                    CmdPlay("Armature|Stomp");
                    break;
                case ClassAbilities.ANIMATIONSTATES.Ability3:
                    CmdPlay("Armature|DrawPower");
                    break;
                case ClassAbilities.ANIMATIONSTATES.Ability4:
                    CmdPlay("Armature|Dash");
                    break;
                case ClassAbilities.ANIMATIONSTATES.Ability5:
                    CmdPlay("Armature|Res1");
                    break;
                case ClassAbilities.ANIMATIONSTATES.Dead:
                    CmdPlay("Armature|Death");
                    break;
                case ClassAbilities.ANIMATIONSTATES.Revived:
                    CmdPlay("Armature|GetRevd");
                    break;
            }
        }
        
        previousState = ca.currentState;
    }

    [Command]
    private void CmdPlay(string animation)
    {
        if (!isClient) ani.Play(animation);
        RpcPlay(animation);
    }

    [ClientRpc]
    private void RpcPlay(string animation)
    {
        ani.Play(animation);
    }
}
