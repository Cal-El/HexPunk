using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ShardAbilities : ClassAbilities {

    public float energyDecay = 6;
    public enum Form { ICE, WATER }

    [SyncVar]
    public Form currentForm = Form.ICE;

    [SyncVar]
    public float energySecondary;

    public GameObject iciclePrefab;
    public GameObject watersprayPrefab;
    public GameObject iceFieldPrefab;
    public GameObject waterspoutPrefab;
    public GameObject iceRamPrefab;
    public MeshRenderer mistCloud;
    private SkinnedMeshRenderer playerRenderer;
    public Transform projectileCastPoint;

    private Ability IceLance;
    private Ability Waterspray;
    private Ability Icefield;
    private Ability Waterspout;
    private Ability FormShift;
    private Ability IceRam;
    private Ability MistCloud;

    private float iceLanceCooldown = 1f;
    private GameObject currentIcicle;
    private Icicle icicleScript;
    private bool chargingAbility1 = false;
    private float growthWindow = 1;
    private float growthTimer;

    private GameObject currentWaterspray;
    private Waterspray watersprayScript;

    private int playerNum = 0;

    // Use this for initialization
    void Start()
    {
        base.Initialize();
        energySecondary = energyMax;
        playerRenderer = graphicObj.gameObject.GetComponent<SkinnedMeshRenderer>();

        IceLance.abilityNum = 1;
        IceLance.baseDmg = 1;
        IceLance.castingTime = Time.deltaTime;
        IceLance.cooldown = 0.01f;
        IceLance.energyCost = -0.1f;

        Waterspray.abilityNum = 1;
        Waterspray.baseDmg = 1.3f;
        Waterspray.castingTime = Time.deltaTime;
        Waterspray.cooldown = 0.01f;
        Waterspray.energyCost = -0.1f;

        Icefield.abilityNum = 2;
        Icefield.baseDmg = 4;
        Icefield.castingTime = 1f;
        Icefield.cooldown = 0.2f;
        Icefield.energyCost = -0.1f;

        Waterspout.abilityNum = 2;
        Waterspout.baseDmg = 1;
        Waterspout.castingTime = 1f;
        Waterspout.cooldown = 0.2f;
        Waterspout.energyCost = -0.1f;

        FormShift.abilityNum = 3;
        FormShift.baseDmg = 0;
        FormShift.castingTime = 0.1f;
        FormShift.cooldown = 0;
        FormShift.energyCost = -0.1f;

        IceRam.abilityNum = 4;
        IceRam.baseDmg = 0;
        IceRam.castingTime = 0.25f;
        IceRam.cooldown = 0.25f;
        IceRam.range = 1.0f;
        IceRam.energyCost = -0.1f;

        MistCloud.abilityNum = 4;
        MistCloud.castingTime = -0.1f;
        MistCloud.cooldown = -0.1f;
        MistCloud.energyCost = -0.1f;
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

        if (energy < FormShift.energyCost)
        {
            energy = energy - Time.deltaTime * energyDecay;
            energySecondary = energySecondary - Time.deltaTime * energyDecay;
        }

        //Energy
        if (energy < energyMax)
        {
            energy = Mathf.Clamp(energy, 0, energyMax);
            energySecondary = Mathf.Clamp(energySecondary, 0, energyMax);
        }

        //Death
        if (health <= 0) CmdDeath();

        if (IsReviving) CmdRevive();

        //Abilities
        if (IsAlive)
        {
            switch (currentForm)
            {
                case Form.ICE:
                    if (Input.GetButtonDown("Ability 1")) StartIceLaunch();
                    if (Input.GetButtonUp("Ability 1"))
                    {
                        LaunchIcicle();
                    }
                    else if (Input.GetButton("Ability 1"))
                    {
                        IceLance.castingTime = Time.deltaTime;
                        UseAbility(IceLance);
                    }
                    if (GetAxisDown1("Ability 2")) UseAbility(Icefield);
                    if (GetAxisDown2("Ability 4")) UseAbility(IceRam);
                    break;

                case Form.WATER:
                    if (Input.GetButtonDown("Ability 1")) StartWaterSpray();
                    if (Input.GetButtonUp("Ability 1")) StopWaterSpray();
                    else if (Input.GetButton("Ability 1"))
                    {
                        Waterspray.castingTime = Time.deltaTime;
                        UseAbility(Waterspray);
                    }
                    if (GetAxisDown1("Ability 2")) UseAbility(Waterspout);
                    if (Input.GetButtonUp("Ability 4")) StopMistCloud();
                    else if (Input.GetButton("Ability 4")) UseAbility(MistCloud);
                    break;
            }

            if (Input.GetButtonDown("Ability 3")) UseAbility(FormShift);

            //Revive
            if (Input.GetKeyDown(KeyCode.E))
            {
                UseAbility(Revive);
            }

            if (waitingForAbility != 0 && castingTimer <= 0)
            {
                switch (currentForm)
                {
                    case Form.ICE:
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
                        break;

                    case Form.WATER:
                        switch (waitingForAbility)
                        {
                            case 1:
                                CmdAbility1b();
                                break;
                            case 2:
                                CmdAbility2b();
                                break;
                            case 3:
                                CmdAbility3();
                                break;
                            case 4:
                                CmdAbility4b();
                                break;
                            case 5:
                                Ability5();
                                break;
                        }
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
    private void CmdAbility1b()
    {
        if (!isClient)
        {
            Ability1b();
        }
        RpcAbility1b();
    }

    [ClientRpc]
    private void RpcAbility1b()
    {
        Ability1b();
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
    private void CmdAbility2b()
    {
        if (!isClient)
        {
            Ability2b();
        }
        RpcAbility2b();
    }

    [ClientRpc]
    private void RpcAbility2b()
    {
        Ability2b();
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

    [Command]
    private void CmdAbility4b()
    {
        if (!isClient)
        {
            Ability4b();
        }
        RpcAbility4b();
    }

    [ClientRpc]
    private void RpcAbility4b()
    {
        Ability4b();
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

        energy += 3;
    }

    private void Ability1()
    {
        if(currentIcicle != null && growthTimer <= growthWindow)
        {
            growthTimer += Time.deltaTime;
            currentIcicle.transform.localScale += Vector3.one * 0.01f;
            icicleScript.damage += 0.05f;
            energy += Time.deltaTime;
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

    #region Ability 1 b (Waterspray)

    private void StartWaterSpray()
    {
        this.GetComponent<PlayerMovement>().IsCasting = true;
        currentWaterspray = Instantiate(watersprayPrefab, projectileCastPoint.position, transform.rotation) as GameObject;
        watersprayScript = currentWaterspray.GetComponent<Waterspray>();
        if (watersprayScript != null)
        {
            watersprayScript.damage = Waterspray.baseDmg;
        }
        growthTimer = 0;
    }

    private void Ability1b()
    {
        if (currentWaterspray != null && growthTimer <= growthWindow / 5)
        {
            growthTimer += Time.deltaTime;
            currentWaterspray.transform.localScale += Vector3.forward * 0.3f;
            currentWaterspray.transform.position = projectileCastPoint.position;
            currentWaterspray.transform.rotation = transform.rotation;
        }

        energySecondary += Time.deltaTime;
    }

    private void StopWaterSpray()
    {
        if (currentWaterspray != null)
        {
            Destroy(currentWaterspray);
        }
    }

    #endregion

    #region Ability 2 (Icefield)

    private void Ability2()
    {
        GameObject iceField = Instantiate(iceFieldPrefab, transform.position, transform.rotation) as GameObject;

        var targets = Physics.OverlapSphere(transform.position, iceField.transform.localScale.x / 2);

        foreach (var col in targets)
        {
            if (col != null && col.gameObject != gameObject)
            {
                Character ch = col.GetComponent<Character>();
                if (ch != null)
                {
                    Vector3 dir = (col.transform.position - transform.position).normalized;
                    ch.TakeDmg(Icefield.baseDmg);
                }
            }
        }

        energy += 10;
    }

    #endregion

    #region Ability 2 b (Waterspout)

    private void Ability2b()
    {
        GameObject waterspout = Instantiate(waterspoutPrefab, transform.position, transform.rotation) as GameObject;

        var targets = Physics.OverlapSphere(transform.position, waterspout.transform.localScale.x / 2);

        foreach (var col in targets)
        {
            if (col != null && col.gameObject != gameObject)
            {
                Character ch = col.GetComponent<Character>();
                if (ch != null)
                {
                    Vector3 dir = (col.transform.position - transform.position).normalized;
                        ch.TakeDmg(Waterspout.baseDmg);
                        ch.Knockback((new Vector3(dir.x, 0, dir.z) * 750), 1);
                }
            }
        }

        energySecondary += 25;
    }

    #endregion

    #region Ability 3 (FormShift)

    private void Ability3()
    {
        switch (currentForm)
        {
            //Change to water form
            case Form.ICE:
                CmdChangeGraphicColour(Color.blue);
                currentForm = Form.WATER;
                break;

            //Change to ice form
            case Form.WATER:
                CmdChangeGraphicColour(Color.white);
                currentForm = Form.ICE;
                break;
        }
    }

    #endregion

    #region Ability 4 (Iceram)

    private void Ability4()
    {

    }

    #endregion

    #region Ability 4 b (Mistcloud)

    private void Ability4b()
    {
        if (!mistCloud.enabled)
        {
            mistCloud.enabled = true;
            playerRenderer.enabled = false;
        }
    }

    private void StopMistCloud()
    {
        playerRenderer.enabled = true;
        mistCloud.enabled = false;
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
