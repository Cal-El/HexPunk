using UnityEngine;
using System.Collections;

public class Waterspray : MonoBehaviour
{
    [HideInInspector]
    public GameObject owner;
    [HideInInspector]
    public float damage;

    void OnTriggerStay(Collider col)
    {
        if (!col.isTrigger)
        {
            if (col.gameObject != owner && col != null)
            {
                Character ch = col.GetComponent<Character>();
                if (ch != null)
                {
                    ch.TakeDmg(damage * Time.deltaTime);
                    Vector3 dir = (col.transform.position - transform.position).normalized;
                    ch.Knockback((new Vector3(dir.x, 0, dir.z) * 200), 0.01f);
                }
            }
        }
    }
}
