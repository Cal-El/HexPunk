using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MeleeEnemyAnimations : NetworkBehaviour
{
    public string Idle, Battlecry, Chasing, Attacking, Dead;

    private MeleeAIBehaviour ai;
    private MeleeAIBehaviour.STATES previousState;
    private Animator ani;

    // Use this for initialization
    void Start()
    {
        ai = GetComponent<MeleeAIBehaviour>();
        ani = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (previousState != ai.animationState)
        {
            switch (ai.agentState)
            {
                case MeleeAIBehaviour.STATES.Idle:
                    ani.Play(Idle);
                    break;
                case MeleeAIBehaviour.STATES.Battlecry:
                    ani.Play(Battlecry);
                    break;
                case MeleeAIBehaviour.STATES.Chasing:
                    ani.Play(Chasing);
                    break;
                case MeleeAIBehaviour.STATES.Attacking:
                    ani.Play(Attacking);
                    break;
                case MeleeAIBehaviour.STATES.Dead:
                    ani.Play(Dead);
                    break;
            }
        }

        if (ai.animationState != MeleeAIBehaviour.STATES.Dead) {
            ani.SetLayerWeight(0, 1 - ai.navAgent.velocity.magnitude / ai.navAgent.speed);
            ani.SetLayerWeight(1, ai.navAgent.velocity.magnitude / ai.navAgent.speed);
        } else {
            ani.SetLayerWeight(0, 1);
            ani.SetLayerWeight(1, 0);
        }

        previousState = ai.animationState;
    }
}
