using UnityEngine;
using System.Collections;

public class Icicle : MonoBehaviour {
    [HideInInspector]
    public GameObject owner;
    [HideInInspector]
    public float damage;
    public float speed = 10;
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    void OnTriggerEnter(Collider col)
    {
        if (!col.isTrigger)
        {
            if (col.gameObject != owner && col != null)
            {
                if (col.transform.tag == "Character" || col.transform.tag == "Player")
                {
                    col.SendMessage("TakeDmg", damage);
                }
                Destroy(gameObject);
            }
        }
    }
}
