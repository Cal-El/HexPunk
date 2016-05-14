using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ClassAbilities : NetworkBehaviour {

    public Transform graphicObj;
    protected PlayerMovement pm;
    public float healthMax = 100;
    private float health;
    public float energyMax = 100;
    private float energy;

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

    public float Health {
        get {
            return health;
        } set {
            health = Mathf.Max(Mathf.Min(value, healthMax), 0f);
        }
    }

    public float Energy {
        get {
            return energy;
        }
        set {
            energy = Mathf.Max(Mathf.Min(value, energyMax), 0f);
        }
    }

    public void TakeDmg(float dmg) {
        Health -= dmg;
    }

    public void Heal(float addedHP) {
        Health += addedHP;
    }

    protected virtual void UseAbility(Ability a) {
        if (currCooldown <= 0) {
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
