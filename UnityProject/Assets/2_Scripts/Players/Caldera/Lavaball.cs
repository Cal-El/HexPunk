﻿using UnityEngine;
using System.Collections;

public class Lavaball : MonoBehaviour {

    [HideInInspector]
    public GameObject owner;
    private PlayerStats ownerStats;
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float range;
    public float damageModifyer = 1;
    public float speed = 1;
    public float splashRadius = 3;
    public float safeWindow = 0.75f;
    private Vector3 startPos;
    public ParticleSystem p;
    public GameObject deathrattle;

    void Start()
    {
        startPos = transform.position;
        safeWindow += Time.time;
        ownerStats = owner.GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ownerStats == null)
            ownerStats = owner.GetComponent<PlayerStats>();
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        if (Vector3.Distance(startPos, transform.position) > range)
        {
            p.transform.parent = null;
            p.enableEmission = false;
            p.loop = false;
            Destroy(p.gameObject, 5);
            Destroy(gameObject);
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (!col.isTrigger && col != null && col.tag != "Guard" && col.tag != "Trigger")
        {
            //Not a spawner
            DestructibleObject dObj = col.GetComponent<DestructibleObject>();
            Character ch = col.GetComponent<Character>();
            if (ch != null && !ch.IsInvulnerable() && dObj == null)
            {
                if (col.gameObject != owner)
                {
                    ch.TakeDmg(damage * Time.deltaTime * damageModifyer, Character.DamageType.FireElectric, ownerStats);
                    if (ch.burn != null)
                        ch.burn.IsBurning = true;
                }
                else
                {
                    if (Time.time > safeWindow)
                    {
                        ch.TakeDmg(damage * Time.deltaTime * damageModifyer / 2, Character.DamageType.FireElectric, ownerStats);
                    }
                }
            }
            else
            {
                p.transform.parent = null;
                p.enableEmission = false;
                p.loop = false;
                Destroy(p.gameObject, 5);
                Splash(transform.position);
                Destroy(gameObject);
            }
        }
    }

    void Splash(Vector3 origin)
    {
        float spashDamage = damage;
        var targets = Physics.OverlapSphere(origin, splashRadius);

        Instantiate(deathrattle, transform.position, deathrattle.transform.rotation);

        foreach (var target in targets)
        {
            if (target != null)
            {
                Character ch = target.GetComponent<Character>();
                if (ch != null && !ch.IsInvulnerable())
                {
                    if (target.gameObject == owner)
                    {
                        ch.TakeDmg(spashDamage / 2, Character.DamageType.FireElectric, ownerStats);
                    }
                    else
                    {
                        ch.TakeDmg(spashDamage, Character.DamageType.FireElectric, ownerStats);
                        if (ch.burn != null)
                            ch.burn.IsBurning = true;
                    }
                }
            }
        }
    }
}
