using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class AIBehaviour : Character {

    public const float TIME_FOR_NAVMESH_UPDATE = 0.5f;

    public enum STATES { Idle, Battlecry, Chasing, MeleeAttacking, RangedAttacking, Knockback, Dead }
    [HideInInspector]
    public STATES agentState = STATES.Idle;
    [HideInInspector]
    public STATES animationState = STATES.Idle;

    public SkinnedMeshRenderer mr;

    //Health Values
    [SyncVar]
    protected float health;
    public float maxHealth = 5;

    //Battlecry Variable
    [Header("Battlecry Attributes")]
    public char[] battleTriggers;           //The char triggers that can override this AI to begin attacking
    public float battlecryTime = 1.5f;      //Time it takes to complete a battlecry
    protected float battlecryTimer = 0.0f;    //Timer for use while battlecry is triggering
    public float battlecryRange = 2.0f;     //Range in which the battlecry triggers others around it
    public float playerPerceptionRange = 5.0f;


    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void ReceiveMessage(char a)
    {
        if (agentState == STATES.Idle)
        {
            foreach (char c in battleTriggers)
            {
                if (c == a)
                {
                    StartBattlecry();
                }
            }
        }
    }

    public void HearBattlecry()
    {
        if (agentState == STATES.Idle)
        {
            StartBattlecry();
        }
    }

    protected void StartBattlecry()
    {
        battlecryTimer = battlecryTime;
        agentState = STATES.Battlecry;
    }

    public override float GetHealth()
    {
        return health;
    }

    public override void Heal(float healVal)
    {
        health = Mathf.Clamp(health + healVal, 0, maxHealth);
    }

    public override void TakeDmg(float dmg, DamageType damageType = DamageType.Standard)
    {
        throw new NotImplementedException();
    }


    public override void Knockback(Vector3 force, float timer)
    {
        throw new NotImplementedException();
    }
}
