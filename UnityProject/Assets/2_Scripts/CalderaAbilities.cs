﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CalderaAbilities : ClassAbilities {

    public float energyDecay = 5;

    public float chainLightningRange = 5.0f;
    public GameObject fireballPrefab;
    public GameObject lavaballPrefab;
    public Transform projectileCastPoint;
    public GameObject staticStompPrefab;
    public GameObject dischargeLightningPathPrefab;

    private Ability Fireball;
    private Ability Lavaball;
    private Ability Eruption;
    private Ability Afterburner;

    private int playerNum = 0;

    private List<ConduitStacks> lightningLists;

    // Use this for initialization
    void Start()
    {
        base.Initialize();

        Fireball.abilityNum = 1;
        Fireball.baseDmg = 1;
        Fireball.castingTime = 0.25f;
        Fireball.cooldown = 0.01f;
        Fireball.range = 2;
        Fireball.energyCost = 0;

        Lavaball.abilityNum = 2;
        Lavaball.baseDmg = 0;
        Lavaball.castingTime = 0.5f;
        Lavaball.cooldown = 0.25f;
        Lavaball.range = 1;
        Lavaball.energyCost = 0;

        Eruption.abilityNum = 3;
        Eruption.baseDmg = 0;
        Eruption.castingTime = 1;
        Eruption.cooldown = 0.5f;
        Eruption.range = 1000;
        Eruption.energyCost = 98;

        Afterburner.abilityNum = 4;
        Afterburner.baseDmg = 0;
        Afterburner.castingTime = 0.25f;
        Afterburner.cooldown = 0.25f;
        Afterburner.range = 1.0f;
        Afterburner.energyCost = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //Energy
        if (energy < 5)
        {
            energy = Mathf.Clamp(energy + Time.deltaTime, 0, 5);
        }
        if (currCooldown > 0)
        {
            currCooldown = Mathf.Max(currCooldown - Time.deltaTime, 0);
            this.GetComponent<PlayerMovement>().IsCasting = true;
        }
        else
        {
            if (IsAlive) this.GetComponent<PlayerMovement>().IsCasting = false;

        }
        if (castingTimer > 0)
        {
            castingTimer = Mathf.Max(castingTimer - Time.deltaTime, 0);
        }

        if (energy < Eruption.energyCost)
        {
            energy = energy - Time.deltaTime * energyDecay;
        }

        //Death
        if (health <= 0) CmdDeath();

        if (IsReviving) CmdRevive();

        //Abilities
        if (IsAlive)
        {
            if (Input.GetButtonDown("Ability 1")) UseAbility(Fireball);
            if (GetAxisDown1("Ability 2")) UseAbility(Lavaball);
            else if (Input.GetButtonDown("Ability 3")) UseAbility(Eruption);
            else if (GetAxisDown2("Ability 4")) UseAbility(Afterburner);

            //Revive
            else if (Input.GetKeyDown(KeyCode.E))
            {
                UseAbility(Revive);
            }

            if (waitingForAbility != 0 && castingTimer <= 0)
            {
                switch (waitingForAbility)
                {
                    case 1:
                        CmdAbility1();
                        break;
                    case 2:
                        CmdAbility2();
                        break;
                    case 3:
                        CmdAbility3();
                        break;
                    case 4:
                        CmdAbility4();
                        break;
                    case 5:
                        Ability5();
                        break;
                }
                waitingForAbility = 0;
            }
        }

        base.BaseUpdate();
    }

    #region Networking Helpers

    #region Ability Commands

    [Command]
    private void CmdAbility1()
    {
        if (!isClient)
        {
            Ability1();
        }
        RpcAbility1();
    }

    [ClientRpc]
    private void RpcAbility1()
    {
        Ability1();
    }

    [Command]
    private void CmdAbility2()
    {
        if (!isClient)
        {
            Ability2();
        }
        RpcAbility2();
    }

    [ClientRpc]
    private void RpcAbility2()
    {
        Ability2();
    }

    [Command]
    private void CmdAbility3()
    {
        if (!isClient)
        {
            Ability3();
        }
        RpcAbility3();
    }

    [ClientRpc]
    private void RpcAbility3()
    {
        Ability3();
    }

    [Command]
    private void CmdAbility4()
    {
        if (!isClient)
        {
            Ability4();
        }
        RpcAbility4();
    }

    [ClientRpc]
    private void RpcAbility4()
    {
        Ability4();
    }

    #endregion

    #endregion

    #region Ability 1 (Fireball)

    private void Ability1()
    {
        if (fireballPrefab != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, projectileCastPoint.position, transform.rotation) as GameObject;
            Fireball fireBallScript = fireball.GetComponent<Fireball>();
            fireBallScript.owner = gameObject;
            fireBallScript.damage = Fireball.baseDmg;
        }
        energy += 3;
    }

    #endregion

    #region Ability 2 (Lavaball)

    private void Ability2()
    {
        if (lavaballPrefab != null)
        {
            GameObject lavaball = Instantiate(lavaballPrefab, projectileCastPoint.position, transform.rotation) as GameObject;
            Lavaball lavaballScript = lavaball.GetComponent<Lavaball>();
            lavaballScript.owner = gameObject;
            lavaballScript.damage = Lavaball.baseDmg;
        }
        energy += 20;
    }

    #endregion

    #region Ability 3 (Eruption)

    private void Ability3()
    {
        energy = 0;
    }

    #endregion

    #region Ability 4 (Afterburner)

    private void Ability4()
    {
        energy += 30;
    }

    #endregion

    #region Ability 5 (Revive)

    private void Ability5()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Revive.range))
        {
            if (hit.transform.tag == "Player" && hit.transform.GetComponent<ClassAbilities>().health <= 0)
            {
                CmdCallRevive(hit.transform.gameObject);
            }
        }
    }

    #endregion

    protected override void UseAbility(Ability a)
    {
        if (currCooldown <= 0 && energy >= a.energyCost)
        {
            //Can only use eruption if they are past the threshold
            if (energy >= Eruption.energyCost)
            {
                if(a.abilityNum == Eruption.abilityNum)
                {
                    base.UseAbility(a);
                }
            }
            else
            {
                base.UseAbility(a);
            }
        }
    }
}
