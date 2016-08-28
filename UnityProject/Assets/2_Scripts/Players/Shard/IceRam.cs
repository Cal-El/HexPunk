﻿using UnityEngine;
using System.Collections;

public class IceRam : MonoBehaviour {

    [HideInInspector]
    public GameObject owner;
    [HideInInspector]
    public float damage;
    public float knockBack;
    [HideInInspector]
    public float range;
    public float speed = 8;
    private Vector3 startPos;

    public ParticleSystem p;

    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
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
        if (!col.isTrigger && col != null)
        {
            Character ch = col.GetComponent<Character>();
            if (ch != null && !ch.IsInvulnerable())
            {
                if (col.gameObject != owner)
                {
                    ch.TakeDmg(damage * Time.deltaTime);
                    Vector3 dir = (col.transform.position - transform.position).normalized;
                    ch.Knockback((new Vector3(dir.x, 0, dir.z) * knockBack), 1);
                }
            }
            else
            {
                p.transform.parent = null;
                p.enableEmission = false;
                p.loop = false;
                Destroy(p.gameObject, 5);
                Destroy(gameObject);
            }
        }
    }
}
