using UnityEngine;
using System.Collections;

public class ConduitStacks : MonoBehaviour {

    public int Stacks = 0;
    public GameObject stacksEffectPrefab;
    private GameObject stacksEffect;
    
    // Use this for initialization
	void Start () {
        stacksEffect = Instantiate(stacksEffectPrefab, new Vector3(transform.position.x, 1.5f, transform.position.z), stacksEffectPrefab.transform.rotation) as GameObject;
        stacksEffect.transform.parent = transform;
        stacksEffect.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {
        if (Stacks > 0) {
            stacksEffect.SetActive(true);
        } else {
            stacksEffect.SetActive(false);
        }
	}

    public void AddStack () {
        Stacks = Mathf.Clamp(Stacks+1, 0, 10);
    }

    public void AddStack (int i) {
        Stacks = Mathf.Clamp(Stacks + i, 0, 10);
    }

    public void Discharge()
    {
        Stacks = 0;
    }

    public int GetStacks() {
        return Stacks;
    }
}
