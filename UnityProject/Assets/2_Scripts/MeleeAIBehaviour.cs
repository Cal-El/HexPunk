using UnityEngine;
using System.Collections;

public class MeleeAIBehaviour : MonoBehaviour {

    private const float TIME_FOR_NAVMESH_UPDATE = 0.5f;

    //State machine variables
    enum STATES { Idle, Battlecry, Chasing, Attacking}
    STATES agentState = STATES.Idle;

    //Health Values
    private float health;
    public float maxHealth = 5;

    //Attack Statistics
    public float baseDmg = 5;                   //Base damage of the melee attack
    public float castingTime = 1;               //The time it takes from starting an attack to it actually connecting with a player
    public float cooldown = 1;                  //The time it between attacks
    private float attackTimer = 0;
    public float range = 2;                     //Range/Reach of the attack

    //Battlecry Variables
    public char[] battleTriggers;           //The char triggers that can override this AI to begin attacking
    public float battlecryTime = 1.5f;      //Time it takes to complete a battlecry
    private float battlecryTimer = 0.0f;    //Timer for use while battlecry is triggering
    public float battlecryRange = 2.0f;     //Range in which the battlecry triggers others around it

    //Pathfinding Variables
    private GameObject target;
    private NavMeshAgent navAgent;
    private NavMeshObstacle navObst;
    private float inactiveTimer = 0.0f;

    void Start () {
        navAgent = this.GetComponent<NavMeshAgent>();
        navObst = this.GetComponent<NavMeshObstacle>();
        health = maxHealth;
	}
	
	void Update () {
        if (inactiveTimer <= 0) {
            if (transform.parent.GetComponent<Room>().roomUnlocked) {
                switch (agentState) {
                    case STATES.Idle:
                        GetComponent<Renderer>().material.color = Color.white;
                        IdleBehaviour();
                        break;
                    case STATES.Battlecry:
                        GetComponent<Renderer>().material.color = Color.yellow;
                        BattlecryBehaviour();
                        break;
                    case STATES.Chasing:
                        GetComponent<Renderer>().material.color = Color.blue;
                        ChasingBehaviour();
                        break;
                    case STATES.Attacking:
                        GetComponent<Renderer>().material.color = Color.red;
                        AttackingBehaviour();
                        break;
                }
            }
        } else {
            inactiveTimer -= Time.deltaTime;
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
            GameObject[] pms = GameObject.FindGameObjectsWithTag("Player");
            GameObject closest = null;
            foreach(GameObject pm in pms) {
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
            attackTimer -= Time.deltaTime;
            if (attackTimer <= cooldown) {      //Theshold crossed. Time to attack
                RaycastHit hit;
                if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, range)) {
                    if(hit.transform.tag == "Player") {
                        hit.transform.SendMessage("TakeDmg", baseDmg, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        } else {                                //Not Attacking
            transform.LookAt(new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.z));
            attackTimer -= Time.deltaTime;
            if (Vector3.Distance(this.transform.position, target.transform.position) > range) {
                StartChase();
                return;
            }
            if(attackTimer <= 0) {              //Ready to attack again
                attackTimer = cooldown + castingTime;
            }
        }
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

    public void TakeDmg(float dmg) {
        health -= dmg;
        if (agentState == STATES.Idle)
            StartBattlecry();
        if (health <= 0) {
            GameObject.Destroy(this.gameObject);
        }
    }
}
