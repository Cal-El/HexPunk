using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EliteAIBehaviour : AIBehaviour {

    public AIEliteAudioManager am;

    //Attack Statistics
    
    [System.Serializable]
    public struct Attack {
        public float baseDmg;                   //Base damage of the melee attack
        public float castingTime;               //The time it takes from starting an attack to it actually connecting with a player
        public float cooldown;                  //The time it between attacks
        public float attackTimer;
        public float range;                     //Range/Reach of the attack
        public float knockback;
    }
    [Header("Melee Attack")]
    public Attack meleeAttack;
    [Header("Ranged Attack")]
    public Attack rangeAttack;
    private Vector3 rangeTarget;
    [SerializeField]
    private GameObject warningEffect;
    [SerializeField]
    private GameObject beamEffect;

    //Pathfinding Variables
    [HideInInspector]public NavMeshAgent navAgent;
    private NavMeshObstacle navObst;
    private float inactiveTimer = 0.0f;

    void Awake () {
        base.Initialise();

        agentState = STATES.Idle;

        am = GetComponentInChildren<AIEliteAudioManager>();

        warningEffect.SetActive(false);
        beamEffect.SetActive(false);

        navAgent = this.GetComponent<NavMeshAgent>();
        navObst = this.GetComponent<NavMeshObstacle>();

        navAgent.speed = navAgent.speed * Random.Range(0.5f, 1.0f);
        navAgent.avoidancePriority = Random.Range(90,99);
        SetHealth(maxHealth);

        mr.material.SetColor("_EmissionColor", Color.red);
    }
	
	void Update () {
        if (agentState != STATES.Dead) {
            Vector3 prePos = transform.position;
            if (inactiveTimer <= 0) {
                if (transform.parent == null || transform.parent.GetComponent<Room>().roomUnlocked) {
                    if (agentState != STATES.RangedAttacking && agentState != STATES.Idle && agentState != STATES.Battlecry) {
                        if (rangeAttack.attackTimer <= 0) {
                            agentState = STATES.RangedAttacking;
                        } else {
                            rangeAttack.attackTimer -= Time.deltaTime;
                        }
                    }
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
                        case STATES.RangedAttacking:
                            
                            animationState = STATES.RangedAttacking;
                            RangeAttackingBehaviour();
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
            navAgent.destination = target.transform.position + (this.transform.position-target.transform.position).normalized* meleeAttack.range /2;
        }
        
        
        if (Vector3.Distance(this.transform.position, target.transform.position) < meleeAttack.range - 0.5f) {
            navAgent.enabled = false;
            navObst.enabled = true;
            agentState = STATES.MeleeAttacking;
        }
    }

    private void AttackingBehaviour() {
        if(meleeAttack.attackTimer > meleeAttack.cooldown) {            //Attacking
            animationState = STATES.MeleeAttacking;
            meleeAttack.attackTimer -= Time.deltaTime;
            if (meleeAttack.attackTimer <= meleeAttack.cooldown) {      //Theshold crossed. Time to attack
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, meleeAttack.range, transform.forward, 0);
                foreach (RaycastHit hit in hits) {
                    if (Vector3.Angle(hit.transform.position - transform.position, transform.forward) < 90) {
                        Character c = hit.transform.GetComponent<Character>();
                        if (c != null) {
                            c.Knockback(transform.forward * meleeAttack.knockback, 1);
                            c.TakeDmg(meleeAttack.baseDmg);

                        }

                    }
                }
            }
        } else {                                //Not Attacking
            animationState = STATES.Idle;
            transform.LookAt(new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.z));
            meleeAttack.attackTimer -= Time.deltaTime;
            if (Vector3.Distance(this.transform.position, target.transform.position) > meleeAttack.range) {
                StartChase();
                return;
            }
            if (!target.IsAlive) {
                Retagetting();
            }
            if(meleeAttack.attackTimer <= 0) {              //Ready to attack again
                meleeAttack.attackTimer = meleeAttack.cooldown + meleeAttack.castingTime;
            }
        }
    }

    private void RangeAttackingBehaviour() {
        //Target player
        if (rangeAttack.attackTimer <= 0) {
            navAgent.enabled = false;
            navObst.enabled = true;

            //Find farthest player in los
            target = null;
            foreach (ClassAbilities p in Megamanager.MM.players) {
                bool inLOS = false;
                if (!p.IsInvulnerable()) {
                    Ray ray = new Ray(transform.position, (p.Position - transform.position).normalized);
                    RaycastHit[] hits = Physics.RaycastAll(ray, rangeAttack.range);
                    hits = Megamanager.SortByDistance(hits);
                    for (int i = 0; i < hits.Length; i++) {
                        Character ch = hits[i].transform.GetComponent<Character>();
                        Debug.Log(i + ": " + hits[i].transform.name);
                        if (ch == p) {
                            inLOS = true;
                        } else if (ch != null) {
                        } else { break; }
                    }
                    if (inLOS) {
                        if (target == null) {
                            target = p;
                        } else if (Vector3.Distance(target.Position, transform.position) < Vector3.Distance(p.Position, transform.position)) {
                            target = p;
                        } else { continue; }
                    } else { continue; }
                } else { continue; }
            }
            //No Target found
            if (target == null) {
                rangeAttack.attackTimer = rangeAttack.cooldown;
                StartChase();
                Debug.Log("No targetFound");
            } else {
                rangeAttack.attackTimer = 0 + Time.deltaTime;
                warningEffect.SetActive(true);
                rangeTarget = target.Position;
            }
        } else if (rangeAttack.attackTimer >= rangeAttack.castingTime && target != null) {
            //Time to attack
            Ray ray = new Ray(transform.position, rangeTarget - transform.position);
            RaycastHit[] hits = Physics.SphereCastAll(ray, 1, rangeAttack.range);

            hits = Megamanager.SortByDistance(hits);

            foreach (RaycastHit hit in hits) {
                if (hit.transform == null) {
                    Debug.LogError("Null transform hit at " + hit.point);
                    continue;
                }
                Character ch = hit.transform.GetComponent<Character>();
                if (ch != null) {
                    if (ch == this) continue;
                    ch.TakeDmg(rangeAttack.baseDmg);
                    ch.Knockback(transform.forward * 1000, 1);
                } else { break; }
            }
            beamEffect.SetActive(true);
            warningEffect.SetActive(false);
            target = null;
        } else if (rangeAttack.attackTimer >= rangeAttack.castingTime + 1) {
            //Start cooldown and chase
            beamEffect.SetActive(false);
            rangeAttack.attackTimer = rangeAttack.cooldown;
            StartChase();
        } else {
            rangeAttack.attackTimer += Time.deltaTime;
            if (target != null) {
                rangeTarget = Vector3.Lerp(rangeTarget, target.Position, Time.deltaTime);
                transform.LookAt(rangeTarget);
                transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
            }
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

    private void StartDeath() {
        navObst.enabled = false;
        navAgent.enabled = false;
        rb.isKinematic = false;
        warningEffect.SetActive(false);
        beamEffect.SetActive(false);
        GetComponent<CapsuleCollider>().enabled = true;
        base.Destroyed();
        agentState = STATES.Dead;
        Destroy(gameObject, 5);
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
    
    public override float TakeDmg(float dmg, DamageType damageType = DamageType.Standard, PlayerStats attacker = null) {
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

    }

    public void CheckForPlayers() {
        foreach(ClassAbilities p in Megamanager.MM.players) {
            if(Vector3.Distance(p.transform.position, transform.position) <= playerPerceptionRange) {
                StartBattlecry();
            }
        }
    }
}
