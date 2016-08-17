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
        [HideInInspector]
        public float[] abilityCds = new float[4];
    }

    public HUD hud;
    public Image[] abilityCooldownIcons = new Image[4];
	
	// Update is called once per frame
	void Update () {
        if (hud == null) return;
	    for(int i = 0; i < abilityCooldownIcons.Length; i++)
        {
            if(abilityCooldownIcons[i].fillAmount > 0)
            {
                abilityCooldownIcons[i].fillAmount -= (Time.deltaTime / hud.abilityCds[i]);
            }
        }
	}

    public void ShowIconCooldown(int abilityNumber, float abilityCooldown)
    {
        if (hud == null) return;
        abilityNumber -= 1;
        hud.abilityCds[abilityNumber] = abilityCooldown;
        if (hud.abilityCds[abilityNumber] != 0)
        {
            abilityCooldownIcons[abilityNumber].fillAmount = 1;
        }
    }
}
