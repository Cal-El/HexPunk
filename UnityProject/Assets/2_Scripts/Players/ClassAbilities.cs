using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class ClassAbilities : Character {

    public enum ANIMATIONSTATES { Idle, Running, Ability1, Ability2, Ability3, Ability4, Ability5, Dead, Revived };
    public ANIMATIONSTATES currentState;

    public Transform graphicObj;
    protected PlayerAudioManager pam;
    protected PlayerMovement pm;
    protected PlayerGUICanvas myGUI;
    protected HUDProperties myHud;
    [SyncVar (hook = "OnMaxHealthChanged")]
    public float healthMax = 100;
    [SyncVar (hook = "OnHealthChanged")]
    public float health;
    public float energyMax = 100;
    [SyncVar]
    public float energy;
    protected float level = 0;

    private bool isAlive = true;
    public float flatMaxHealthReviveCost = 10;
    public float percentHealthOnRevive = 20;
    [SyncVar (hook = "OnRevive")]
    public bool IsReviving = false;
    public Ability Revive = new Ability(5,0,1,0.25f,0,1,0);
    private GameObject reviveCapsule;
    protected RaycastHit reviveTarget;

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

    [System.Serializable]
    public class EnergyAddedAbility : Ability
    {
        [Tooltip("Energy added after the ability has been cast. This is not the energy cost.")]
        public float energyAdded;

        public EnergyAddedAbility(int abilityNum = 0, float baseDmg = 0, float castingTime = 0, float cooldown = 0, float energyCost = 0, float range = 0, float knockbackStr = 0, float energyAdded = 0)
            :base(abilityNum, baseDmg, castingTime, cooldown, energyCost, range, knockbackStr)
        {
            this.energyAdded = energyAdded;
        }
    }

    protected virtual void UseAbility(Ability a)
    {
        if (currCooldown <= 0)
        {
            currentState = ANIMATIONSTATES.Running + a.abilityNum;
            Debug.Log(a.abilityNum + " " + currentState);
            castingTimer = a.castingTime;
            waitingForAbility = a.abilityNum;
            currCooldown = a.castingTime + a.cooldown;
            if(myHud != null)
            {
                myHud.ShowIconCooldown(a.abilityNum, currCooldown);
            }
        }
    }

    protected float currCooldown = 0.0f;
    protected float castingTimer = 0.0f;
    protected int waitingForAbility = 0; //0 means none

    protected void Initialize() {
        DontDestroyOnLoad(gameObject);
        CmdSetHealth(healthMax);
        pm = GetComponent<PlayerMovement>();
        pam = GetComponentInChildren<PlayerAudioManager>();
        myGUI = pm.playerCamera.GetComponentInChildren<PlayerGUICanvas>();
        myHud = myGUI.myHud;
        reviveCapsule = transform.FindChild("ReviveCapsule").gameObject;
        base.Initialise();
    }

    protected void BaseUpdate()
    {
        if(myGUI == null || myHud == null)
        {
            myGUI = pm.playerCamera.GetComponentInChildren<PlayerGUICanvas>();
            myHud = myGUI.myHud;
        }

        if(!rb.isKinematic) {
            knockbackTimer -= Time.deltaTime;
            if(knockbackTimer <= 0) {
                rb.isKinematic = true;
            }
        }
        
        energy = (Mathf.Max(Mathf.Min(energy, energyMax), 0f));

        //Used for testing
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddHealth(10);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddHealth(-10);
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

    [Command]
    protected virtual void CmdSetMaxHealth(float value)
    {
        healthMax = value;
        healthMax = Mathf.Max(healthMax, 0f);
        CmdSetHealth(Mathf.Min(health, healthMax));
    }

    protected virtual void OnMaxHealthChanged(float value)
    {
        healthMax = value;
        healthMax = Mathf.Max(healthMax, 0f);
    }

    public override float GetHealth() {
        return health;
    }

    [Command]
    protected virtual void CmdSetHealth(float value)
    {
        health = value;
        health = Mathf.Max(Mathf.Min(health, healthMax), 0f);
    }

    protected virtual void OnHealthChanged(float value)
    {
        health = value;
        health = Mathf.Max(Mathf.Min(health, healthMax), 0f);
    }

    public override void TakeDmg(float dmg, DamageType damageType = DamageType.Standard) {
        CmdSetHealth(Mathf.Clamp(health - dmg, 0, healthMax));
        if (health > 0) pam.PlayTakeDamageAudio();
    }

    public override void Heal(float addedHP) {
        CmdSetHealth(Mathf.Clamp(health + addedHP, 0, healthMax));
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
    private void AddHealth(float hp)
    {
        CmdSetHealth(health + hp);
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
            if (value)
            {
                CmdSetIsReviving(false);
            }
        }

    }

    protected virtual void EnableCharacter(bool enabled)
    {
        if (enabled) CmdSetHealth(healthMax * percentHealthOnRevive / 100);
        var cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = enabled;
        reviveCapsule.SetActive(!enabled);
        if (pm != null) pm.ControlEnabled = enabled;
    }

    [Command]
    protected void CmdDeath()
    {
        if (!isClient)
        {
            currentState = ANIMATIONSTATES.Dead;
            IsAlive = false;
            pam.PlayDeathAudio();
            //Put death animation and stuff here
        }
        RpcDeath();
    }

    [ClientRpc]
    protected void RpcDeath()
    {
        currentState = ANIMATIONSTATES.Dead;
        IsAlive = false;
        pam.PlayDeathAudio();
        //Put death animation and stuff here
    }

    protected void Ability5(RaycastHit hit)
    {
        CmdCallRevive(hit.transform.gameObject);
    }

    [Command]
    protected void CmdCallRevive(GameObject o)
    {
        var ca = o.GetComponent<ClassAbilities>();
        if (ca != null)
        {
            CmdSetMaxHealth(healthMax - flatMaxHealthReviveCost);
            ca.CmdSetIsReviving(true);
        }
    }

    [Command]
    protected virtual void CmdSetIsReviving(bool value)
    {
        IsReviving = value;
    }

    protected virtual void OnRevive(bool value)
    {
        IsReviving = value;
        if (value)
        {
            IsAlive = value;
        }
    }

    #endregion

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

    public Vector3 Position {
        get {
            return new Vector3(transform.position.x, transform.position.y+1, transform.position.z);
        }
    }

    public override bool IsInvulnerable()
    {
        return false;
    }
}
