using UnityEngine;
using System.Collections;

public class Icicle : MonoBehaviour {
    [HideInInspector]
    public GameObject owner;
    private PlayerStats ownerStats;
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float range;
    public float speed = 10;
    private Vector3 startPos;
    public ParticleSystem p;

    void Start()
    {
        startPos = transform.position;
        ownerStats = owner.GetComponent<PlayerStats>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(ownerStats == null)
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
		if (!col.isTrigger && this.enabled && col.tag != "Guard" && col.tag != "Trigger") {
            if (col.gameObject != owner && col != null)
            {
                Character ch = col.GetComponent<Character>();
                if (ch != null && !ch.IsInvulnerable())
                {
                    ch.TakeDmg(damage, Character.DamageType.Standard, ownerStats);
                }
                p.transform.parent = null;
                p.enableEmission = false;
                p.loop = false;
                Destroy(p.gameObject, 5);
                Destroy(gameObject);
            }
        }
    }
}
