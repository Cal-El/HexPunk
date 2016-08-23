using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour {

    [HideInInspector]
    public GameObject owner;
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float range;
    public float speed = 1;
    public float splashRadius = 1;
    public float safeWindow = 0.2f;
    private Vector3 startPos;
    public ParticleSystem p;

    void Start()
    {
        startPos = transform.position;
        safeWindow = Time.time;
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

    void OnTriggerEnter(Collider col)
    {
        if (!col.isTrigger)
        {
            Character ch = col.GetComponent<Character>();
            //Give the owner a safeWindow
            if (col.gameObject != owner && col != null)
            {
                if (ch != null)
                {
                    ch.TakeDmg(damage);
                    ch.burn.IsBurning = true;
                }
                Splash(transform.position);
                p.transform.parent = null;
                p.enableEmission = false;
                p.loop = false;
                Destroy(p.gameObject, 5);
                Destroy(gameObject);
            }
            //Damage owner if they run in to it
            else
            {
                if (Time.time > safeWindow)
                {
                    ch.TakeDmg(damage / 2);
                    Splash(transform.position);
                    p.transform.parent = null;
                    p.enableEmission = false;
                    p.loop = false;
                    Destroy(p.gameObject, 5);
                    Destroy(gameObject);
                }
            }
        }
    }

    void Splash(Vector3 origin)
    {
        float spashDamage = damage / 2;
        var targets = Physics.OverlapSphere(origin, splashRadius);

        foreach (var target in targets)
        {
            if (target != null)
            {
                Character ch = target.GetComponent<Character>();
                if (ch != null)
                {
                    if (target.gameObject == owner)
                    {
                        ch.TakeDmg(spashDamage / 2);
                    }
                    else
                    {
                        ch.TakeDmg(spashDamage);
                        ch.burn.IsBurning = true;
                    }
                }
            }
        }
    }
}
