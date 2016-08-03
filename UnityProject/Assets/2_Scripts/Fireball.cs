using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour {

    [HideInInspector]
    public GameObject owner;
    [HideInInspector]
    public float damage;
    public float speed = 1;
    public float splashRadius = 1;
    public float safeWindow = 0.2f;

    // Use this for initialization
    void Start () {
        safeWindow += Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    void OnTriggerEnter(Collider col)
    {
        if (!col.isTrigger)
        {
            //Give the owner a safeWindow
            if (col.gameObject != owner && col != null)
            {
                if (col.transform.tag == "Character" || col.transform.tag == "Player")
                {
                    col.SendMessage("TakeDmg", damage);
                    CalderaBurnDamage burn = col.GetComponent<CalderaBurnDamage>();
                    if (burn != null) burn.IsBurning = true;
                }
                Splash(transform.position);
                Destroy(gameObject);
            }
            //Damage owner if they run in to it
            else
            {
                if (Time.time > safeWindow)
                {
                    col.SendMessage("TakeDmg", damage / 2);
                    Splash(transform.position);
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
                if (target.transform.tag == "Character" || target.transform.tag == "Player")
                {
                    if (target.gameObject == owner)
                    {
                        target.SendMessage("TakeDmg", spashDamage / 2);
                    }
                    else
                    {
                        target.SendMessage("TakeDmg", spashDamage);
                        CalderaBurnDamage burn = target.GetComponent<CalderaBurnDamage>();
                        if(burn != null) burn.IsBurning = true;
                    }
                }
            }
        }
    }
}
