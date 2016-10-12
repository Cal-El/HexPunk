﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ShardAbilities : ClassAbilities {

    [HideInInspector]
    public ShardAudioManager am;

    public float energyDecay = 1;

    public GameObject iciclePrefab;
    public GameObject iceFieldPrefab;
    public ParticleSystem mistCloudEffect;
    [SyncVar]
    public bool isMist;
    public GameObject iceRamPrefab;
    private SkinnedMeshRenderer playerRenderer;
    public Transform projectileCastPoint;

    [System.Serializable]
    public class EnergyAddedChannelledAbility : EnergyAddedAbility
    {
        [Tooltip("Determines the rate at which energy is added / subtracted while channelling.")]
        public float energyChargeModifier;

        public EnergyAddedChannelledAbility(int abilityNum = 0, float baseDmg = 0, float castingTime = 0, float cooldown = 0, float energyCost = 0, float range = 0, float knockbackStr = 0, float initialEnergyAdded = 0, float energyChargeModifier = 0)
            : base(abilityNum, baseDmg, castingTime, cooldown, energyCost, range, knockbackStr, initialEnergyAdded)
        {
            this.energyChargeModifier = energyChargeModifier;
        }
    }
    
    public EnergyAddedChannelledAbility IceLance = new EnergyAddedChannelledAbility( 1, 1, 0, 0.01f, 0, 100, 0, 3, 1);
    private bool startedCasting = false;
    private float iceLanceCast;
    private float iceLanceCooldown;
    public EnergyAddedAbility Icefield = new EnergyAddedAbility( 2, 4, 1f, 0.2f, 0, 3, 0, 25 );
    [Tooltip("Energy is taken away while channelled.")]
    public EnergyAddedChannelledAbility MistCloud = new EnergyAddedChannelledAbility( 3, 0, 0, 0, 0, 0, 0, 0, 1 );
    public Ability IceRam = new Ability(4, 15, 0.25f, 0.25f, 100, 100, 1000);

    private GameObject currentIcicle;
    private Icicle icicleScript;
    private bool chargingAbility1 = false;
    public float growthWindow = 1;
    private float growthTimer;
    public int iceLanceDamageMod = 1;
    private int playerNum = 0;

    // Use this for initialization
    void Start()
    {
        base.Initialize();
        am = GetComponentInChildren<ShardAudioManager>();
        iceLanceCast = IceLance.castingTime;
        iceLanceCooldown = IceLance.cooldown;
        IceLance.cooldown = Time.deltaTime;
        playerRenderer = graphicObj.gameObject.GetComponent<SkinnedMeshRenderer>();
        mistCloudEffect.enableEmission = false;
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
            IsCasting = true;
        }
        else
        {
            if (IsAlive) {
                IsCasting = false;
                CanAimWhileCasting = false;
            }
        }

        if (castingTimer > 0)
        {
            castingTimer = Mathf.Max(castingTimer - Time.deltaTime, 0);
        }

        if (energy < IceRam.energyCost && !startedCasting)
        {
            energy = energy - Time.deltaTime * energyDecay;
        }

        //Energy
        if (energy < energyMax)
        {
            energy = Mathf.Clamp(energy, 0, energyMax);
        }

        //Hud ability indicators
        if (myHud != null)
        {
            myHud.ShowIconNotEnoughEnergy(MistCloud.abilityNum, 0.005f, energy);
            myHud.ShowIconNotEnoughEnergy(IceRam.abilityNum, IceRam.energyCost, energy);
        }

        //Abilities
        if (IsAlive && pm.ControlEnabled)
        {
            if (Input.GetButtonDown("Ability 1") && !isMist)
            {
                ShardUseAbility(IceLance, true);
            }
            if (!Input.GetButton("Ability 1") && startedCasting && !isMist)
            {
                ShardUseAbility(IceLance, false, true);
            }
            else if (Input.GetButton("Ability 1") && startedCasting && !isMist)
            {
                ShardUseAbility(IceLance);
            }
            if (GetAxisDown1("Ability 2") && !isMist) { CanAimWhileCasting = true; ShardUseAbility(Icefield); }
            if (!Input.GetButton("Ability 3") || energy <= 0) CmdStopMistCloud();
            else if (Input.GetButton("Ability 3") && energy > 0) ShardUseAbility(MistCloud);
            if (GetAxisDown2("Ability 4") && !isMist) { CanAimWhileCasting = true; ShardUseAbility(IceRam); }

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
                        if (reviveTarget.transform != null) Ability5(reviveTarget);
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

    #region Ability 1 (IceLance)
    
    private void Ability1()
    {
        if (currentIcicle != null && Time.time <= growthTimer)
        {
            currentIcicle.transform.localScale += Vector3.one * 0.01f;
            icicleScript.damage += IceLance.baseDmg * Time.deltaTime * iceLanceDamageMod;
            energy += Time.deltaTime * IceLance.energyChargeModifier;
        }

    }

    [Command]
    private void CmdStartIceLaunch()
    {
        if (!isClient) StartIceLaunch();
        RpcStartIceLaunch();
    }

    [ClientRpc]
    private void RpcStartIceLaunch()
    {
        StartIceLaunch();
    }

    private void StartIceLaunch()
    {
        CanAimWhileCasting = true;
        IsCasting = true;
        currentIcicle = Instantiate(iciclePrefab, projectileCastPoint.position, transform.rotation) as GameObject;
        currentIcicle.transform.parent = this.transform;
        icicleScript = currentIcicle.GetComponent<Icicle>();
        if (icicleScript != null)
        {
            icicleScript.owner = gameObject;
            icicleScript.damage = IceLance.baseDmg;
            icicleScript.range = IceLance.range;
        }
        growthTimer = Time.time + growthWindow;

        energy += IceLance.energyAdded;
    }

    [Command]
    private void CmdLaunchIcicle()
    {
        if (!isClient) LaunchIcicle();
        RpcLaunchIcicle();
    }

    [ClientRpc]
    private void RpcLaunchIcicle()
    {
        LaunchIcicle();
    }

    private void LaunchIcicle()
    {
        if (currentIcicle != null) {
            currentIcicle.transform.parent = null;
            currentIcicle.GetComponent<CapsuleCollider>().enabled = true;
            if (icicleScript != null) {
                icicleScript.enabled = true;
            }
        }
    }

    #endregion

    #region Ability 2 (Icefield)

    private void Ability2()
    {
        GameObject iceField = Instantiate(iceFieldPrefab, transform.position, transform.rotation) as GameObject;

        IceField script = iceField.GetComponent<IceField>();

        if(script != null)
        {
            script.range = Icefield.range;
        }

        var targets = Physics.OverlapSphere(transform.position, Icefield.range);

        foreach (var col in targets)
        {
            if (col != null && col.gameObject != gameObject)
            {
                Character ch = col.GetComponent<Character>();
                if (ch != null)
                {
                    Vector3 dir = (col.transform.position - transform.position).normalized;
                    ch.TakeDmg(Icefield.baseDmg, DamageType.Standard, playerStats);
                }
            }
        }

        energy += Icefield.energyAdded;
    }

    #endregion

    #region Ability 3 (Mistcloud)

    private void Ability3()
    {
        if (!isMist)
        {
            isMist = true;
            mistCloudEffect.enableEmission = true;
            playerRenderer.enabled = false;
        }

        energy -= Time.deltaTime * MistCloud.energyChargeModifier;
    }

    private void StopMistCloud()
    {
        if (isMist)
        {
            isMist = false;
            playerRenderer.enabled = true;
            mistCloudEffect.enableEmission = false;
        }
    }

    [Command]
    private void CmdStopMistCloud()
    {
        if (!isClient) StopMistCloud();
        RpcStopMistCloud();
    }

    [ClientRpc]
    private void RpcStopMistCloud()
    {
        StopMistCloud();
    }

    #endregion

    #region Ability 4 (Iceram)

    private void Ability4()
    {
        if (iceRamPrefab != null)
        {
            GameObject iceRam = Instantiate(iceRamPrefab, projectileCastPoint.position, transform.rotation) as GameObject;
            IceRam iceRamScript = iceRam.GetComponent<IceRam>();
            iceRamScript.owner = gameObject;
            iceRamScript.damage = IceRam.baseDmg;
            iceRamScript.knockBack = IceRam.knockbackStr;
            iceRamScript.range = IceRam.range;
        }

        energy = 0;
    }

    #endregion

    private void ShardUseAbility(Ability a, bool start = false, bool launch = false)
    {
        if (launch && startedCasting)
        {
            currCooldown = 0;
            IceLance.cooldown = iceLanceCooldown;
            base.UseAbility(a);
            CmdLaunchIcicle();
            startedCasting = false;
            IceLance.cooldown = Time.deltaTime;
        }
        else if (currCooldown <= 0 && energy >= a.energyCost)
        {
            if (start)
            {
                startedCasting = true;
                IceLance.castingTime = iceLanceCast;
                CmdStartIceLaunch();
                base.UseAbility(a);
                IceLance.castingTime = Time.deltaTime;
            }
            else
            {
                base.UseAbility(a);
            }
        }
    }

    public override float TakeDmg(float dmg, DamageType damageType = DamageType.Standard, PlayerStats attacker = null)
    {
        if (!isMist && !isActuallyGod && dmg > 0.02f)
        {
            //Used to add playerstats before the IsAlive bool is set
            float tempHealth = Mathf.Clamp(health - dmg, 0, healthMax);

            if (tempHealth <= 0)
            {
                if (IsAlive)
                {
                    if (attacker != null && attacker.isLocalPlayer)
                    {
                        attacker.CmdAddPlayerKills(1);
                    }
                    if (isLocalPlayer)
                    {
                        playerStats.CmdAddDamageTaken(dmg);
                        playerStats.CmdAddDeaths(1);
                    }
                }
            }
            else            
            {
                if (isLocalPlayer) playerStats.CmdAddDamageTaken(dmg);
                if (attacker != null && attacker.isLocalPlayer) attacker.CmdAddDamageDealt(dmg);
                pam.PlayTakeDamageAudio();
            }
            if (isClient && isLocalPlayer) CmdSetHealth(tempHealth);

            BloodSplatterer.MakeBlood(transform.position);
        }
        return health;
    }

    public override bool IsInvulnerable() {
        return isMist || isActuallyGod;
    }
}
