using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ShardAbilities : ClassAbilities {

    public float energyDecay = 6;

    public GameObject iciclePrefab;
    public Transform projectileCastPoint;

    private Ability IceLance;
    private Ability IceLanceLaunch;
    private Ability Lavaball;
    private Ability Eruption;
    private Ability Afterburner;

    private GameObject currentIcicle;
    private Icicle icicleScript;
    private bool chargingAbility1 = false;
    private float growthWindow = 1;
    private float growthTimer;

    private int playerNum = 0;

    // Use this for initialization
    void Start()
    {
        base.Initialize();

        IceLance.abilityNum = 1;
        IceLance.baseDmg = 1;
        IceLance.castingTime = Time.deltaTime;
        IceLance.cooldown = 0.01f;
        IceLance.energyCost = -0.1f;

        Lavaball.abilityNum = 2;
        Lavaball.baseDmg = 4;
        Lavaball.castingTime = 1.5f;
        Lavaball.cooldown = 0.2f;
        Lavaball.energyCost = -0.1f;

        Eruption.abilityNum = 3;
        Eruption.baseDmg = 0;
        Eruption.castingTime = 1;
        Eruption.cooldown = 0.5f;
        Eruption.range = 1000;
        Eruption.energyCost = 90;

        Afterburner.abilityNum = 4;
        Afterburner.baseDmg = 0;
        Afterburner.castingTime = 0.25f;
        Afterburner.cooldown = 0.25f;
        Afterburner.range = 1.0f;
        Afterburner.energyCost = -0.1f;
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
            if (Input.GetButtonDown("Ability 1")) StartIceLaunch();
            if (Input.GetButtonUp("Ability 1")) LaunchIcicle();
            else if (Input.GetButton("Ability 1"))
            {
                IceLance.castingTime = Time.deltaTime;
                UseAbility(IceLance);
            }
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

    #region Ability 1 (IceLance)

    private void StartIceLaunch()
    {
        this.GetComponent<PlayerMovement>().IsCasting = true;
        currentIcicle = Instantiate(iciclePrefab, projectileCastPoint.position, transform.rotation) as GameObject;
        icicleScript = currentIcicle.GetComponent<Icicle>();
        if (icicleScript != null)
        {
            icicleScript.damage = IceLance.baseDmg;
        }
        growthTimer = 0;
    }

    private void Ability1()
    {
        if(currentIcicle != null && growthTimer <= growthWindow)
        {
            growthTimer += Time.deltaTime;
            currentIcicle.transform.localScale += Vector3.one * 0.01f;
            icicleScript.damage += 0.05f;
        }
    }

    private void LaunchIcicle()
    {
        if (icicleScript != null)
        {
            icicleScript.enabled = true;
        }
    }

    #endregion

    #region Ability 2 (Lavaball)

    private void Ability2()
    {
    }

    #endregion

    #region Ability 3 (Eruption)

    private void Ability3()
    {
    }

    #endregion

    #region Ability 4 (Afterburner)

    private void Ability4()
    {
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
        base.UseAbility(a);
    }
}
