using UnityEngine;
using System.Collections;

public class VictoryTrigger : MonoBehaviour {

    private Megamanager megamanager;

	// Use this for initialization
	void Start () {
        megamanager = FindObjectOfType<Megamanager>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(megamanager == null)
            megamanager = FindObjectOfType<Megamanager>();
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            megamanager.Victory();
        }
    }
}
