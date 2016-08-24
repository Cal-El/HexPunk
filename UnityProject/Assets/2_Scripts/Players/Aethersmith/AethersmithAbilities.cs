using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AethersmithAbilities : ClassAbilities {

    [SerializeField]private GameObject maelstromPrefab;
    [SerializeField]private GameObject bubblePrefab;
    [SerializeField] private GameObject javalinPrefab;
    [SerializeField]private GameObject javalinVisual;
    [SerializeField]private ParticleSystem javalinParticle;

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
        if (!isLocalPlayer) {
            return;
        }

        //Energy
        //Is based on Hammer Swing attack

        if (currCooldown > 0) {
            currCooldown = Mathf.Max(currCooldown - Time.deltaTime, 0);
            this.GetComponent<PlayerMovement>().IsCasting = true;
        } else {
            if (IsAlive) pm.IsCasting = false;

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

        //Death
        if (health <= 0) CmdDeath();

        if (IsReviving) CmdRevive();

        //Abilities
        if (IsAlive) {
            if (Input.GetButtonDown("Ability 1")) UseAbility(HammerSwing);
            if (GetAxisDown1("Ability 2")) UseAbility(SpectralSpear);
            else if (Input.GetButtonDown("Ability 3")) UseAbility(BubbleShield);
            else if (GetAxisDown2("Ability 4")) UseAbility(Maelstrom);

            //Revive
            else if (Input.GetKeyDown(KeyCode.E)) {
                UseAbility(Revive);
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
                        Ability5();
                        break;
                }
                waitingForAbility = 0;
            }
        }

        base.BaseUpdate();
	}

    #region Networking Helpers

    //[Command]
    //private void CmdTakeDmg(GameObject o, float damage)
    //{
    //    o.SendMessage("TakeDmg", damage, SendMessageOptions.DontRequireReceiver);
    //}

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
                    c.Knockback(transform.forward * HammerSwing.knockbackStr, 1);
                    c.TakeDmg(HammerSwing.baseDmg);
                    
                }
                
            }
        }
        //60 degree cone of size HammerSwing.range
        //Add energy for each character hit
        //Small Knockback
    }

    #endregion

    #region Ability 2 (SpectralSpear)

    private void Ability2() {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit)) {
            Character c = hit.transform.GetComponent<Character>();
            if (c != null) {
                c.Knockback(transform.forward * SpectralSpear.knockbackStr, 1);
                c.TakeDmg(SpectralSpear.baseDmg);
            }
            GameObject g = Instantiate(javalinPrefab, javalinParticle.transform.position, Quaternion.identity) as GameObject;
            g.transform.LookAt(new Vector3(hit.point.x, 1, hit.point.z));
            g.transform.localScale = new Vector3(1, 1, hit.distance);

        } else {
            GameObject g = Instantiate(javalinPrefab, javalinParticle.transform.position, Quaternion.identity) as GameObject;
            g.transform.LookAt(javalinVisual.transform.position + transform.forward);
            g.transform.localScale = new Vector3(1, 1, SpectralSpear.range);
        }
        javalinVisual.SetActive(false);
        javalinParticle.enableEmission = false;
        energy -= 10;
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

    #region Ability 5 (Revive)

    private void Ability5() {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Revive.range)) {
            if (hit.transform.tag == "Player" && hit.transform.GetComponent<ClassAbilities>().health <= 0) {
                CmdCallRevive(hit.transform.gameObject);
            }
        }

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

    public override void TakeDmg(float dmg, DamageType damageType = DamageType.Standard) {
        health = Mathf.Clamp(health - (dmg - dmg*((energy*0.5f)/energyMax)),0,healthMax);
        if (health > 0) pam.PlayTakeDamageAudio();
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
            Maelstrom.range *= 1.2f;
        }
    }
}
