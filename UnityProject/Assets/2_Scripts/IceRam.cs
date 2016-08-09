using UnityEngine;
using System.Collections;

public class IceRam : MonoBehaviour {

    [HideInInspector]
    public GameObject owner;
    [HideInInspector]
    public float damage;
    public float speed = 8;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
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
                    ch.TakeDmg(damage * Time.deltaTime);
                    Vector3 dir = (col.transform.position - transform.position).normalized;
                    ch.Knockback((new Vector3(dir.x, 0, dir.z) * 1000), 1);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
