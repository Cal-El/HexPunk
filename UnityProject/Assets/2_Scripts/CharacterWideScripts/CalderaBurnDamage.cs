using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Character))]

public class CalderaBurnDamage : MonoBehaviour {

    private PlayerStats calderaStats;
    private bool isBurning = false;
    public float dotTimer = 1;
    private float timer;
    public int numberOfTicks = 3;
    private int currentTick;
    public float tickDamage = 1;
    private Character c;

    void Start () {
        c = GetComponent<Character>();
        var caldera = FindObjectOfType<CalderaAbilities>();
        if (caldera != null)
        {
            calderaStats = FindObjectOfType<CalderaAbilities>().gameObject.GetComponent<PlayerStats>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isBurning)
        {
            if (calderaStats == null)
                calderaStats = FindObjectOfType<CalderaAbilities>().gameObject.GetComponent<PlayerStats>();
            else
            {
                if (Time.time > timer && currentTick < numberOfTicks)
                {
                    if (calderaStats != null) c.TakeDmg(tickDamage, Character.DamageType.FireElectric, calderaStats);
                    else c.TakeDmg(tickDamage, Character.DamageType.FireElectric);
                    currentTick++;
                    if (currentTick >= numberOfTicks) isBurning = false;
                    timer += dotTimer;
                }
            }
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
