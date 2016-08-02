using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour {

    public GameObject owner;
    public float damage;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.transform.tag == "Character" || col.transform.tag == "Player")
        {
            col.SendMessage("TakeDmg", damage);
        }
        Destroy(gameObject);
    }
}
