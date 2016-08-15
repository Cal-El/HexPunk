using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : DestructibleObject {

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

	// Use this for initialization
	void Start () {
        Initialise();

        if (spawnList == null || spawnList.Length <= 0) {
            Debug.LogError("SpawnList is Empty");
        }
        
        spawnedList = new List<Spawnable>();

	}
	
	// Update is called once per frame
	void Update () {
        //Activate Spawn cycle, if able
        timer += Time.deltaTime;
        if(timer >= cycleTime) {
            timer = 0;

            //Clean up list
            for (int i = 0; i < spawnedList.Count; i++) {
                if (spawnedList[i].spawn == null) {
                    totalSeverity -= spawnedList[i].severity;
                    spawnedList.Remove(spawnedList[i]);
                }
            }

            //Determine what is being spawned
            float totalSevThisCycle = 0;
            List<Spawnable> newSpawns = new List<Spawnable>();
            for (int i = 0; i < spawnList.Length; i++) {
                if (totalSeverity >= maxSeverity) {
                    break;
                }
                while (spawnList[i].severity + totalSevThisCycle <= maxSeverityPerCycle && spawnList[i].severity + totalSeverity <= maxSeverity) {
                    newSpawns.Add(spawnList[i]);
                    Debug.Log("Determining Spawns.");
                    totalSeverity += spawnList[i].severity;
                    Debug.Log("Determining Spawns..");
                    totalSevThisCycle += spawnList[i].severity;
                    Debug.Log("Determining Spawns...");
                }
            }
            Debug.Log("Spawning Units...");

            //Spawn the new guys in a circle around the spawner
            for(int i = 0; i < newSpawns.Count; i++) {
                Spawnable newGuy;
                Vector3 offsetPostition = new Vector3(
                    transform.position.x + Mathf.Cos((i + 0.0f) / newSpawns.Count * Mathf.PI * 2) * 2,
                    transform.position.y,
                    transform.position.z + Mathf.Sin((i + 0.0f) / newSpawns.Count * Mathf.PI * 2) * 2
                    );
                newGuy.spawn = Instantiate(newSpawns[i].spawn, offsetPostition, transform.rotation) as GameObject;
                newGuy.severity = newSpawns[i].severity;
                try { newGuy.spawn.GetComponent<Character>().TakeDmg(0.01f); }
                catch { Debug.LogError("Spawning non-character Entity"); }
                spawnedList.Add(newGuy);
            }
            Debug.Log("Spawning Complete");

        }
	}
}
