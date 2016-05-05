using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ConduitAbilities : ClassAbilities {

    public float chainLightningRange = 5.0f;

    private Ability LightingPunch;
    private Ability StaticStomp;
    private Ability Discharge;
    private Ability LightningDash;

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
                    ChainLightning(alreadyHit, hit.transform.gameObject, 1);
                }
                Debug.Break();
                hit.transform.GetComponent<ConduitStacks>().AddStack();
                //hit.transform.SendMessage("TakeDmg", LightingPunch.baseDmg, SendMessageOptions.DontRequireReceiver);
            }
        }
        graphicObj.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    private void Ability2()
    {
        graphicObj.GetComponent<MeshRenderer>().material.color = Color.green;
        Debug.Log("Ability 2");
    }

    private void Ability3()
    {
        graphicObj.GetComponent<MeshRenderer>().material.color = Color.red;
        Debug.Log("Ability 3");
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

    private void ChainLightning(List<GameObject> alreadyHit, GameObject justHit, int pass) {
        GameObject candidate = Megamanager.FindClosestAttackable(justHit, pass);
        if (candidate == null)
            return;

        bool invalidCandidate = false;
        foreach (GameObject g in alreadyHit) {
            if (candidate == g)
                invalidCandidate = true;
        }
        if (candidate == this.gameObject || candidate.GetComponent<ConduitStacks>() == null)
            invalidCandidate = true;

        if (Vector3.Distance(candidate.transform.position, justHit.transform.position) > chainLightningRange)
            return;
        if (!invalidCandidate) {
            if (candidate.GetComponent<ConduitStacks>().Stacks > 0) {
                alreadyHit.Add(candidate);
                ChainLightning(alreadyHit, candidate, 1);
                Debug.DrawLine(justHit.transform.position, candidate.transform.position, Color.cyan);
            }
            candidate.GetComponent<ConduitStacks>().AddStack();
            //justHit.SendMessage("TakeDmg", LightingPunch.baseDmg / 2, SendMessageOptions.DontRequireReceiver); 
        } else {
            if (pass > 5)
                return;
            else
                ChainLightning(alreadyHit, justHit, pass + 1);
        }
    }
}
