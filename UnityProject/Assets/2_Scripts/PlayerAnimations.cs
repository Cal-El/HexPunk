using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerAnimations : NetworkBehaviour
{
    public string idle, running, ability1, ability2, ability3, ability4, ability5, dead, revived;

    private ClassAbilities ca;
    private ClassAbilities.ANIMATIONSTATES previousState;
    private Animator ani;

    // Use this for initialization
    void Start()
    {
        ca = GetComponent<ConduitAbilities>();
        ani = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        if (previousState != ca.currentState)
        {
            switch (ca.currentState)
            {
                case ClassAbilities.ANIMATIONSTATES.Idle:
                    CmdPlay(idle);
                    break;
                case ClassAbilities.ANIMATIONSTATES.Running:
                    CmdPlay(running);
                    break;
                case ClassAbilities.ANIMATIONSTATES.Ability1:
                    CmdPlay(ability1);
                    break;
                case ClassAbilities.ANIMATIONSTATES.Ability2:
                    CmdPlay(ability2);
                    break;
                case ClassAbilities.ANIMATIONSTATES.Ability3:
                    CmdPlay(ability3);
                    break;
                case ClassAbilities.ANIMATIONSTATES.Ability4:
                    CmdPlay(ability4);
                    break;
                case ClassAbilities.ANIMATIONSTATES.Ability5:
                    CmdPlay(ability5);
                    break;
                case ClassAbilities.ANIMATIONSTATES.Dead:
                    CmdPlay(dead);
                    break;
                case ClassAbilities.ANIMATIONSTATES.Revived:
                    CmdPlay(revived);
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
