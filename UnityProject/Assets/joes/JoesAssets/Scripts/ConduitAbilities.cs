using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ConduitAbilities : ClassAbilities {

    public float chainLightningRange = 5.0f;
    public GameObject lightningBoltPathPrefab;
    public GameObject staticStompPrefab;
    public GameObject dischargeLightningPathPrefab;

    private Ability LightingPunch;
    private Ability StaticStomp;
    private Ability Discharge;
    private Ability LightningDash;
    private Ability Revive;

    private List<ConduitStacks> lightningLists;

    // Use this for initialization
    void Start () {
        base.Initialize();
        LightingPunch.abilityNum = 1;
        LightingPunch.baseDmg = 1;
        LightingPunch.castingTime = 0.25f;
        LightingPunch.cooldown = 0.25f;
        LightingPunch.range = 2;

        StaticStomp.abilityNum = 2;
        StaticStomp.baseDmg = 0;
        StaticStomp.castingTime = 0.5f;
        StaticStomp.cooldown = 0.25f;
        StaticStomp.range = 1;

        Discharge.abilityNum = 3;
        Discharge.baseDmg = 0;
        Discharge.castingTime = 1;
        Discharge.cooldown = 0.5f;
        Discharge.range = 1000;

        LightningDash.abilityNum = 4;
        LightningDash.baseDmg = 0;
        LightningDash.castingTime = 0.25f;
        LightningDash.cooldown = 0.25f;
        LightningDash.range = 1.0f;

        Revive.abilityNum = 5;
        Revive.castingTime = 1f;
        Revive.cooldown = 0.25f;
        Revive.range = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //Used for testing
        if (Input.GetKeyDown(KeyCode.O))
        {
            CmdAddHealth(10);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            CmdAddHealth(-10);
        }

        //Energy
        if (Energy < 5) {
            Energy = Mathf.Clamp(Energy + Time.deltaTime, 0, 5);
        }
        if (currCooldown > 0) {
            currCooldown = Mathf.Max(currCooldown - Time.deltaTime, 0);
            this.GetComponent<PlayerMovement>().ControlEnabled = false;
        } else
        {
            if (canControl) this.GetComponent<PlayerMovement>().ControlEnabled = true;
            CmdChangeGraphicColour(Color.white);
        }
        if (castingTimer > 0) {
            castingTimer = Mathf.Max(castingTimer - Time.deltaTime, 0);
        }

        Energy = Energy + Time.deltaTime;

        //Death
        if (Health <= 0) CmdDeath();

        if (isReviving) CmdRevive();

        //Abilities
        if (!canControl) return;

        if (Input.GetButtonDown("Ability 1")) UseAbility(LightingPunch);
        if (GetAxisDown1("Ability 2")) UseAbility(StaticStomp);
        else if (Input.GetButtonDown("Ability 3")) UseAbility(Discharge);
        else if (GetAxisDown2("Ability 4")) UseAbility(LightningDash);

        //Revive
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, LightingPunch.range))
            {
                if (hit.transform.tag == "Player" && hit.transform.GetComponent<ClassAbilities>().Health <= 0)
                {
                    UseAbility(Revive);
                }
            }
        }

        if (waitingForAbility != 0 && castingTimer <= 0) {
            switch (waitingForAbility) {
                case 1:
                    Ability1();
                    break;
                case 2:
                    Ability2();
                    break;
                case 3:
                    Ability3();
                    break;
                case 4:
                    Ability4();
                    break;
                case 5:
                    Ability5();
                    break;
            }
            waitingForAbility = 0;
        }

        base.BaseUpdate();
    }

    #region Networking Helpers

    [Command]
    private void CmdAddHealth(float hp)
    {
        if (!isClient) Health += hp;
        RpcAddHealth(hp);
    }

    [ClientRpc]
    private void RpcAddHealth(float hp)
    {
        Health += hp;
    }

    [Command]
    private void CmdChangeGraphicColour(Color colour)
    {
        if (!isClient) graphicObj.GetComponent<MeshRenderer>().material.color = colour;
        RpcChangeGraphicColour(colour);
    }

    [ClientRpc]
    private void RpcChangeGraphicColour(Color colour)
    {
        graphicObj.GetComponent<MeshRenderer>().material.color = colour;
    }

    [Command]
    private void CmdTakeDmg(GameObject o, float damage)
    {
        o.SendMessage("TakeDmg", damage, SendMessageOptions.DontRequireReceiver);
    }

    #endregion

    #region Ability 1 (Lightning punch)

    private void Ability1()
    {
        Ray ray = new Ray(transform.position+Vector3.up, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, LightingPunch.range)) {
            if (hit.transform.GetComponent<ConduitStacks>() != null) {
                if (hit.transform.GetComponent<ConduitStacks>().Stacks > 0) {
                    List<GameObject> alreadyHit = new List<GameObject>();
                    alreadyHit.Add(hit.transform.gameObject);
                    List<GameObject> hits = ChainLightning(alreadyHit, hit.transform.gameObject, 1);
                    List<GameObject> copies = new List<GameObject>();
                    GameObject lightningBolts = Instantiate(lightningBoltPathPrefab);
                    for (int i = 0; i < hits.Count; i++) {
                        hits[i].GetComponent<ConduitStacks>().AddStack();
                        if (i>0)
                            CmdTakeDmg(hit.transform.gameObject, LightingPunch.baseDmg / 2);
                        if (hits[i] == null) {
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
                CmdTakeDmg(hit.transform.gameObject, LightingPunch.baseDmg);
            }
        }
        CmdChangeGraphicColour(Color.blue);
    }

    #endregion

    #region Ability 2 (Static stomp)

    private void Ability2()
    {
        RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, StaticStomp.range * Energy, transform.forward, 0.0f);
        foreach(RaycastHit h in hits) {
            if (h.transform.GetComponent<ConduitStacks>() != null)
                h.transform.GetComponent<ConduitStacks>().AddStack();
        }
        GameObject blast = Instantiate(staticStompPrefab, this.transform.position, this.transform.rotation) as GameObject;
        blast.GetComponent<StaticStompVisual>().lifeTime = Energy;
        Energy = 0;
        CmdChangeGraphicColour(Color.green);
    }

    #endregion

    #region Ability 3 (Discharge)

    private void Ability3()
    {
        if (lightningLists != null)
        foreach(ConduitStacks c in lightningLists) {
                if (c != null)
                {
                    CmdTakeDmg(c.gameObject, LightingPunch.baseDmg);
                    CmdDischargeStacks(c.gameObject);
                }
            }
        CmdChangeGraphicColour(Color.red);
    }

    [Command]
    private void CmdDischargeStacks(GameObject o)
    {
        if (!isClient) o.SendMessage("Discharge", SendMessageOptions.DontRequireReceiver);
        RpcDischargeStacks(o);
    }

    [ClientRpc]
    private void RpcDischargeStacks(GameObject o)
    {
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

    #endregion

    #region Ability 4 (Lightning Dash)

    private void Ability4()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit[] hit = Physics.RaycastAll(ray, LightningDash.range * Energy);
        Vector3 telePos = ray.origin + (ray.direction * LightningDash.range * Energy);
        if(hit.Length > 0)
            for (int i = 0; i < hit.Length; i++) {
                if(hit[i].transform.tag != "Character" && hit[i].transform.tag != "Destructible") {
                    telePos = ray.origin + (ray.direction * (hit[i].distance - 0.5f));
                    break;
                } else {
    
                    if(LightningDash.range * Energy - hit[i].distance < 0.5f) {
                        telePos = ray.origin + (ray.direction * (hit[i].distance - 0.5f));
                    } else if (LightningDash.range * Energy - hit[i].distance >= 0.5f && LightningDash.range * Energy - hit[i].distance < 1.5f) {
                        telePos = ray.origin + (ray.direction * (hit[i].distance + 1.5f));
                        hit[i].transform.GetComponent<ConduitStacks>().AddStack();
                        CmdTakeDmg(hit[i].transform.gameObject, 0.1f);
                    } else {
                        hit[i].transform.GetComponent<ConduitStacks>().AddStack();
                        CmdTakeDmg(hit[i].transform.gameObject, 0.1f);
                    }
                }
            }
        this.transform.position = telePos+Vector3.down;
        Energy = 0;
        CmdChangeGraphicColour(Color.yellow);
    }

    #endregion

    #region Ability 5 (Revive)

    private void Ability5()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, LightingPunch.range))
        {
            CmdCallRevive(hit.transform.gameObject);
            CmdChangeGraphicColour(Color.magenta);
        }
    }

    #endregion

    protected override void UseAbility(Ability a) {
        if (currCooldown <= 0) {
            base.UseAbility(a);
            if (waitingForAbility == 3) {
                StartAbility3();
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
