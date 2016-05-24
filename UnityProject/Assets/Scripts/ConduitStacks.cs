using UnityEngine;
using System.Collections;

public class ConduitStacks : MonoBehaviour {

    public int Stacks = 0;
    
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
