using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HealthPickup : NetworkBehaviour {

    public enum TYPE { Health, XP}
    public TYPE pickupType = TYPE.Health;

    public GameObject particleEffect;

    [SyncVar]
    private Vector3 syncPosition;
    private  float lerp = 15;

    [SerializeField]
    private float healVal = 5;

    [SerializeField]
    private float xpVal = 0.1f;

    private const float MAXSPEED = 5;
    private const float ATTRACTIONRANGE = 4;
    private float forceModifier = 1;
    private Vector3 velocity;
    private Transform target;
    private Vector3 originalPos;

    // Use this for initialization
    void Start () {
        originalPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        ServerUpdatePosition();
    }

    [ServerCallback]
    void ServerUpdatePosition()
    {
        Transform t = UpdatePosition();
        RpcUpdatePosition(t.position, t.rotation);
    }

    [ClientRpc]
    void RpcUpdatePosition(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }

    Transform UpdatePosition()
    {
        Transform t = transform;
        t.Rotate(0, Time.deltaTime * 180, 0);
        if (target != null)
        {
            forceModifier += Time.deltaTime;
            velocity *= 0.8f;
            velocity += (target.position + Vector3.up - t.position).normalized * forceModifier * ATTRACTIONRANGE / Mathf.Min(Mathf.Pow((target.position + Vector3.up - t.position).magnitude, 2), 5) * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, MAXSPEED);
            t.position += velocity;

        }
        else
        {
            t.position = originalPos + Vector3.up * Mathf.Sin(Time.timeSinceLevelLoad * 0.5f * Mathf.PI * 2) * 0.1f;
            target = FindClosestPlayer();
        }
        return t;
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

    [ServerCallback]
    public virtual void SetTarget(GameObject attacker)
    {
        if (!isClient)
        {
            target = attacker.transform;
        }
        RpcSetTarget(attacker);
    }

    [ClientRpc]
    private void RpcSetTarget(GameObject attacker)
    {
        target = attacker.transform;
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
