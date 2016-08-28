using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public abstract class Character : NetworkBehaviour {

    public enum DamageType { Standard, FireElectric };

    [HideInInspector]
    public ConduitStacks stacks;
    [HideInInspector]
    public CalderaBurnDamage burn;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector] public Room myRoom;

    /// <summary>
    /// To be called by all start functions
    /// </summary>
    protected void Initialise() {
        Megamanager.AddCharacterToList(this);
        stacks = GetComponent<ConduitStacks>();
        burn = GetComponent<CalderaBurnDamage>();
        rb = GetComponent<Rigidbody>();
    }

    protected void Destroyed() {
        Megamanager.RemoveCharacterFromList(this);
        if(myRoom != null) myRoom.RemoveCharacter(this);
    }

    public abstract float GetHealth();

    public abstract void Heal(float healVal);

    public abstract void TakeDmg(float dmg, DamageType damageType = DamageType.Standard);

    public abstract void Knockback(Vector3 force, float timer);

    public abstract bool IsInvulnerable();
}
