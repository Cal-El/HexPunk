using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class DestructibleObject : Character
{

    public GameObject particleEffect;
    private bool _isDestroyed = false;

    [SerializeField]
    private float maxHealth;
    private float health;

    // Use this for initialization
    void Start()
    {
        base.Initialise();
    }

    public override void TakeDmg(float dmg)
    {
        health -= dmg;
        if (health <= 0) {
            if (particleEffect != null) {
                GameObject.Instantiate(particleEffect, this.transform.position, this.transform.rotation);
            }
            Destruct();
        }
    }

    public override float GetHealth() {
        return health;
    }

    public override void Heal(float healVal) {
        health += healVal;
    }

    public override void Knockback(Vector3 force, float timer) {
        //Non Knockbackable
    }

    private void Destruct()
    {
        Megamanager.RemoveCharacterFromList(this);

        if (_isDestroyed) return;

        _isDestroyed = true;

        Destroy(gameObject);
    }
}
