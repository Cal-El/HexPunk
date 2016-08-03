using UnityEngine;
using System.Collections;

public class CalderaBurnDamage : MonoBehaviour {
    
    private bool isBurning = false;
    public float dotTimer = 1;
    private float timer;
    public int numberOfTicks = 3;
    private int currentTick;
    public float tickDamage = 1;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(Time.time > timer && currentTick < numberOfTicks)
        {
            gameObject.SendMessage("TakeDmg", tickDamage);
            currentTick++;
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
