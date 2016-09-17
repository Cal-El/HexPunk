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
    public GameObject effectPrefab;
    private ParticleSystem ps;

    void Start () {

        c = GetComponent<Character>();
        var caldera = FindObjectOfType<CalderaAbilities>();
        if (caldera != null)
        {
            calderaStats = FindObjectOfType<CalderaAbilities>().gameObject.GetComponent<PlayerStats>();
        }

        GameObject g = Instantiate(effectPrefab, transform.position, effectPrefab.transform.rotation) as GameObject;
        g.transform.position = new Vector3(g.transform.position.x, 1, g.transform.position.z);
        g.transform.parent = this.transform;
        ps = g.GetComponent<ParticleSystem>();
        ps.enableEmission = false;
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

        if (isBurning) {
            ps.enableEmission = true;
        } else {
            ps.enableEmission = false;
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
