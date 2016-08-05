using UnityEngine;
using System.Collections;

public class MeleeAIBehaviour : Character {

    private const float TIME_FOR_NAVMESH_UPDATE = 0.5f;

    //State machine variables
    public enum STATES { Idle, Battlecry, Chasing, Attacking, Dead}
    [HideInInspector]public STATES agentState = STATES.Idle;
    [HideInInspector]public STATES animationState = STATES.Idle;

    public SkinnedMeshRenderer mr;

    //Health Values
    private float health;
    public float maxHealth = 5;

    //Attack Statistics
    public float baseDmg = 5;                   //Base damage of the melee attack
    public float castingTime = 1;               //The time it takes from starting an attack to it actually connecting with a player
    public float cooldown = 1;                  //The time it between attacks
    private float attackTimer = 0;
    public float range = 2;                     //Range/Reach of the attack

    //Battlecry Variable
    public char[] battleTriggers;           //The char triggers that can override this AI to begin attacking
    public float battlecryTime = 1.5f;      //Time it takes to complete a battlecry
    private float battlecryTimer = 0.0f;    //Timer for use while battlecry is triggering
    public float battlecryRange = 2.0f;     //Range in which the battlecry triggers others around it

    //Pathfinding Variables
    private ClassAbilities target;
    [HideInInspector]public NavMeshAgent navAgent;
    private NavMeshObstacle navObst;
    private float inactiveTimer = 0.0f;

    void Awake () {
        navAgent = this.GetComponent<NavMeshAgent>();
        navObst = this.GetComponent<NavMeshObstacle>();

        navAgent.speed = navAgent.speed * Random.Range(0.5f, 1.0f);
        navAgent.avoidancePriority = Random.Range(1,99);
        health = maxHealth;
	}
	
	void Update () {
        if (agentState != STATES.Dead) {
            Vector3 prePos = transform.position;
            if (inactiveTimer <= 0) {
                if (transform.parent == null || transform.parent.GetComponent<Room>().roomUnlocked) {
                    switch (agentState) {
                        case STATES.Idle:
                            mr.material.SetColor("_EmissionColor", Color.green);
                            animationState = STATES.Idle;
                            IdleBehaviour();
                            break;
                        case STATES.Battlecry:
                            mr.material.SetColor("_EmissionColor", Color.yellow);
                            animationState = STATES.Battlecry;
                            BattlecryBehaviour();
                            break;
                        case STATES.Chasing:
                            mr.material.SetColor("_EmissionColor", Color.red);
                            Retagetting();
                            animationState = STATES.Chasing;
                            ChasingBehaviour();
                            break;
                        case STATES.Attacking:
                            mr.material.SetColor("_EmissionColor", Color.white);
                            AttackingBehaviour();
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
    }

    private void BattlecryBehaviour() {
        battlecryTimer -= Time.deltaTime;
        if(battlecryTimer <= 0) {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, battlecryRange, transform.forward, 0);
            foreach(RaycastHit hit in hits) {
                if (hit.transform.GetComponent<MeleeAIBehaviour>() != null) {
                    hit.transform.GetComponent<MeleeAIBehaviour>().HearBattlecry();
                }
            }
            ClassAbilities[] pms = GameObject.FindObjectsOfType<ClassAbilities>();
            ClassAbilities closest = null;
            foreach(ClassAbilities pm in pms) {
                if(closest == null) {
                    closest = pm;
                }
                if(Vector3.Distance(this.transform.position, pm.transform.position) < Vector3.Distance(this.transform.position, closest.transform.position)) {
                    closest = pm;
                }
            }
            target = closest;
            StartChase();
        }
        
    }

    private void ChasingBehaviour() {
        if (navAgent != null && target != null) {
            navAgent.enabled = true;
            navObst.enabled = false;
            navAgent.destination = target.transform.position + (this.transform.position-target.transform.position).normalized*range/2;
        }
        
        
        if (Vector3.Distance(this.transform.position, target.transform.position) < range - 0.5f) {
            navAgent.enabled = false;
            navObst.enabled = true;
            agentState = STATES.Attacking;
        }
    }

    private void AttackingBehaviour() {
        if(attackTimer > cooldown) {            //Attacking
            animationState = STATES.Attacking;
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
            if (!target.IsAlive) {
                Retagetting();
            }
            if(attackTimer <= 0) {              //Ready to attack again
                attackTimer = cooldown + castingTime;
            }
        }
    }

    private void DeadBehaviour() {
        
    }

    private void StartBattlecry() {
        battlecryTimer = battlecryTime;
        agentState = STATES.Battlecry;
    }

    private void StartChase() {
        inactiveTimer = TIME_FOR_NAVMESH_UPDATE;
        navObst.enabled = false;
        agentState = STATES.Chasing;
    }

    private void StartDeath() {
        navObst.enabled = false;
        navAgent.enabled = false;
        GetComponent<Collider>().enabled = false;
        agentState = STATES.Dead;
        Destroy(gameObject, 5);
    }

    public void ReceiveMessage(char a) {
        if (agentState == STATES.Idle) {
            foreach (char c in battleTriggers) {
                if (c == a) {
                    StartBattlecry();
                }
            }
        }
    }

    public void HearBattlecry() {
        if (agentState == STATES.Idle) {
            StartBattlecry();
        }
    }

    public override float GetHealth() {
        return health;
    }

    public override void Heal(float healVal) {
        health = Mathf.Clamp(health + healVal, 0, maxHealth);
    }

    public override void TakeDmg(float dmg) {
        health = Mathf.Clamp(health - dmg, 0, maxHealth);
        if (agentState == STATES.Idle)
            StartBattlecry();
        if (health <= 0) {
            StartDeath();
        }
    }

    public override void Knockback(Vector3 force, float timer) {

    }

    public void Retagetting()
    {
        float threshold = Vector3.Distance(this.transform.position, target.transform.position) * 0.8f;
        if (target.currentState == ClassAbilities.ANIMATIONSTATES.Dead) threshold = 1000;
        foreach (ClassAbilities p in Megamanager.MM.players)
        {
            if (Vector3.Distance(this.transform.position, p.transform.position) <= threshold && p.currentState != ClassAbilities.ANIMATIONSTATES.Dead){
                target = p;
            }
        }
    }
}
