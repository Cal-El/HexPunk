using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour {

    public enum TYPE { Health, XP}
    public TYPE pickupType = TYPE.Health;

    public GameObject particleEffect;

    [SerializeField]
    private float healVal = 5;

    [SerializeField]
    private float xpVal = 0.1f;

    private const float MAXSPEED = 5;
    private const float ATTRACTIONRANGE = 4;
    private Vector3 velocity;
    private Transform target;
    private Vector3 originalPos;

    // Use this for initialization
    void Start () {
        originalPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, Time.deltaTime * 180, 0);
        if (target != null) {
            velocity += (target.position + Vector3.up - transform.position).normalized * ATTRACTIONRANGE / Mathf.Pow((target.position+Vector3.up - transform.position).magnitude, 2) * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, MAXSPEED);
            transform.position += velocity;
            if (Vector3.Distance(target.position, transform.position) < 1f)
                TriggerPickup(target);

        } else {
            transform.position = originalPos + Vector3.up * Mathf.Sin(Time.timeSinceLevelLoad*0.5f * Mathf.PI * 2)*0.1f;
            target = FindClosestPlayer();
            
        }
	}

    void OnTriggerEnter(Collider e) {
        if(e.tag == "Player") {
            TriggerPickup(e.transform);
        }
    }

    private void TriggerPickup(Transform e) {
        if(pickupType == TYPE.Health)
            e.GetComponent<ClassAbilities>().Heal(healVal);
        else
            e.GetComponent<ClassAbilities>().GainXP(xpVal);
        

        if (particleEffect != null) {
            Instantiate(particleEffect, this.transform.position, this.transform.rotation);
        }
        Destroy(this.gameObject);
    }

    private Transform FindClosestPlayer() {
        Transform returnVal = null;
        if (Megamanager.MM.players != null) {
            foreach (ClassAbilities g in Megamanager.MM.players) {
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
