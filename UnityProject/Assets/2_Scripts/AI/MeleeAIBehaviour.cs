using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MeleeAIBehaviour : AIBehaviour {

    [HideInInspector]
    public AIMeleeAudioManager am;

    [Header("Scouting")]
    public bool ImAScout = false;

    //Attack Statistics
    [Header("Combat Statistics")]
    public float baseDmg = 5;                   //Base damage of the melee attack
    public float castingTime = 1;               //The time it takes from starting an attack to it actually connecting with a player
    public float cooldown = 1;                  //The time it between attacks
    private float attackTimer = 0;
    public float range = 2;                     //Range/Reach of the attack
    public float deathTime = 2;
    //Pathfinding Variables
    [HideInInspector]public NavMeshAgent navAgent;
    private NavMeshObstacle navObst;
    private float inactiveTimer = 0.0f;
    private float knockbackTimer = 0.0f;

    void Awake () {
        base.Initialise();

        am = GetComponentInChildren<AIMeleeAudioManager>();

        navAgent = this.GetComponent<NavMeshAgent>();
        navObst = this.GetComponent<NavMeshObstacle>();

        navAgent.speed = navAgent.speed * Random.Range(0.5f, 1.0f);
        if (ImAScout)
        {
            navAgent.avoidancePriority = Random.Range(75, 99);
        }
        else
        {
            navAgent.avoidancePriority = Random.Range(1, 74);
        }

        SetHealth(maxHealth);

        mr.material.SetColor("_EmissionColor", Color.red);
    }
	
	void Update () {
        if (agentState != STATES.Dead) {
            Vector3 prePos = transform.position;
            if(knockbackTimer > 0) {
                knockbackTimer -= Time.deltaTime;
                if(knockbackTimer <= 0) {
                    StartChase();
                }
            }
            if (inactiveTimer <= 0) {
                if (transform.parent == null || transform.parent.GetComponent<Room>().roomUnlocked) {
                    switch (agentState) {
                        case STATES.Idle:
                            
                            animationState = STATES.Idle;
                            IdleBehaviour();
                            break;
                        case STATES.Battlecry:
                            
                            animationState = STATES.Battlecry;
                            BattlecryBehaviour();
                            break;
                        case STATES.Chasing:
                            
                            Retagetting();
                            animationState = STATES.Chasing;
                            if(target != null)
                                ChasingBehaviour();
                            break;
                        case STATES.MeleeAttacking:
                            
                            AttackingBehaviour();
                            break;
                        case STATES.Knockback:
                            
                            break;
                        case STATES.Dead:
                            
                            break;
                    }
                }
                if (navAgent.enabled && navAgent.pathStatus == NavMeshPathStatus.PathComplete) {
                    animationState = STATES.Chasing;
                }
            } else {
                inactiveTimer -= Time.deltaTime;
                
            }
        } else {
            mr.material.SetColor("_EmissionColor", Color.black);
            animationState = STATES.Dead;
            DeadBehaviour();
        }
	}

    private void IdleBehaviour() {
        if(navAgent.avoidancePriority > 75) {
            CheckForPlayers();
        }
    }

    private void BattlecryBehaviour() {
        battlecryTimer -= Time.deltaTime;
        if(battlecryTimer <= 0) {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, battlecryRange, transform.forward, 0);
            foreach(RaycastHit hit in hits) {
                if (hit.transform.GetComponent<AIBehaviour>() != null) {
                    hit.transform.GetComponent<AIBehaviour>().HearBattlecry();
                }
            }
            FindTarget();
            StartChase();
        }
        
    }

    private void ChasingBehaviour() {
        if (navAgent != null && target != null) {
            navAgent.enabled = true;
            navObst.enabled = false;
            navAgent.destination = target.transform.position + (this.transform.position-target.transform.position).normalized*range/2;
        }
        
        
        if (Vector3.Distance(this.transform.position, target.transform.position) < range - 0.5f && knockbackTimer <= 0) {
            navAgent.enabled = false;
            navObst.enabled = true;
            agentState = STATES.MeleeAttacking;
        }
    }

    private void AttackingBehaviour() {
        if(attackTimer > cooldown) {            //Attacking
            animationState = STATES.MeleeAttacking;
            attackTimer -= Time.deltaTime;
            if (attackTimer <= cooldown) {      //Theshold crossed. Time to attack
                RaycastHit hit;
                if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, range)) {
                    if(hit.transform.tag == "Player") {
                        hit.transform.GetComponent<ClassAbilities>().TakeDmg(baseDmg);
                    }
                }
            }
        } else {                                //Not Attacking
            animationState = STATES.Idle;
            transform.LookAt(new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.z));
            attackTimer -= Time.deltaTime;
            if (Vector3.Distance(this.transform.position, target.transform.position) > range) {
                StartChase();
                return;
            }
            if (!target.IsAlive || target.IsInvulnerable()) {
                Retagetting();
            }
            if(attackTimer <= 0) {              //Ready to attack again
                attackTimer = cooldown + castingTime;
            }
        }
    }

    private void KnockbackBehaviour() {
        knockbackTimer -= Time.deltaTime;
        if(knockbackTimer <= 0) {
            StartChase();
        }
    }

    private void DeadBehaviour() {
        
    }

    private void StartChase() {
        inactiveTimer = TIME_FOR_NAVMESH_UPDATE;
        navObst.enabled = false;
        rb.isKinematic = true;
        agentState = STATES.Chasing;
    }

    private void StartDeath(Transform whoKilledMe = null) {
        navObst.enabled = false;
        navAgent.enabled = false;
        rb.isKinematic = true;
        GetComponent<CapsuleCollider>().enabled = false;
        base.Destroyed();
        agentState = STATES.Dead;

        Destroy(gameObject, deathTime);
    }

    public override float GetHealth() {
        return health;
    }

    [ServerCallback]
    protected override void SetHealth(float value)
    {
        base.SetHealth(value);
        if (health <= 0)
        {
            StartDeath();
        }
    }

    protected override void OnHealthChanged(float value)
    {
        base.OnHealthChanged(value);
        if (health <= 0)
        {
            StartDeath();
        }
    }

    public override void Heal(float healVal) {
        SetHealth(Mathf.Clamp(health + healVal, 0, maxHealth));
    }
    
    public override float TakeDmg(float dmg, DamageType damageType = DamageType.Standard, PlayerStats attacker = null)
    {
        if (dmg > 0.02f) {
            SetHealth(Mathf.Clamp(health - dmg, 0, maxHealth));

            BloodSplatterer.MakeBlood(transform.position);

            if (attacker != null && attacker.isLocalPlayer) attacker.CmdAddDamageDealt(dmg);
            if (health <= 0) {
                if (attacker != null && attacker.isLocalPlayer) attacker.CmdAddKills(1);
                if (damageType == DamageType.FireElectric) {
                    am.PlayDeathBurnElectricAudio();
                } else {
                    am.PlayDeathAudio();
                }

                if (xpItem != null) {
                    GameObject g = ServerSpawn(xpItem.gameObject, transform.position, transform.rotation) as GameObject;
                    if (attacker != null && g != null) {
                        var pickup = g.GetComponent<HealthPickup>();
                        if (pickup != null) pickup.SetTarget(attacker.gameObject);
                    }
                }
            } else {
                am.PlayTakeDamageAudio();
            }
        }
        if (agentState == STATES.Idle)
            StartBattlecry();
        return health;
    }

    public override void Knockback(Vector3 force, float timer) {
        if(agentState == STATES.Idle) {
            FindTarget();
        }
        transform.LookAt(transform.position - force, Vector3.up);
        navAgent.enabled = false;
        navObst.enabled = true;
        rb.isKinematic = false;
        rb.AddForce(force);
        knockbackTimer = timer;
        if (GetHealth() > 0) {
            agentState = STATES.Knockback;
        }
    }

    public void CheckForPlayers() {
        foreach(ClassAbilities p in Megamanager.MM.players) {
            if(Vector3.Distance(p.transform.position, transform.position) <= playerPerceptionRange) {
                StartBattlecry();
            }
        }
    }
}
