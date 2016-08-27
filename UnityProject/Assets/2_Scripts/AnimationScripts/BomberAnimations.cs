using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BomberAnimations : NetworkBehaviour
{
    public string Idle, Battlecry, Chasing, Attacking, Dead;

    private BomberAIBehaviour ai;
    private BomberAIBehaviour.STATES previousState;
    private Animator ani;

    // Use this for initialization
    void Start()
    {
        ai = GetComponent<BomberAIBehaviour>();
        ani = GetComponentInChildren<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (previousState != ai.animationState)
        {
            
            switch (ai.agentState)
            {
                case BomberAIBehaviour.STATES.Idle:
                    ani.Play(Idle);
                    break;
                case BomberAIBehaviour.STATES.Battlecry:
                    ani.Play(Battlecry);
                    break;
                case BomberAIBehaviour.STATES.Chasing:
                    ani.Play(Chasing);
                    break;
                case BomberAIBehaviour.STATES.MeleeAttacking:
                    ani.Play(Attacking);
                    break;
                case BomberAIBehaviour.STATES.Dead:
                    ani.Play(Dead);
                    break;
            }
        }

        if (ai.animationState != BomberAIBehaviour.STATES.Dead) {
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
