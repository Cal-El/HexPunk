using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CalderaAbilities : ClassAbilities {
    
    [HideInInspector]
    public CalderaAudioManager am;

    public float energyDecay = 6;
    
    public GameObject fireballPrefab;
    public GameObject lavaballPrefab;
    public GameObject eruptionPrefab;
    public GameObject fireRunning;
    public Transform projectileCastPoint;
    
    public EnergyAddedAbility Fireball = new EnergyAddedAbility(1, 1, 0.25f, 0.01f, 0, 100, 0, 4);
    public EnergyAddedAbility Lavaball = new EnergyAddedAbility(2, 4, 1.5f, 0.2f, 0, 100, 0, 40);
    public EnergyAddedAbility Afterburner = new EnergyAddedAbility(3, 0, 0, 0, -0.1f, 0, 0, 25);
    public float afterburnerDuration = 2f;
    private float afterburnerTimer;
    public Ability Eruption = new Ability(4, 8, 1, 0.5f, 90, 5, 1000);
    private bool pastPressureThreshold = false;

    private int playerNum = 0;


    // Use this for initialization
    void Start()
    {
        base.Initialize();
        am = GetComponentInChildren<CalderaAudioManager>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsAlive) {
            EButtonPrompt.SetActive(true);
        } else {
            EButtonPrompt.SetActive(false);
        }
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
        else
        {
            pastPressureThreshold = true;
            energy = energy + Time.deltaTime * energyDecay;
        }

        //Energy
        if (energy < energyMax)
        {
            energy = Mathf.Clamp(energy, 0, energyMax);
        }


        //Hud ability indicators
        if (myHud != null)
        {
            if (pastPressureThreshold)
            {
                myHud.ShowCanCastAbility(Fireball.abilityNum, true);
                myHud.ShowCanCastAbility(Lavaball.abilityNum, true);
                myHud.ShowCanCastAbility(Afterburner.abilityNum, true);
            }
            else
            {
                myHud.ShowCanCastAbility(Fireball.abilityNum, false);
                myHud.ShowCanCastAbility(Lavaball.abilityNum, false);
                myHud.ShowCanCastAbility(Afterburner.abilityNum, false);
            }
            myHud.ShowIconNotEnoughEnergy(Eruption.abilityNum, Eruption.energyCost, energy);
        }        

        //Abilities
        if (IsAlive)
        {
            if (!pastPressureThreshold)
            {
                if (Input.GetButtonDown("Ability 1")) UseAbility(Fireball);
                if (GetAxisDown1("Ability 2")) UseAbility(Lavaball);
                if (Input.GetButtonDown("Ability 3")) UseAbility(Afterburner);
            }

            if (GetAxisDown2("Ability 4")) UseAbility(Eruption);

            //Revive
            if (Input.GetButton("Revive"))
            {
                bool hasRevived = false;
                foreach (var hit in Physics.SphereCastAll(new Vector3(transform.position.x, 0, transform.position.z), 2, transform.forward, Revive.range))
                {
                    if (hasRevived) return;
                    Debug.Log(hit.transform.gameObject);
                    if (hit.transform.tag == "Player" && !hit.transform.GetComponent<ClassAbilities>().IsAlive)
                    {
                        hasRevived = true;
                        reviveTarget = hit;
                        UseAbility(Revive);
                    }
                }
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
                        if(reviveTarget.transform != null) Ability5(reviveTarget);
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
            fireBallScript.range = Fireball.range;
        }
        energy += Fireball.energyAdded;
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
            lavaballScript.range = Lavaball.range;
        }
        energy += Lavaball.energyAdded;
    }

    #endregion

    #region Ability 3 (Afterburner)

    private void Ability3()
    {
        afterburnerTimer = Time.time + afterburnerDuration;
        pm.speed = pm.baseSpeed * 1.5f;
        GameObject g = Instantiate(fireRunning, transform.position, transform.rotation) as GameObject;
        g.transform.parent = this.transform;
        energy += Afterburner.energyAdded;
    }

    #endregion

    #region Ability 4 (Eruption)

    private void Ability4()
    {
        pastPressureThreshold = false;
        GameObject eruption = Instantiate(eruptionPrefab, transform.position, transform.rotation) as GameObject;

        Eruption script = eruption.GetComponent<Eruption>();

        if(script != null)
        {
            script.range = Eruption.range;
        }

        var targets = Physics.OverlapSphere(transform.position, Eruption.range);

        foreach (var col in targets)
        {
            if (col != null && col.gameObject != gameObject)
            {
                Character ch = col.GetComponent<Character>();
                if (ch != null && !ch.IsInvulnerable())
                {
                    Vector3 dir = (col.transform.position - transform.position).normalized;
                    ch.TakeDmg(Eruption.baseDmg * (Vector3.Distance(col.transform.position, transform.position)) / Eruption.range, DamageType.FireElectric, playerStats);
                    ch.Knockback((new Vector3(dir.x, 0, dir.z) * Eruption.knockbackStr), 1);
                    if (ch.gameObject != gameObject)
                    {
                        if (ch.burn != null)
                            ch.burn.IsBurning = true;
                    }
                }
            }
        }

        energy = 0;
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
