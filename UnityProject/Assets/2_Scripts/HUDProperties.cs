using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDProperties : MonoBehaviour {

    [System.Serializable]
    public class HUD
    {
        public Image healthBar;
        public Image energyBar;
        public Image xpBar;
        [Tooltip ("Shows a slight indication of the time it takes to cast the ability.")]
        public float[] abilityCds = new float[4];
    }

    public HUD hud;
    public Image[] abilityCooldownIcons = new Image[4];
    private float[] cooldownTimers = new float[4];
    
    void Start()
    {
        ShowIconCooldown(1);
    }
	
	// Update is called once per frame
	void Update () {
        if (hud == null) return;
	    for(int i = 0; i < abilityCooldownIcons.Length; i++)
        {
            if(abilityCooldownIcons[i].fillAmount > 0)
            {
                cooldownTimers[i] += Time.deltaTime;
                abilityCooldownIcons[i].fillAmount -= (cooldownTimers[i] / hud.abilityCds[i]) * Time.deltaTime;
            }
        }
	}

    public void ShowIconCooldown(int abilityNumber)
    {
        if (hud == null) return;
        abilityNumber -= 1;
        if (hud.abilityCds[abilityNumber] != 0)
        {
            cooldownTimers[abilityNumber] = 0;
            abilityCooldownIcons[abilityNumber].fillAmount = 1;
        }
    }
}
