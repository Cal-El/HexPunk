using UnityEngine;
using System.Collections;

public class Lavaball : MonoBehaviour {

    [HideInInspector]
    public GameObject owner;
    [HideInInspector]
    public float damage;
    public float damageModifyer = 1;
    public float speed = 1;
    public float splashRadius = 3;
    public float safeWindow = 0.75f;

    // Use this for initialization
    void Start ()
    {
        safeWindow += Time.time;
    }
	
	// Update is called once per frame
	void Update ()
    {
        safeWindow += Time.deltaTime;
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
    void OnTriggerStay(Collider col)
    {
        if (!col.isTrigger)
        {
            if (col.transform.tag == "Character" || col.transform.tag == "Player")
            {
                if (col.gameObject != owner)
                {
                    col.SendMessage("TakeDmg", damage * Time.deltaTime * damageModifyer);
                    Debug.Log(damage * Time.deltaTime * damageModifyer);
                }
                else
                {
                    if (Time.time > safeWindow)
                    {
                        col.SendMessage("TakeDmg", damage * Time.deltaTime * damageModifyer / 2);
                        Debug.Log(damage * Time.deltaTime * damageModifyer / 2);
                    }
                }
            }
            else
            {
                Splash(col.transform.position);
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
            if (target.transform.tag == "Character" || target.transform.tag == "Player")
            {
                if (target.gameObject == owner)
                {
                    target.SendMessage("TakeDmg", spashDamage / 2);
                }
                else
                {
                    target.SendMessage("TakeDmg", spashDamage);
                }
            }
        }
    }
}
