using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ConduitAbilities : NetworkBehaviour {

    public Transform graphicObj;

    private bool isAxisInUse1 = false;
    private bool isAxisInUse2 = false;
    private bool isAxisDown1 = false;
    private bool isAxisDown2 = false;

    private struct Ability {
        public int abilityNum;
        public float baseDmg;
        public float castingTime;
        public float cooldown;
        public float range;
    };

    private Ability LightingPunch;
    private Ability StaticStomp;
    private Ability Discharge;
    private Ability LightningDash;

    private float chargeLevel = 5.0f;
    private float currCooldown = 0.0f;
    private float castingTimer = 0.0f;
    private int waitingForAbility = 0; //0 means none

    // Use this for initialization
    void Start () {
        LightingPunch.abilityNum = 1;
        LightingPunch.baseDmg = 1;
        LightingPunch.castingTime = 0.25f;
        LightingPunch.cooldown = 0.25f;
        LightingPunch.range = 1;

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
        LightningDash.range = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (chargeLevel < 5) {
            chargeLevel = Mathf.Clamp(chargeLevel + Time.deltaTime, 0, 5);
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

    private void UseAbility(Ability a) {
        if (currCooldown <= 0) {
            castingTimer = a.castingTime;
            waitingForAbility = a.abilityNum;
            currCooldown = a.castingTime + a.cooldown;
        }
    }

    private void Ability1()
    {
        graphicObj.GetComponent<MeshRenderer>().material.color = Color.blue;
        Debug.Log("Ability 1");
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
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hit = Physics.RaycastAll(ray, LightningDash.range * chargeLevel);
        Vector3 telePos = ray.origin + (ray.direction * LightningDash.range * chargeLevel);
        if(hit.Length > 0)
            for (int i = 0; i < hit.Length; i++) {
                if(hit[i].transform.tag != "Character" && hit[i].transform.tag != "Destructible") {
                    telePos = ray.origin + (ray.direction * (hit[i].distance - 0.5f));
                    break;
                } else {
    
                    if(LightningDash.range * chargeLevel - hit[i].distance < 0.5f) {
                        telePos = ray.origin + (ray.direction * (hit[i].distance - 0.5f));
                    } else if (LightningDash.range * chargeLevel - hit[i].distance >= 0.5f && LightningDash.range * chargeLevel - hit[i].distance < 1.5f) {
                        telePos = ray.origin + (ray.direction * (hit[i].distance + 1.5f));
                        hit[i].transform.GetComponent<ConduitStacks>().AddStack();
                        hit[i].transform.SendMessage("TakeDmg", 0.1f, SendMessageOptions.DontRequireReceiver);
                    } else {
                        hit[i].transform.GetComponent<ConduitStacks>().AddStack();
                        hit[i].transform.SendMessage("TakeDmg", 0.1f, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        this.transform.position = telePos;
        graphicObj.GetComponent<MeshRenderer>().material.color = Color.yellow;
        Debug.Log("Ability 4");
    }

    private bool GetAxisDown1(string axis)
    {
        if (Input.GetAxisRaw(axis) != 0)
        {
            if (isAxisInUse1 == false)
            {
                isAxisDown1 = true;
                isAxisInUse1 = true;
            } else
            {
                isAxisDown1 = false;
            }
        }
        if (Input.GetAxisRaw(axis) == 0)
        {
            isAxisInUse1 = false;
            isAxisDown1 = false;
        }
        return isAxisDown1;
    }

    private bool GetAxisDown2(string axis)
    {
        if (Input.GetAxisRaw(axis) != 0)
        {
            if (isAxisInUse2 == false)
            {
                isAxisDown2 = true;
                isAxisInUse2 = true;
            }
            else
            {
                isAxisDown2 = false;
            }
        }
        if (Input.GetAxisRaw(axis) == 0)
        {
            isAxisInUse2 = false;
            isAxisDown2 = false;
        }
        return isAxisDown2;
    }
}
