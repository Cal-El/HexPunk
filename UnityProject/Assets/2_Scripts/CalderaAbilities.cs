using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CalderaAbilities : ClassAbilities {

    public float energyDecay = 6;
    
    public GameObject fireballPrefab;
    public GameObject lavaballPrefab;
    public GameObject eruptionPrefab;
    public Transform projectileCastPoint;

    private Ability Fireball;
    private Ability Lavaball;
    private Ability Afterburner;
    private Ability Eruption;

    private int playerNum = 0;

    public float afterburnerDuration = 2f;
    private float afterburnerTimer;

    // Use this for initialization
    void Start()
    {
        base.Initialize();

        Fireball.abilityNum = 1;
        Fireball.baseDmg = 1;
        Fireball.castingTime = 0.25f;
        Fireball.cooldown = 0.01f;
        Fireball.energyCost = -0.1f;

        Lavaball.abilityNum = 2;
        Lavaball.baseDmg = 4;
        Lavaball.castingTime = 1.5f;
        Lavaball.cooldown = 0.2f;
        Lavaball.energyCost = -0.1f;

        Afterburner.abilityNum = 3;
        Afterburner.baseDmg = 0;
        Afterburner.castingTime = 0;
        Afterburner.cooldown = 0;
        Afterburner.range = 1.0f;
        Afterburner.energyCost = -0.1f;

        Eruption.abilityNum = 4;
        Eruption.baseDmg = 8;
        Eruption.castingTime = 1;
        Eruption.cooldown = 0.5f;
        Eruption.energyCost = 90;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
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

        //Energy
        if (energy < energyMax)
        {
            energy = Mathf.Clamp(energy, 0, energyMax);
        }

        //Death
        if (health <= 0) CmdDeath();

        if (IsReviving) CmdRevive();

        //Abilities
        if (IsAlive)
        {
            if (Input.GetButtonDown("Ability 1")) UseAbility(Fireball);
            if (GetAxisDown1("Ability 2")) UseAbility(Lavaball);
            if (Input.GetButtonDown("Ability 3")) UseAbility(Afterburner);
            if (GetAxisDown2("Ability 4")) UseAbility(Eruption);

            //Revive
            if (Input.GetKeyDown(KeyCode.E))
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

            if(Time.time > afterburnerTimer && pm.speed != pm.baseSpeed)
            {
                pm.speed = pm.baseSpeed;
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
        energy += 4;
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
        energy += 40;
    }

    #endregion

    #region Ability 3 (Afterburner)

    private void Ability3()
    {
        afterburnerTimer = Time.time + afterburnerDuration;
        pm.speed = pm.baseSpeed * 1.5f;
        energy += 25;
    }

    #endregion

    #region Ability 4 (Eruption)

    private void Ability4()
    {
        GameObject eruption = Instantiate(eruptionPrefab, transform.position, transform.rotation) as GameObject;

        var radius = eruption.transform.localScale.x / 2;

        var targets = Physics.OverlapSphere(transform.position, radius);

        foreach (var col in targets)
        {
            if (col != null && col.gameObject != gameObject)
            {
                Character ch = col.GetComponent<Character>();
                if (ch != null)
                {
                    Vector3 dir = (col.transform.position - transform.position).normalized;
                    ch.TakeDmg(Eruption.baseDmg * (Vector3.Distance(col.transform.position, transform.position)) / radius);
                    ch.Knockback((new Vector3(dir.x, 0, dir.z) * 1000), 1);
                }
            }
        }

        energy = 0;
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
                base.UseAbility(a);
        }
    }
}
