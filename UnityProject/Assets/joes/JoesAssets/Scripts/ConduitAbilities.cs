﻿using UnityEngine;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Energy < 5) {
            Energy = Mathf.Clamp(Energy + Time.deltaTime, 0, 5);
        }
        if (currCooldown > 0) {
            currCooldown = Mathf.Max(currCooldown - Time.deltaTime, 0);
            this.GetComponent<PlayerMovement>().controlEnabled = false;
        } else {
            this.GetComponent<PlayerMovement>().controlEnabled = true;
            graphicObj.GetComponent<MeshRenderer>().material.color = Color.white;
        }
        if (castingTimer > 0) {
            castingTimer = Mathf.Max(castingTimer - Time.deltaTime, 0);
        }

        Energy = Energy + Time.deltaTime;

        if (Input.GetButtonDown("Ability 1")) UseAbility(LightingPunch);
        if (GetAxisDown1("Ability 2")) UseAbility(StaticStomp);
        else if (Input.GetButtonDown("Ability 3")) UseAbility(Discharge);
        else if (GetAxisDown2("Ability 4")) UseAbility(LightningDash);

        if(waitingForAbility != 0 && castingTimer <= 0) {
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
            }
            waitingForAbility = 0;
        }

    }

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
                            hits[i].transform.SendMessage("TakeDmg", LightingPunch.baseDmg/2, SendMessageOptions.DontRequireReceiver);
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
                hit.transform.SendMessage("TakeDmg", LightingPunch.baseDmg, SendMessageOptions.DontRequireReceiver);
            }
        }
        graphicObj.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

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
        graphicObj.GetComponent<MeshRenderer>().material.color = Color.green;
    }

    private void Ability3()
    {
        if (lightningLists != null)
        foreach(ConduitStacks c in lightningLists) {
            c.Discharge();
        }
        graphicObj.GetComponent<MeshRenderer>().material.color = Color.red;
    }

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
                        hit[i].transform.SendMessage("TakeDmg", 0.1f, SendMessageOptions.DontRequireReceiver);
                    } else {
                        hit[i].transform.GetComponent<ConduitStacks>().AddStack();
                        hit[i].transform.SendMessage("TakeDmg", 0.1f, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        this.transform.position = telePos+Vector3.down;
        Energy = 0;
        graphicObj.GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    protected override void UseAbility(Ability a) {
        if (currCooldown <= 0) {
            base.UseAbility(a);
            if (waitingForAbility == 3) {
                StartAbility3();
            }
        }
    }

    private void StartAbility3() {
        Debug.Log("Method Called");
        GameObject lightningBolts = Instantiate(dischargeLightningPathPrefab);
        ConduitStacks[] things = GameObject.FindObjectsOfType<ConduitStacks>();
        List<GameObject> lightningList = new List<GameObject>();
        lightningLists = new List<ConduitStacks>();
        lightningList.Add(gameObject);
        for (int i = 0; i < things.Length; i++) {
            if (things[i].Stacks > 0) {
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
