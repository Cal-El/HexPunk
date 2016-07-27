using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ClassAbilities : NetworkBehaviour {

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

    private bool isAlive = true;
    public bool IsReviving { get; set; }

    protected bool isAxisInUse1 = false;
    protected bool isAxisInUse2 = false;
    protected bool isAxisDown1 = false;
    protected bool isAxisDown2 = false;

    protected struct Ability {
        public int abilityNum;
        public float baseDmg;
        public float castingTime;
        public float cooldown;
        public float range;
    };

    protected float currCooldown = 0.0f;
    protected float castingTimer = 0.0f;
    protected int waitingForAbility = 0; //0 means none

    protected void Initialize() {
        health = healthMax;
        energy = energyMax;
        pm = GetComponent<PlayerMovement>();
    }

    protected void BaseUpdate()
    {
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

    public void TakeDmg(float dmg) {
        health -= dmg;
    }

    public void Heal(float addedHP) {
        health += addedHP;
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
