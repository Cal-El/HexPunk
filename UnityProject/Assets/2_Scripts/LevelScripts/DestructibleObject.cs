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
    [SyncVar (hook = "OnHealthChanged")]
    private float health;

    void Start() {
        Initialize();
    }

    // Use this for initialization
    protected void Initialize()
    {
        SetHealth(maxHealth);
        base.Initialise();
    }

    public override float TakeDmg(float dmg, DamageType damageType = DamageType.Standard)
    {
        SetHealth(health - dmg);
        return health;
    }

    public override float GetHealth() {
        return health;
    }

    [ServerCallback]
    protected void SetHealth(float value)
    {
        health = value;
        if (health <= 0)
        {
            if (particleEffect != null)
            {
                GameObject.Instantiate(particleEffect, this.transform.position, this.transform.rotation);
            }
            Destruct();
        }
    }

    protected void OnHealthChanged(float value)
    {
        health = value;
        if (health <= 0)
        {
            if (particleEffect != null)
            {
                GameObject.Instantiate(particleEffect, this.transform.position, this.transform.rotation);
            }
            Destruct();
        }
    }

    public override void Heal(float healVal) {
        SetHealth(health + healVal);
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
    
    public override bool IsInvulnerable()
    {
        return false;
    }
}
