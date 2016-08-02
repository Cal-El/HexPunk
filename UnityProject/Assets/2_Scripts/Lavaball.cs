using UnityEngine;
using System.Collections;

public class Lavaball : MonoBehaviour {

    [HideInInspector]
    public GameObject owner;
    [HideInInspector]
    public float damage;
    public float speed = 1;
    public float splashRadius = 3;
    public float safeWindow = 0.3f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }
    void OnTriggerStay(Collider col)
    {
        if (col.transform.tag == "Character" || col.transform.tag == "Player")
        {
            col.SendMessage("TakeDmg", damage * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Splash(Vector3 origin)
    {
        float spashDamage = damage / 2;
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
