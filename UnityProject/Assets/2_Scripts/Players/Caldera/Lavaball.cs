using UnityEngine;
using System.Collections;

public class Lavaball : MonoBehaviour {

    [HideInInspector]
    public GameObject owner;
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float range;
    public float damageModifyer = 1;
    public float speed = 1;
    public float splashRadius = 3;
    public float safeWindow = 0.75f;
    private Vector3 startPos;

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
            Destroy(gameObject);
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (!col.isTrigger && col != null)
        {
            Character ch = col.GetComponent<Character>();
            if (ch != null)
            {
                if (col.gameObject != owner)
                {
                    ch.TakeDmg(damage * Time.deltaTime * damageModifyer, Character.DamageType.FireElectric);
                    ch.burn.IsBurning = true;
                }
                else
                {
                    if (Time.time > safeWindow)
                    {
                        ch.TakeDmg(damage * Time.deltaTime * damageModifyer / 2, Character.DamageType.FireElectric);
                    }
                }
            }
            else
            {
                Splash(transform.position);
                Destroy(gameObject);
            }
        }
    }

    void Splash(Vector3 origin)
    {
        float spashDamage = damage;
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
                        ch.TakeDmg(spashDamage / 2, Character.DamageType.FireElectric);
                    }
                    else
                    {
                        ch.TakeDmg(spashDamage, Character.DamageType.FireElectric);
                        ch.burn.IsBurning = true;
                    }
                }
            }
        }
    }
}
