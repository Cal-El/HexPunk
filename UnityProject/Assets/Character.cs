using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public abstract class Character : NetworkBehaviour {

    [HideInInspector]
    public ConduitStacks stacks;
    [HideInInspector]
    public CalderaBurnDamage burn;
    [HideInInspector]
    public Rigidbody rb;

    void Start() {
        stacks = GetComponent<ConduitStacks>();
        burn = GetComponent<CalderaBurnDamage>();
        rb = GetComponent<Rigidbody>();
    }

    public abstract float GetHealth();

    public abstract void Heal(float healVal);

    public abstract void TakeDmg(float dmg);

    public abstract void Knockback(Vector3 force);

}
