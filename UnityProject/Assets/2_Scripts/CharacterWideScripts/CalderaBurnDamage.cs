using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Character))]

public class CalderaBurnDamage : MonoBehaviour {
    
    private bool isBurning = false;
    public float dotTimer = 1;
    private float timer;
    public int numberOfTicks = 3;
    private int currentTick;
    public float tickDamage = 1;
    private Character c;

    void Start () {
        c = GetComponent<Character>();
    }

	// Update is called once per frame
	void Update ()
    {
	    if(Time.time > timer && currentTick < numberOfTicks)
        {
            c.TakeDmg(tickDamage, Character.DamageType.FireElectric);
            currentTick++;
            if (currentTick >= numberOfTicks) isBurning = false;
            timer += dotTimer;
        }
	}

    public bool IsBurning
    {
        get
        {
            return isBurning;
        }

        set
        {
            if (value)
            {
                timer = Time.time;
                currentTick = 0;
            }
            else
            {
                currentTick = numberOfTicks;
            }
            isBurning = value;
        }
    }
}
