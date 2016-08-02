using UnityEngine;
using System.Collections;

public class Lavaball : MonoBehaviour {

    public GameObject owner;
    public float damage;

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
            col.SendMessage("TakeDmg", damage);
        }
        Destroy(gameObject);
    }
}
