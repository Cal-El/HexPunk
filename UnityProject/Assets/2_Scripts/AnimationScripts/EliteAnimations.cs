using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EliteAnimations : NetworkBehaviour
{
    public string Idle, Battlecry, Chasing, MeleeAttacking, RangeAttacking, Dead;

    private EliteAIBehaviour ai;
    private EliteAIBehaviour.STATES previousState;
    private Animator ani;

    // Use this for initialization
    void Start()
    {
        ai = GetComponent<EliteAIBehaviour>();
        ani = GetComponentInChildren<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (previousState != ai.animationState)
        {
            
            switch (ai.agentState)
            {
                case EliteAIBehaviour.STATES.Idle:
                    ani.Play(Idle);
                    break;
                case EliteAIBehaviour.STATES.Battlecry:
                    ani.Play(Battlecry);
                    break;
                case EliteAIBehaviour.STATES.Chasing:
                    ani.Play(Chasing);
                    break;
                case EliteAIBehaviour.STATES.MeleeAttacking:
                    ani.Play(MeleeAttacking);
                    break;
                case EliteAIBehaviour.STATES.RangedAttacking:
                    ani.Play(RangeAttacking);
                    break;
                case EliteAIBehaviour.STATES.Dead:
                    ani.Play(Dead);
                    break;
            }
        }

        if (ai.animationState != EliteAIBehaviour.STATES.Dead) {
            ani.SetLayerWeight(0, 1 - ai.navAgent.velocity.magnitude / ai.navAgent.speed);
            ani.SetLayerWeight(1, Mathf.Lerp(ani.GetLayerWeight(1), ai.navAgent.velocity.magnitude / ai.navAgent.speed, Time.deltaTime*10));
            ani.SetFloat("Random Offset", Mathf.Max(Mathf.Lerp(ani.GetFloat("Random Offset"), ai.navAgent.velocity.magnitude / ai.navAgent.speed, Time.deltaTime*10), 0.001f));
        } else {
            ani.SetLayerWeight(0, 1);
            ani.SetLayerWeight(1, 0);
        }

        previousState = ai.animationState;
    }
}
