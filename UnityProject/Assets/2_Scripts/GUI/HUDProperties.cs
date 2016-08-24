using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDProperties : MonoBehaviour {

    public Image icon;
    public Sprite standardIcon;
    public Sprite betrayerIcon;
    public Image healthBar;
    public Image energyBar;
    public Image xpBar;
    [HideInInspector]
    public float[] abilityCds = new float[4];
    public Image[] abilityCooldownIcons = new Image[4];
    public Image[] notEnoughEnergyIcons = new Image[4];
	
	// Update is called once per frame
	void Update () {
	    for(int i = 0; i < abilityCooldownIcons.Length; i++)
        {
            if(abilityCooldownIcons[i].fillAmount > 0)
            {
                abilityCooldownIcons[i].fillAmount -= (Time.deltaTime / abilityCds[i]);
            }
        }
	}

    public void ShowIconCooldown(int abilityNumber, float abilityCooldown)
    {
        abilityNumber -= 1;
        abilityCds[abilityNumber] = abilityCooldown;
        if (abilityCds[abilityNumber] != 0)
        {
            abilityCooldownIcons[abilityNumber].fillAmount = 1;
        }
    }
}
