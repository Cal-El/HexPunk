using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AethersmithAbilities : ClassAbilities {

    [SerializeField]private GameObject maelstromPrefab;
    [SerializeField]private GameObject bubblePrefab;

    private Ability HammerSwing;
    private Ability SpectralSpear;
    private Ability BubbleShield;
    private Ability Maelstrom;

    private int playerNum = 0;

	// Use this for initialization
	void Start () {
        base.Initialize();

        HammerSwing.abilityNum = 1;
        HammerSwing.baseDmg = 3;
        HammerSwing.castingTime = 0.5f;
        HammerSwing.cooldown = 0;
        HammerSwing.range = 2f;
        HammerSwing.energyCost = 0;

        SpectralSpear.abilityNum = 2;
        SpectralSpear.baseDmg = 5;
        SpectralSpear.castingTime = 0.25f;
        SpectralSpear.cooldown = 0.1f;
        SpectralSpear.range = 100f;
        SpectralSpear.energyCost = 10;

        BubbleShield.abilityNum = 3;
        BubbleShield.baseDmg = 5;
        BubbleShield.castingTime = 1f;
        BubbleShield.cooldown = 0.5f;
        BubbleShield.range = 10f;
        BubbleShield.energyCost = 30;

        Maelstrom.abilityNum = 4;
        Maelstrom.baseDmg = 5f;
        Maelstrom.castingTime = 1f;
        Maelstrom.cooldown = 0.25f;
        Maelstrom.range = 10f;
        Maelstrom.energyCost = 50;
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

    #region Ability 1 (Lightning punch)

    private void Ability1() {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, HammerSwing.range, transform.forward, 0);
        foreach(RaycastHit hit in hits) {
            if (Vector3.Angle(hit.transform.position-transform.position, transform.forward) < 60) {
                Character c = hit.transform.GetComponent<Character>();
                if (c != null) {
                    energy += 5;
                    c.Knockback(transform.forward*200, 1);
                    c.TakeDmg(HammerSwing.baseDmg);
                    
                }
                
            }
        }
        //60 degree cone of size HammerSwing.range
        //Add energy for each character hit
        //Small Knockback
    }

    #endregion

    #region Ability 2 (Static stomp)

    private void Ability2() {
        RaycastHit hit;
        if(Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit)) {
            Character c = hit.transform.GetComponent<Character>();
            if(c != null) {
                c.Knockback(transform.forward * 1000, 1);
                c.TakeDmg(SpectralSpear.baseDmg);
            }
        }
        energy -= 10;
        //Shoot a spear that has almost no travel time
        //Large Single-target knockback
    }

    #endregion

    #region Ability 3 (Discharge)

    private void Ability3() {
        //Create a bubble around the character that blocks movement in and out of it and ranged attacks
        Instantiate(bubblePrefab, transform.position, Quaternion.identity);
        energy -= BubbleShield.energyCost;
    }

    #endregion

    #region Ability 4 (Lightning Dash)

    private void Ability4() {
        //Create a persistent AoE DoT spinning whirlwind of death
        GameObject whirlwind = Instantiate(maelstromPrefab, transform.position, Quaternion.identity) as GameObject;
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
        }
    }

    public override void TakeDmg(float dmg) {
        health -= dmg - dmg*((energy*0.5f)/energyMax);
    }
}
