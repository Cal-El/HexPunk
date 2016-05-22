using UnityEngine;
using System.Collections;

public class ConduitAnimation : MonoBehaviour {

    ConduitAbilities ca;
    ConduitAbilities.ANIMATIONSTATES previousState;
    Animator ani;

	// Use this for initialization
	void Start () {
        ca = GetComponentInParent<ConduitAbilities>();
        ani = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if(previousState != ca.currentState)
        {
            switch (ca.currentState)
            {
                case ClassAbilities.ANIMATIONSTATES.Idle:
                    ani.Play("Armature|Idle");
                    break;
                case ClassAbilities.ANIMATIONSTATES.Running:
                    ani.Play("Armature|Run Cycle");
                    break;
                case ClassAbilities.ANIMATIONSTATES.Ability1:
                    ani.Play("Armature|Punch 1");
                    break;
                case ClassAbilities.ANIMATIONSTATES.Ability2:
                    ani.Play("Armature|Stomp");
                    break;
                case ClassAbilities.ANIMATIONSTATES.Ability3:
                    ani.Play("Armature|DrawPower");
                    break;
                case ClassAbilities.ANIMATIONSTATES.Ability4:
                    ani.Play("Armature|Punch 2");
                    break;
            }
        }
        
        previousState = ca.currentState;
    }
}
