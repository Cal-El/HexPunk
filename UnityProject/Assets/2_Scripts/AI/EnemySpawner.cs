using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : DestructibleObject {

    public enum STATES { Idle, Activated, Dying};
    public STATES agentState = STATES.Idle;

    private Animation anim;
    public float perceptionRange = 10;

    [System.Serializable]
    public struct Spawnable {
        [Tooltip("Character object to be spawned.")]
        public GameObject spawn;
        [Tooltip("Difficulty of character being spawned. EG: Melee Grunt = 1")]
        public float severity;
    }

    [Tooltip("Ordered list from most severe to lease severe.")]
    public Spawnable[] spawnList;
    [Tooltip("Maximum number of enemies that can be spawned, based on their severity")]
    public float maxSeverity = 10;
    [Tooltip("Maximum number of enemies per spawn cycle.")]
    public float maxSeverityPerCycle = 5;
    [Tooltip("Time between spawn cycles.")]
    public float cycleTime = 6;

    private float totalSeverity = 0;
    private float timer = 0;
    private List<Spawnable> spawnedList;
    [SerializeField]
    private Renderer RedCentre;
    private float redness = 1;

	// Use this for initialization
	void Start () {
        Initialize();
        anim = GetComponent<Animation>();

        if (spawnList == null || spawnList.Length <= 0) {
            Debug.LogError("SpawnList is Empty");
        }
        
        spawnedList = new List<Spawnable>();

        redness = (health / maxHealth);
        RedCentre.material.SetColor("_EmissionColor", Color.Lerp(Color.black, Color.red, redness));

    }
	
	// Update is called once per frame
	void Update () {
        redness = Mathf.Lerp(redness, 1, Time.deltaTime);
        RedCentre.material.SetColor("_EmissionColor", Color.Lerp(Color.black, Color.red, redness));

        switch (agentState)
        {
            case STATES.Idle:
                IdleBehaviour();
                break;
            case STATES.Activated:
                ActivatedBehaviour();
                break;
            case STATES.Dying:
                DeathBehaviour();
                break;
        }
	}

    private void IdleBehaviour()
    {
        foreach(ClassAbilities p in Megamanager.MM.players)
        {
            if(Vector3.Distance(p.transform.position, transform.position) < perceptionRange)
            {
                Activate();
                break;
            }
        }
    }

    private void ActivatedBehaviour()
    {
        //Activate Spawn cycle, if able
        timer += Time.deltaTime;
        if (timer >= cycleTime)
        {
            timer = 0;

            //Clean up list
            for (int i = 0; i < spawnedList.Count; i++)
            {
                if (spawnedList[i].spawn == null)
                {
                    totalSeverity -= spawnedList[i].severity;
                    spawnedList.Remove(spawnedList[i]);
                }
            }

            //Determine what is being spawned
            float totalSevThisCycle = 0;
            List<Spawnable> newSpawns = new List<Spawnable>();
            for (int i = 0; i < spawnList.Length; i++)
            {
                if (totalSeverity >= maxSeverity)
                {
                    break;
                }
                while (spawnList[i].severity + totalSevThisCycle <= maxSeverityPerCycle && spawnList[i].severity + totalSeverity <= maxSeverity)
                {
                    newSpawns.Add(spawnList[i]);
                    totalSeverity += spawnList[i].severity;
                    totalSevThisCycle += spawnList[i].severity;
                }
            }

            //Spawn the new guys in a circle around the spawner
            for (int i = 0; i < newSpawns.Count; i++)
            {
                Spawnable newGuy;
                Vector3 offsetPostition = new Vector3(
                    transform.position.x + Mathf.Cos((i + 0.0f) / newSpawns.Count * Mathf.PI * 2) * 2,
                    transform.position.y,
                    transform.position.z + Mathf.Sin((i + 0.0f) / newSpawns.Count * Mathf.PI * 2) * 2
                    );
                newGuy.spawn = ServerSpawn(newSpawns[i].spawn, offsetPostition, transform.rotation);
                newGuy.severity = newSpawns[i].severity;
                try { newGuy.spawn.GetComponent<Character>().TakeDmg(0.01f); }
                catch { Debug.LogError("Spawning non-character Entity"); }
                spawnedList.Add(newGuy);
            }

        }
    }

    public override float TakeDmg(float dmg, DamageType damageType = DamageType.Standard, PlayerStats attacker = null) {
        redness -= (dmg*10) / maxHealth;
        redness = Mathf.Clamp(redness, 0.1f, 1);
        return base.TakeDmg(dmg, damageType, attacker);
    }

    private void DeathBehaviour()
    {
        Destroy(this.gameObject);
    }

    public void Activate()
    {
        anim.Play();
        agentState = STATES.Activated;
    }
}
