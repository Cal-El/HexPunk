using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ClassAbilities : Character {

    public enum ANIMATIONSTATES { Idle, Running, Ability1, Ability2, Ability3, Ability4, Ability5, Dead, Revived };
    public ANIMATIONSTATES currentState;

    public Transform graphicObj;
    protected PlayerMovement pm;
    public float healthMax = 100;
    [SyncVar]
    public float health;
    public float energyMax = 100;
    [SyncVar]
    public float energy;
    protected float level = 0;

    private bool isAlive = true;
    public bool IsReviving { get; set; }
    protected Ability Revive;

    private float knockbackTimer = 0;

    protected bool isAxisInUse1 = false;
    protected bool isAxisInUse2 = false;
    protected bool isAxisDown1 = false;
    protected bool isAxisDown2 = false;

    [System.Serializable]
    public class Ability
    {
        [HideInInspector]
        public int abilityNum;
        public float baseDmg;
        public float castingTime;
        public float cooldown;
        public float energyCost;
        public float range;
        public float knockbackStr;

        public Ability(int abilityNum = 0, float baseDmg = 0, float castingTime = 0, float cooldown = 0, float energyCost = 0, float range = 0, float knockbackStr = 0)
        {
            this.abilityNum = abilityNum;
            this.baseDmg = baseDmg;
            this.castingTime = castingTime;
            this.cooldown = cooldown;
            this.energyCost = energyCost;
            this.range = range;
            this.knockbackStr = knockbackStr;
        }
    }

    //protected struct Ability {
    //    public int abilityNum;
    //    public float baseDmg;
    //    public float castingTime;
    //    public float cooldown;
    //    public float range;
    //    public float energyCost;
    //}

    protected float currCooldown = 0.0f;
    protected float castingTimer = 0.0f;
    protected int waitingForAbility = 0; //0 means none

    protected void Initialize() {
        DontDestroyOnLoad(gameObject);
        health = healthMax;
        pm = GetComponent<PlayerMovement>();

        //Setup revive ability which is used by all classes
        Revive.abilityNum = 5;
        Revive.castingTime = 1f;
        Revive.cooldown = 0.25f;
        Revive.range = 1.0f;

        base.Initialise();
    }

    protected void BaseUpdate()
    {
        if(!rb.isKinematic) {
            knockbackTimer -= Time.deltaTime;
            if(knockbackTimer <= 0) {
                rb.isKinematic = true;
            }
        }

        health = Mathf.Max(Mathf.Min(health, healthMax), 0f);
        energy = Mathf.Max(Mathf.Min(energy, energyMax), 0f);

        //Used for testing
        if (Input.GetKeyDown(KeyCode.O))
        {
            CmdAddHealth(10);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            CmdAddHealth(-10);
        }

        //FAKE IT TIL WE MAKE IT
        if (Input.GetKeyDown(KeyCode.Keypad1))
            CmdChangeGraphicColour(Color.cyan);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            CmdChangeGraphicColour(Color.yellow);
        if (Input.GetKeyDown(KeyCode.Keypad3))
            CmdChangeGraphicColour(Color.red);
        if (Input.GetKeyDown(KeyCode.Keypad4))
            CmdChangeGraphicColour(Color.green);
        if (Input.GetKeyDown(KeyCode.Keypad5))
            CmdChangeGraphicColour(Color.magenta);
        if (Input.GetKeyDown(KeyCode.Keypad6))
            CmdChangeGraphicColour(Color.blue);
        if (Input.GetKeyDown(KeyCode.Keypad7))
            CmdChangeGraphicColour(Color.black);

        if (Input.GetKeyDown(KeyCode.I))
            level += 0.1f;

        if (Input.GetKeyDown(KeyCode.K))
            Knockback(-transform.forward *1000, 1);

        if (currCooldown <= 0)
        {
            if (!isAlive)
            {
                currentState = ANIMATIONSTATES.Dead;
            }
            else if (pm.IsMoving)
            {
                currentState = ANIMATIONSTATES.Running;
            }
            else
            {
                currentState = ANIMATIONSTATES.Idle;
            }
        }
    }

    public virtual void GainXP(float xp) {
        level += xp;
    }

    public float GetLevel() {
        return level;
    }

    public override float GetHealth() {
        return health;
    }

    public override void TakeDmg(float dmg) {
        health = Mathf.Clamp(health - dmg, 0, healthMax);
    }

    public override void Heal(float addedHP) {
        health = Mathf.Clamp(health + addedHP, 0, healthMax);
    }

    public override void Knockback(Vector3 force, float timer) {
        //
        //  OH
        //
        //  GOD
        //
        //  WHY
        //

        //rb.isKinematic = false;
        //rb.AddForce(force);
        //knockbackTimer = timer;

    }

    //Used for testing
    [Command]
    private void CmdAddHealth(float hp)
    {
        if (!isClient) health += hp;
        RpcAddHealth(hp);
    }

    [ClientRpc]
    private void RpcAddHealth(float hp)
    {
        health += hp;
    }

    [Command]
    protected void CmdChangeGraphicColour(Color colour)
    {
        if (!isClient) graphicObj.GetComponent<SkinnedMeshRenderer>().material.SetColor("_EmissionColor", colour);
        RpcChangeGraphicColour(colour);
    }

    [ClientRpc]
    protected void RpcChangeGraphicColour(Color colour)
    {
        graphicObj.GetComponent<SkinnedMeshRenderer>().material.SetColor("_EmissionColor", colour);
    }

    #region Death and Respawn

    public bool IsAlive
    {
        get
        {
            return isAlive;
        }

        set
        {
            EnableCharacter(value);
            isAlive = value;
        }

    }

    protected virtual void EnableCharacter(bool enabled)
    {
        if (enabled) health = 80;
        var cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = enabled;

        if (pm != null) pm.ControlEnabled = enabled;
    }

    [Command]
    protected void CmdDeath()
    {
        if (!isClient)
        {
            currentState = ANIMATIONSTATES.Dead;
            IsAlive = false;
            //Put death animation and stuff here
        }
        RpcDeath();
    }

    [ClientRpc]
    protected void RpcDeath()
    {
        currentState = ANIMATIONSTATES.Dead;
        IsAlive = false;
        //Put death animation and stuff here
    }

    [Command]
    protected void CmdCallRevive(GameObject o)
    {
        if (!isClient)
        {
            var ca = o.GetComponent<ClassAbilities>();
            ca.IsReviving = true;
        }
        RpcCallRevive(o);
    }

    [ClientRpc]
    protected void RpcCallRevive(GameObject o)
    {

        var ca = o.GetComponent<ClassAbilities>();
        ca.IsReviving = true;
        ca.health = 80;
    }

    [Command]
    protected void CmdRevive()
    {
        if (!isClient)
        {
            IsAlive = true;
        }
        RpcRevive();
    }

    [ClientRpc]
    protected void RpcRevive()
    {
        IsAlive = true;
    }

    #endregion

    protected virtual void UseAbility(Ability a) {
        if (currCooldown <= 0) {
            currentState = ANIMATIONSTATES.Running + a.abilityNum;
            Debug.Log(a.abilityNum + " " + currentState);
            castingTimer = a.castingTime;
            waitingForAbility = a.abilityNum;
            currCooldown = a.castingTime + a.cooldown;
        }
    }

    protected bool GetAxisDown1(string axis)
    {
        if (Input.GetAxisRaw(axis) != 0)
        {
            if (isAxisInUse1 == false)
            {
                isAxisDown1 = true;
                isAxisInUse1 = true;
            } else
            {
                isAxisDown1 = false;
            }
        }
        if (Input.GetAxisRaw(axis) == 0)
        {
            isAxisInUse1 = false;
            isAxisDown1 = false;
        }
        return isAxisDown1;
    }

    protected bool GetAxisDown2(string axis)
    {
        if (Input.GetAxisRaw(axis) != 0)
        {
            if (isAxisInUse2 == false)
            {
                isAxisDown2 = true;
                isAxisInUse2 = true;
            }
            else
            {
                isAxisDown2 = false;
            }
        }
        if (Input.GetAxisRaw(axis) == 0)
        {
            isAxisInUse2 = false;
            isAxisDown2 = false;
        }
        return isAxisDown2;
    }
}
