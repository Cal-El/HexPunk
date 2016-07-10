using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BetrayerBehaviour : MonoBehaviour
{
    public List<GameObject> players = new List<GameObject>();

    private struct BetrayerDamage
    {
        private GameObject player;
        private float damage;
    }

    //Keep record of damage betrayer does to players
    private BetrayerDamage[] damageInflicted = new BetrayerDamage[3];

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
