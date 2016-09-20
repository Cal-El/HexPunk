using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour {

    [HideInInspector]
    public GameObject owner;
    private PlayerStats ownerStats;
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float range;
    public float speed = 1;
    public float splashRadius = 1;
    public float safeWindow = 0.2f;
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

    void OnTriggerEnter(Collider col)
    {        
        if (col != null && !col.isTrigger)
        {
            Character ch = col.GetComponent<Character>();
            //Give the owner a safeWindow
            if (col.gameObject != owner && col != null)
            {
                if (ch != null && !ch.IsInvulnerable())
                {
                    ch.TakeDmg(damage, Character.DamageType.FireElectric, ownerStats);
                    if (ch.burn != null)
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
                    ch.TakeDmg(damage / 2, Character.DamageType.FireElectric, ownerStats);
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

        Instantiate(deathrattle, transform.position, deathrattle.transform.rotation);

        foreach (var target in targets)
        {
            if (target != null && !target.isTrigger)
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
