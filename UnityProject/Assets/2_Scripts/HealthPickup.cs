using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour {

    public GameObject particleEffect;
    private const float HEALVAL = 5;
    private const float MAXSPEED = 5;
    private const float ATTRACTIONRANGE = 4;
    private Vector3 velocity;
    private Transform target;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(target != null) {
            velocity += (target.position - transform.position).normalized*ATTRACTIONRANGE / Mathf.Pow((target.position - transform.position).magnitude,2)* Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, MAXSPEED);
            transform.position += velocity;
            if (Vector3.Distance(target.position, transform.position) < 1f)
                TriggerPickup(target);

        } else {
            target = FindClosestPlayer();
        }
	}

    void OnTriggerEnter(Collider e) {
        if(e.tag == "Player") {
            TriggerPickup(e.transform);
        }
    }

    private void TriggerPickup(Transform e) {
        e.GetComponent<ClassAbilities>().Heal(HEALVAL);
        if (particleEffect != null) {
            Instantiate(particleEffect, this.transform.position, this.transform.rotation);
        }
        Destroy(this.gameObject);
    }

    private Transform FindClosestPlayer() {
        Transform returnVal = null;
        if (Megamanager.MM.players != null) {
            foreach (GameObject g in Megamanager.MM.players) {
                if (Vector3.Distance(g.transform.position, transform.position) < ATTRACTIONRANGE) {
                    if (returnVal == null) {
                        returnVal = g.transform;
                    } else {
                        if (Vector3.Distance(returnVal.position, transform.position) > Vector3.Distance(g.transform.position, transform.position)) {
                            returnVal = g.transform;
                        }
                    }
                }
            }
        }
        return returnVal;
    }

    public void TakeDmg() {
        Destroy(this.gameObject);
    }
}
