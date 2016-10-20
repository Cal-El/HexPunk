using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AethersmithAbilities : ClassAbilities {

    [SerializeField]private GameObject maelstromPrefab;
    [SerializeField]private GameObject bubblePrefab;
    [SerializeField] private GameObject javalinPrefab;
    [SerializeField]private GameObject javalinVisual;
    [SerializeField]private ParticleSystem javalinParticle;
    [SerializeField]
    private GameObject javalinHitEffect;
    [SerializeField]
    private GameObject hammerSwingEffect;

    public Ability HammerSwing = new Ability( 1, 3, 0.5f, 0, 0, 2f, 200);
    public Ability SpectralSpear = new Ability( 2, 5, 0.25f, 0.1f, 10, 100f, 1000);
    public Ability BubbleShield = new Ability( 3, 5, 1f, 0.5f, 30, 10f);
    public Ability Maelstrom = new Ability( 4, 5f, 1f, 0.25f, 50, 6f);

    private int playerNum = 0;

	// Use this for initialization
	void Start () {
        base.Initialize();
        
        javalinVisual.SetActive(false);
        javalinParticle.enableEmission = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (!IsAlive) {
            EButtonPrompt.SetActive(true);
        } else {
            EButtonPrompt.SetActive(false);
        }
        if (!isLocalPlayer) {
            return;
        }

        //Energy
        //Is based on Hammer Swing attack
        


        if (currCooldown > 0) {
            currCooldown = Mathf.Max(currCooldown - Time.deltaTime, 0);
            IsCasting = true;
        } else {
            if (IsAlive) {
                IsCasting = false;
                CanAimWhileCasting = false;
            }
        }

        if (castingTimer > 0) {
            castingTimer = Mathf.Max(castingTimer - Time.deltaTime, 0);
        }

        //Hud ability indicators
        if (myHud != null)
        {
            myHud.ShowIconNotEnoughEnergy(SpectralSpear.abilityNum, SpectralSpear.energyCost, energy);
            myHud.ShowIconNotEnoughEnergy(BubbleShield.abilityNum, BubbleShield.energyCost, energy);
            myHud.ShowIconNotEnoughEnergy(Maelstrom.abilityNum, Maelstrom.energyCost, energy);
        }

        //Abilities
        if (IsAlive && pm.ControlEnabled) {
            if (Input.GetButtonDown("Ability 1")) { UseAbility(HammerSwing); }
            if (GetAxisDown1("Ability 2")) { CanAimWhileCasting = true; UseAbility(SpectralSpear); }
            else if (Input.GetButtonDown("Ability 3")) { CanAimWhileCasting = true; UseAbility(BubbleShield); }
            else if (GetAxisDown2("Ability 4")) UseAbility(Maelstrom);

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

            if (waitingForAbility != 0 && castingTimer <= 0) {
                switch (waitingForAbility) {
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

        if (energy < 10)
            energy += Time.deltaTime;
        energy = Mathf.Clamp(energy, 0, energyMax);

        base.BaseUpdate();
	}

    #region Networking Helpers
    
    #region Ability Commands

    [Command]
    private void CmdAbility1() {
        if (!isClient) {
            Ability1();
        }
        RpcAbility1();
    }

    [ClientRpc]
    private void RpcAbility1() {
        Ability1();
    }

    [Command]
    private void CmdAbility2() {
        if (!isClient) {
            Ability2();
        }
        RpcAbility2();
    }

    [ClientRpc]
    private void RpcAbility2() {
        Ability2();
    }

    [Command]
    private void CmdAbility3() {
        if (!isClient) {
            Ability3();
        }
        RpcAbility3();
    }

    [ClientRpc]
    private void RpcAbility3() {
        Ability3();
    }

    [Command]
    private void CmdAbility4() {
        if (!isClient) {
            Ability4();
        }
        RpcAbility4();
    }

    [ClientRpc]
    private void RpcAbility4() {
        Ability4();
    }

    #endregion

    #endregion

    #region Ability 1 (HammerSwing)

    private void Ability1() {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, HammerSwing.range, transform.forward, 0);
        foreach(RaycastHit hit in hits) {
            if (Vector3.Angle(hit.transform.position-transform.position, transform.forward) < 60) {
                Character c = hit.transform.GetComponent<Character>();
                if (c != null) {
                    energy += 5;
                    energy = Mathf.Clamp(energy, 0, energyMax);
                    c.Knockback(transform.forward * HammerSwing.knockbackStr, 1);
                    c.TakeDmg(HammerSwing.baseDmg, DamageType.Standard, playerStats);
                    
                }
                
            }
        }
        Destroy(Instantiate(hammerSwingEffect, Position, transform.rotation) as GameObject, 0.5f);
        //60 degree cone of size HammerSwing.range
        //Add energy for each character hit
        //Small Knockback
    }

    #endregion

    #region Ability 2 (SpectralSpear)

    private void Ability2() {
        CanAimWhileCasting = true;

        RaycastHit[] sphereHits = Physics.SphereCastAll(Position, 0.5f, transform.forward, SpectralSpear.range);
        if (sphereHits != null)
        {
            int penetration = 0;
            int maxPenetration = 3;
            int farthestHit = 0;


            for (int i = 0; i < sphereHits.Length; i++)
            {
                if(sphereHits[i].distance > sphereHits[farthestHit].distance)
                    farthestHit = i;
                if (!sphereHits[i].collider.isTrigger && sphereHits[i].transform.tag != "Trigger" && sphereHits[i].transform.tag != "Guard" && sphereHits[i].transform.tag != "Aetherwall")
                {
                    Character ch = sphereHits[i].transform.GetComponent<Character>();
                    if (ch != null)
                    {
                        if (!ch.IsInvulnerable() && ch != this)
                        {
                            ch.Knockback(transform.forward * SpectralSpear.knockbackStr, 1);
                            ch.TakeDmg(SpectralSpear.baseDmg, DamageType.Standard, playerStats);
                            Destroy(Instantiate(javalinHitEffect, new Vector3(ch.transform.position.x, 1, ch.transform.position.z), javalinHitEffect.transform.rotation) as GameObject, 0.5f);
                            penetration++;
                            if (penetration >= maxPenetration)
                            {
                                farthestHit = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                }
            }
            GameObject g = Instantiate(javalinPrefab, javalinParticle.transform.position, Quaternion.identity) as GameObject;
            g.transform.LookAt(new Vector3(sphereHits[farthestHit].point.x, 1, sphereHits[farthestHit].point.z));
            g.transform.localScale = new Vector3(1, 1, sphereHits[farthestHit].distance);
        }
        else
        {
            GameObject g = Instantiate(javalinPrefab, javalinParticle.transform.position, Quaternion.identity) as GameObject;
            g.transform.LookAt(javalinVisual.transform.position + transform.forward);
            g.transform.localScale = new Vector3(1, 1, SpectralSpear.range);
        }
        javalinVisual.SetActive(false);
        javalinParticle.enableEmission = false;
        energy -= SpectralSpear.energyCost;
        //Shoot a spear that has almost no travel time
        //Large Single-target knockback
    }

    #endregion

    #region Ability 3 (BubbleShield)

    private void Ability3() {
        //Create a bubble around the character that blocks movement in and out of it and ranged attacks
        Instantiate(bubblePrefab, transform.position, Quaternion.identity);
        energy -= BubbleShield.energyCost;
    }

    #endregion

    #region Ability 4 (Maelstrom)

    private void Ability4() {
        //Create a persistent AoE DoT spinning whirlwind of death
        GameObject whirlwind = Instantiate(maelstromPrefab, transform.position, Quaternion.identity) as GameObject;
        whirlwind.transform.localScale *= Maelstrom.range;
        whirlwind.transform.parent = transform;
        energy -= Maelstrom.energyCost;
        //Will follow player after summoned for a duration
    }

    #endregion

    protected override void UseAbility(Ability a) {
        if (currCooldown <= 0 && energy >= a.energyCost) {
            base.UseAbility(a);
            if (a == SpectralSpear) {
                javalinVisual.SetActive(true);
                javalinParticle.enableEmission = true;
            }
        }
    }

    public override float TakeDmg(float dmg, DamageType damageType = DamageType.Standard, PlayerStats attacker = null) {
        if (!isActuallyGod && dmg > 0.02f)
        {
            //Used to add playerstats before the IsAlive bool is set
            float reducedDmg = dmg - dmg * ((energy * 0.5f) / energyMax);
            energy = Mathf.Clamp(energy + reducedDmg * 2, 0, energyMax);
            float tempHealth = Mathf.Clamp(health - reducedDmg, 0, healthMax);

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
                        playerStats.CmdAddDamageTaken(reducedDmg);
                        playerStats.CmdAddDeaths(1);
                    }
                }
            }
            else
            {
                if (isLocalPlayer) playerStats.CmdAddDamageTaken(reducedDmg);
                if (attacker != null && isServer) attacker.CmdAddDamageDealt(reducedDmg);
                pam.PlayTakeDamageAudio();
            }
            if (isClient && isLocalPlayer) CmdSetHealth(tempHealth);

            BloodSplatterer.MakeBlood(transform.position);
        }
        return health;
    }

    public override void GainXP(float xp) {
        float preLevel = level;
        base.GainXP(xp);
        if (preLevel < 1 && level >= 1) {
            SpectralSpear.energyCost *= 0.5f;
        } else if (preLevel < 2 && level >= 2) {
            BubbleShield.energyCost = 20;
        } else if (preLevel < 3 && level >= 3) {
            Maelstrom.energyCost = 35;
        } else if (preLevel < 4 && level >= 4) {
            HammerSwing.baseDmg *= 1.5f;
        } else if (preLevel < 5 && level >= 5) {
            Maelstrom.range *= 1.5f;
        }
    }
}
