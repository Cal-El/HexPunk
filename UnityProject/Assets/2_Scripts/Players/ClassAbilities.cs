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
    public PlayerStats playerStats;
    protected PlayerGUICanvas myGUI;
    protected HUDProperties myHud;
    public GameObject EButtonPrompt;
    [SyncVar (hook = "OnMaxHealthChanged")]
    public float healthMax = 100;
    [SyncVar (hook = "OnHealthChanged")]
    public float health;
    public float energyMax = 100;
    [SyncVar]
    public float energy;
    protected float level = 0;

    [HideInInspector] public bool isActuallyGod = false;
    [SyncVar (hook = "OnIsAliveChanged")]
    public bool IsAlive = true;
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
        EButtonPrompt.SetActive(false);
        base.Initialise();
    }

    protected void BaseUpdate()
    {
        if (isActuallyGod) {
            health = healthMax;
            energy = energyMax;
        }

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

        if (Input.GetKeyDown(KeyCode.I))
            level += 0.1f;

        if (Input.GetKeyDown(KeyCode.K))
            Knockback(-transform.forward *1000, 1);

        if (Input.GetButtonDown("HelpGUI") && !myGUI.helpGUI.activeSelf)
        {
            myGUI.ShowHelp(true);
        }

        if (Input.GetButtonUp("HelpGUI") && myGUI.helpGUI.activeSelf)
        {
            if (myGUI.helpGUI.activeSelf) myGUI.ShowHelp(false);
        }

        if (currCooldown <= 0)
        {
            if (!IsAlive)
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
        //Death
        if (isLocalPlayer && health <= 0 && IsAlive) CmdSetIsAlive(false);
    }
    public override float TakeDmg(float dmg, DamageType damageType = DamageType.Standard, PlayerStats attacker = null) {
        if (!isActuallyGod) {
            CmdSetHealth(Mathf.Clamp(health - dmg, 0, healthMax));
            if (attacker != null && isServer) attacker.CmdAddDamageDealt(dmg);
            playerStats.CmdAddDamageTaken(dmg);
            if (health > 0)
            {
                pam.PlayTakeDamageAudio();
            }
            else
            {
                if (IsAlive && isServer)
                {
                    if (attacker != null)
                    {
                        attacker.CmdAddPlayerKills(1);
                        attacker.CmdAddKills(1);
                    }
                    playerStats.CmdAddDeaths(1);
                }
            }
        }
        return health;
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

    public void BecomeGod() {
        if (!isActuallyGod) {
            isActuallyGod = true;
            health = healthMax;
            energy = energyMax;
        } else {
            isActuallyGod = false;
        }
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

    [Command]
    protected void CmdSetIsAlive(bool value)
    {
        var megamanager = FindObjectOfType<Megamanager>();
        if (megamanager != null) megamanager.AddDeadPlayer(value ? -1 : 1);
        EnableCharacter(value);
        IsAlive = value;
        if (!value)
        {
            currentState = ANIMATIONSTATES.Dead;
            pam.PlayDeathAudio();
        }
    }

    protected virtual void OnIsAliveChanged(bool value)
    {
        EnableCharacter(value);
        IsAlive = value;
        if (value)
        {
            CmdSetIsReviving(false);
        }
        else
        {
            currentState = ANIMATIONSTATES.Dead;
            pam.PlayDeathAudio();
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
    protected void CmdAddDeathToMegaManager()
    {
        var megamanager = FindObjectOfType<Megamanager>();
        if (megamanager != null) megamanager.AddDeadPlayer(1);
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
            CmdSetIsAlive(value);
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
        return isActuallyGod;
    }
}
