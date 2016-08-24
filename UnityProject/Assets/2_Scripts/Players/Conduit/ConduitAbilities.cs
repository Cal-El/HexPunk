using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ConduitAbilities : ClassAbilities {

    [HideInInspector]
    public ConduitAudioManager am;

    public float chainLightningRange = 5.0f;
    public GameObject lightningBoltPathPrefab;
    public GameObject punchBang;
    public Transform punchBangPoint;
    public GameObject staticStompPrefab;
    public GameObject dischargeLightningPathPrefab;

    public Ability LightingPunch = new Ability( 1, 1, 0.25f, 0.25f, 0, 2);
    public Ability StaticStomp = new Ability( 2, 0, 0.5f, 0.25f, 2, 1);
    public Ability LightningDash = new Ability( 3, 0, 0.25f, 0.25f, 4, 1.0f);
    public Ability Discharge = new Ability( 4, 1, 1, 0.5f, 0, 1000);

    private int playerNum = 0;

    private List<Character> lightningLists;

    // Use this for initialization
    void Start() {
        base.Initialize();
        energy = energyMax;
        am = GetComponentInChildren<ConduitAudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //Energy
        if (energy < energyMax)
        {
            energy = Mathf.Clamp(energy + Time.deltaTime, 0, energyMax);
        }

        //Hud ability indicators
        if (myHud != null)
        {
            myHud.ShowIconNotEnoughEnergy(StaticStomp.abilityNum, StaticStomp.energyCost, energy);
            myHud.ShowIconNotEnoughEnergy(LightningDash.abilityNum, LightningDash.energyCost, energy);
            //Need to add a way to do the same for discharge
        }

        //Cooldown
        if (currCooldown > 0) {
            currCooldown = Mathf.Max(currCooldown - Time.deltaTime, 0);
            this.GetComponent<PlayerMovement>().IsCasting = true;
        } else
        {
            if (IsAlive) this.GetComponent<PlayerMovement>().IsCasting = false;
            
        }
        if (castingTimer > 0) {
            castingTimer = Mathf.Max(castingTimer - Time.deltaTime, 0);
        }

        //Death
        if (health <= 0 && IsAlive) CmdDeath();

        if (IsReviving && !IsAlive) CmdRevive();

        //Abilities
        if (IsAlive)
        {
            if (Input.GetButtonDown("Ability 1")) UseAbility(LightingPunch);
            if (GetAxisDown1("Ability 2")) UseAbility(StaticStomp);
            if (Input.GetButtonDown("Ability 3")) UseAbility(LightningDash);
            if (GetAxisDown2("Ability 4")) UseAbility(Discharge);

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

    #region Ability 1 (Lightning punch)

    private void Ability1()
    {
        Ray ray = new Ray(transform.position+Vector3.up, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, LightingPunch.range)) {
            Character ch = hit.transform.GetComponent<Character>();
            if (ch != null) {
                Instantiate(punchBang, punchBangPoint.position, punchBangPoint.rotation);
                
                if (ch.stacks.Stacks > 0) {
                    List<Character> alreadyHit = new List<Character>();
                    alreadyHit.Add(ch);
                    List<Character> hits = ChainLightning(alreadyHit, ch, 1);
                    List<GameObject> copies = new List<GameObject>();
                    GameObject lightningBolts = Instantiate(lightningBoltPathPrefab);
                    for (int i = 0; i < hits.Count; i++)
                    {
                        hits[i].stacks.AddStack();
                        if (i > 0)
                        {
                            hits[i].TakeDmg(LightingPunch.baseDmg / 2, DamageType.FireElectric);
                        }
                        if (hits[i] == null)
                        {
                            hits.Remove(hits[i]);
                            continue;
                        }
                        GameObject g = new GameObject("Point");
                        g.transform.position = hits[i].transform.position;
                        g.transform.rotation = hits[i].transform.rotation;
                        g.transform.parent = lightningBolts.transform;
                        copies.Add(g);
                    }
                    lightningBolts.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPathScript>().LightningPath.List = copies;
                    lightningBolts.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPathScript>().AllowOrthographicMode = false;
                    lightningBolts.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPathScript>().Camera = null;
                } else {
                    ch.stacks.AddStack();
                }
                ch.TakeDmg(LightingPunch.baseDmg);
            }
        }
    }

    #endregion

    #region Ability 2 (Static stomp)

    private void Ability2()
    {
        RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, StaticStomp.range * energy, transform.forward, 0.0f);
        List<GameObject> alreadyHit = new List<GameObject>();
        foreach(RaycastHit h in hits) {
            if (!alreadyHit.Contains(h.transform.gameObject))
            {
                if (h.transform.GetComponent<ConduitStacks>() != null && h.transform != transform)
                    h.transform.GetComponent<ConduitStacks>().AddStack();
                alreadyHit.Add(h.transform.gameObject);
            }
        }
        if (staticStompPrefab != null){
            GameObject statStompBoom = Instantiate(staticStompPrefab, this.transform.position, staticStompPrefab.transform.rotation) as GameObject;
            statStompBoom.transform.localScale *= StaticStomp.range * energy;
        }
        energy = 0;
    }

    #endregion

    #region Ability 3 (Lightning Dash)

    private void Ability3()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit[] hit = Physics.RaycastAll(ray, LightningDash.range * energy);
        Vector3 telePos = ray.origin + (ray.direction * LightningDash.range * energy);
        if (hit.Length > 0)
            for (int i = 0; i < hit.Length; i++)
            {
                Character ch = hit[i].transform.GetComponent<Character>();
                if (ch == null)
                {
                    telePos = ray.origin + (ray.direction * (hit[i].distance - 0.5f));
                    break;
                }
                else
                {

                    if (LightningDash.range * energy - hit[i].distance < 0.5f)
                    {
                        telePos = ray.origin + (ray.direction * (hit[i].distance - 0.5f));
                    }
                    else if (LightningDash.range * energy - hit[i].distance >= 0.5f && LightningDash.range * energy - hit[i].distance < 1.5f)
                    {
                        telePos = ray.origin + (ray.direction * (hit[i].distance + 1.5f));
                        ch.stacks.AddStack();
                        ch.TakeDmg(LightningDash.baseDmg, DamageType.FireElectric);
                    }
                    else
                    {
                        ch.stacks.AddStack();
                        ch.TakeDmg(LightningDash.baseDmg, DamageType.FireElectric);
                    }
                }
            }
        Vector3 newPos = telePos + Vector3.down;
        this.transform.position = newPos;
        GetComponent<NetworkSyncPosition>().syncPos = newPos;
        energy = 0;
    }

    #endregion

    #region Ability 4 (Discharge)

    private void Ability4()
    {
        if (lightningLists != null)
        foreach(Character c in lightningLists) {
                if (c != null)
                {
                    float stks = c.stacks.Stacks;
                    CmdDischargeStacks(c.gameObject);
                    c.TakeDmg(Discharge.baseDmg * stks, DamageType.FireElectric);
                }
            }
    }

    [Command]
    private void CmdDischargeStacks(GameObject o)
    {
        if (o != null)
        {
            if (!isClient) o.GetComponent<Character>().stacks.Discharge();
            else RpcDischargeStacks(o);
        }
    }

    [ClientRpc]
    private void RpcDischargeStacks(GameObject o)
    {
        if(o!=null)
            o.GetComponent<Character>().stacks.Discharge();
    }

    private void StartAbility4()
    {
        GameObject lightningBolts = Instantiate(dischargeLightningPathPrefab);
        Character[] things = GameObject.FindObjectsOfType<Character>();
        List<GameObject> lightningList = new List<GameObject>();
        lightningLists = new List<Character>();
        lightningList.Add(gameObject);
        for (int i = 0; i < things.Length; i++)
        {
            if (things[i].stacks.Stacks > 0)
            {
                GameObject g = new GameObject("Point");
                g.transform.position = things[i].transform.position;
                g.transform.rotation = things[i].transform.rotation;
                g.transform.parent = lightningBolts.transform;
                lightningList.Add(g);
                lightningLists.Add(things[i]);
                lightningList.Add(gameObject);
            }
        }
        lightningBolts.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPathScript>().LightningPath.List = lightningList;
        lightningBolts.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPathScript>().AllowOrthographicMode = false;
        lightningBolts.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPathScript>().Camera = null;
    }

    [Command]
    private void CmdStartAbility4()
    {
        if (!isClient)
        {
            StartAbility4();
        }
        RpcStartAbility4();
    }

    [ClientRpc]
    private void RpcStartAbility4()
    {
        StartAbility4();
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

    protected override void UseAbility(Ability a) {
        if (currCooldown <= 0 && energy >= a.energyCost) {
            base.UseAbility(a);
            if (waitingForAbility == 4) {
                CmdStartAbility4();
            }
        }
    }

    private List<Character> ChainLightning(List<Character> alreadyHit, Character justHit, int pass) {
        Character candidate = Megamanager.FindClosestAttackable(justHit, pass);
        if (candidate == null)
            return alreadyHit;

        bool invalidCandidate = false;
        foreach (Character g in alreadyHit) {
            if (candidate == g)
                invalidCandidate = true;
        }
        if (candidate.gameObject == this.gameObject || candidate.stacks == null)
            invalidCandidate = true;

        if (Vector3.Distance(candidate.transform.position, justHit.transform.position) > chainLightningRange)
            return alreadyHit;
        if (!invalidCandidate) {
            alreadyHit.Add(candidate);
            if (candidate.stacks.Stacks > 0) {
                return ChainLightning(alreadyHit, candidate, 1);
            }
        } else {
            if (pass > 5)
                return alreadyHit;
            else
                return ChainLightning(alreadyHit, justHit, pass + 1);
        }

        return alreadyHit;
    }

    public override void GainXP(float xp) {
        float preLevel = level;
        base.GainXP(xp);
        if (preLevel < 1 && level >= 1) {
            LightningDash.range *= 1.2f;
        } else if (preLevel < 2 && level >= 2) {
            LightningDash.baseDmg = 1f;
        } else if (preLevel < 3 && level >= 3) {
            LightingPunch.baseDmg = 3f;
        } else if (preLevel < 4 && level >= 4) {
            Discharge.baseDmg *= 2f;
        } else if (preLevel < 5 && level >= 5) {
            StaticStomp.range *= 1.5f;
        }
    }
}
