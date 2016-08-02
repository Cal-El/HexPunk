using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ConduitAbilities : ClassAbilities {

    public float chainLightningRange = 5.0f;
    public GameObject lightningBoltPathPrefab;
    public GameObject punchBang;
    public Transform punchBangPoint;
    public GameObject staticStompPrefab;
    public GameObject dischargeLightningPathPrefab;

    private Ability LightingPunch;
    private Ability StaticStomp;
    private Ability Discharge;
    private Ability LightningDash;

    private int playerNum = 0;

    private List<ConduitStacks> lightningLists;

    // Use this for initialization
    void Start() {
        base.Initialize();
        energy = energyMax;

        LightingPunch.abilityNum = 1;
        LightingPunch.baseDmg = 1;
        LightingPunch.castingTime = 0.25f;
        LightingPunch.cooldown = 0.25f;
        LightingPunch.range = 2;
        LightingPunch.energyCost = 0;

        StaticStomp.abilityNum = 2;
        StaticStomp.baseDmg = 0;
        StaticStomp.castingTime = 0.5f;
        StaticStomp.cooldown = 0.25f;
        StaticStomp.range = 1;
        StaticStomp.energyCost = 2;

        Discharge.abilityNum = 3;
        Discharge.baseDmg = 0;
        Discharge.castingTime = 1;
        Discharge.cooldown = 0.5f;
        Discharge.range = 1000;
        Discharge.energyCost = 0;

        LightningDash.abilityNum = 4;
        LightningDash.baseDmg = 0;
        LightningDash.castingTime = 0.25f;
        LightningDash.cooldown = 0.25f;
        LightningDash.range = 1.0f;
        LightningDash.energyCost = 4;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //Energy
        if (energy < 5) {
            energy = Mathf.Clamp(energy + Time.deltaTime, 0, 5);
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
        if (health <= 0) CmdDeath();

        if (IsReviving) CmdRevive();

        //Abilities
        if (IsAlive)
        {
            if (Input.GetButtonDown("Ability 1")) UseAbility(LightingPunch);
            if (GetAxisDown1("Ability 2")) UseAbility(StaticStomp);
            else if (Input.GetButtonDown("Ability 3")) UseAbility(Discharge);
            else if (GetAxisDown2("Ability 4")) UseAbility(LightningDash);

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
            if (hit.transform.GetComponent<ConduitStacks>() != null) {
                Instantiate(punchBang, punchBangPoint.position, punchBangPoint.rotation);
                if (hit.transform.GetComponent<ConduitStacks>().Stacks > 0) {
                    List<GameObject> alreadyHit = new List<GameObject>();
                    alreadyHit.Add(hit.transform.gameObject);
                    List<GameObject> hits = ChainLightning(alreadyHit, hit.transform.gameObject, 1);
                    List<GameObject> copies = new List<GameObject>();
                    GameObject lightningBolts = Instantiate(lightningBoltPathPrefab);
                    for (int i = 0; i < hits.Count; i++)
                    {
                        hits[i].GetComponent<ConduitStacks>().AddStack();
                        if (i > 0)
                        {
                            hit.transform.gameObject.SendMessage("TakeDmg", LightingPunch.baseDmg / 2, SendMessageOptions.DontRequireReceiver);
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
                    hit.transform.GetComponent<ConduitStacks>().AddStack();
                }
                hit.transform.gameObject.SendMessage("TakeDmg", LightingPunch.baseDmg, SendMessageOptions.DontRequireReceiver);
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
            Instantiate(staticStompPrefab, this.transform.position, staticStompPrefab.transform.rotation);
        }
        energy = 0;
    }

    #endregion

    #region Ability 3 (Discharge)

    private void Ability3()
    {
        if (lightningLists != null)
        foreach(ConduitStacks c in lightningLists) {
                if (c != null)
                {
                    float stks = c.Stacks;
                    CmdDischargeStacks(c.gameObject);
                    c.gameObject.SendMessage("TakeDmg", LightingPunch.baseDmg * stks, SendMessageOptions.DontRequireReceiver);
                }
            }
    }

    [Command]
    private void CmdDischargeStacks(GameObject o)
    {
        if (o != null)
        {
            if (!isClient) o.SendMessage("Discharge", SendMessageOptions.DontRequireReceiver);
            else RpcDischargeStacks(o);
        }
    }

    [ClientRpc]
    private void RpcDischargeStacks(GameObject o)
    {
        if(o!=null)
        o.SendMessage("Discharge", SendMessageOptions.DontRequireReceiver);
    }

    private void StartAbility3()
    {
        GameObject lightningBolts = Instantiate(dischargeLightningPathPrefab);
        ConduitStacks[] things = GameObject.FindObjectsOfType<ConduitStacks>();
        List<GameObject> lightningList = new List<GameObject>();
        lightningLists = new List<ConduitStacks>();
        lightningList.Add(gameObject);
        for (int i = 0; i < things.Length; i++)
        {
            if (things[i].Stacks > 0)
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
    private void CmdStartAbility3()
    {
        if (!isClient)
        {
            StartAbility3();
        }
        RpcStartAbility3();
    }

    [ClientRpc]
    private void RpcStartAbility3()
    {
        StartAbility3();
    }

    #endregion

    #region Ability 4 (Lightning Dash)

    private void Ability4()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit[] hit = Physics.RaycastAll(ray, LightningDash.range * energy);
        Vector3 telePos = ray.origin + (ray.direction * LightningDash.range * energy);
        if(hit.Length > 0)
            for (int i = 0; i < hit.Length; i++) {
                if(hit[i].transform.tag != "Character" && hit[i].transform.tag != "Destructible" && hit[i].transform.tag != "Player")
                {
                    telePos = ray.origin + (ray.direction * (hit[i].distance - 0.5f));
                    break;
                } else {
    
                    if(LightningDash.range * energy - hit[i].distance < 0.5f) {
                        telePos = ray.origin + (ray.direction * (hit[i].distance - 0.5f));
                    } else if (LightningDash.range * energy - hit[i].distance >= 0.5f && LightningDash.range * energy - hit[i].distance < 1.5f) {
                        telePos = ray.origin + (ray.direction * (hit[i].distance + 1.5f));
                        hit[i].transform.GetComponent<ConduitStacks>().AddStack();
                        hit[i].transform.gameObject.SendMessage("TakeDmg", 0.1f, SendMessageOptions.DontRequireReceiver);
                    } else {
                        hit[i].transform.GetComponent<ConduitStacks>().AddStack();
                        hit[i].transform.gameObject.SendMessage("TakeDmg", 0.1f, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        Vector3 newPos = telePos + Vector3.down;
        this.transform.position = newPos;
        GetComponent<NetworkSyncPosition>().syncPos = newPos;
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

    protected override void UseAbility(Ability a) {
        if (currCooldown <= 0 && energy >= a.energyCost) {
            base.UseAbility(a);
            if (waitingForAbility == 3) {
                CmdStartAbility3();
            }
        }
    }

    private List<GameObject> ChainLightning(List<GameObject> alreadyHit, GameObject justHit, int pass) {
        GameObject candidate = Megamanager.FindClosestAttackable(justHit, pass);
        if (candidate == null)
            return alreadyHit;

        bool invalidCandidate = false;
        foreach (GameObject g in alreadyHit) {
            if (candidate == g)
                invalidCandidate = true;
        }
        if (candidate == this.gameObject || candidate.GetComponent<ConduitStacks>() == null)
            invalidCandidate = true;

        if (Vector3.Distance(candidate.transform.position, justHit.transform.position) > chainLightningRange)
            return alreadyHit;
        if (!invalidCandidate) {
            alreadyHit.Add(candidate);
            if (candidate.GetComponent<ConduitStacks>().Stacks > 0) {
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
}
