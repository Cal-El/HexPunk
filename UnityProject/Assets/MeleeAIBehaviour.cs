using UnityEngine;
using System.Collections;

public class MeleeAIBehaviour : MonoBehaviour {

    enum STATES { Idle, Battlecry, Chasing, Attacking}
    STATES agentState = STATES.Idle;
    public char[] battleTriggers;
    public float battlecryTime = 1.5f;
    private float battlecryTimer = 0.0f;
    public float battlecryRange = 2.0f;
    private GameObject target;
    private NavMeshAgent navAgent;

	// Use this for initialization
	void Start () {
        navAgent = this.GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        //if (transform.parent.GetComponent<Room>().RoomActive) {
            switch (agentState) {
                case STATES.Idle:
                    IdleBehaviour();
                    break;
                case STATES.Battlecry:
                    BattlecryBehaviour();
                    break;
                case STATES.Chasing:
                    ChasingBehaviour();
                    break;
                case STATES.Attacking:
                    AttackingBehaviour();
                    break;
            }
        //}
	}

    private void IdleBehaviour() {

    }

    private void BattlecryBehaviour() {
        battlecryTimer -= Time.deltaTime;
        if(battlecryTimer <= 0) {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, battlecryRange, transform.forward, 0);
            foreach(RaycastHit hit in hits) {
                if (GetComponent<MeleeAIBehaviour>() != null) {
                    GetComponent<MeleeAIBehaviour>().HearBattlecry();
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
            agentState = STATES.Chasing;
        }
    }

    private void ChasingBehaviour() {
        if(navAgent != null && target != null) {
            navAgent.destination = target.transform.position + (this.transform.position-target.transform.position).normalized*2;
        }
        if(Vector3.Distance(this.transform.position, target.transform.position) < 1f) {
            navAgent.Stop();
            agentState = STATES.Attacking;
        }
        //If within melee range, go to attacking behaviour
    }

    private void AttackingBehaviour() {
        if (Vector3.Distance(this.transform.position, target.transform.position) > 1f) {
            navAgent.Resume();
            agentState = STATES.Chasing;
        }
    }

    private void StartBattlecry() {
        battlecryTimer = battlecryTime;
        agentState = STATES.Battlecry;
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
}
