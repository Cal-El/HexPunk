using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDProperties : MonoBehaviour {

    public Image icon;
    public Sprite standardIcon;
    public Sprite betrayerIcon;
    public Image maxHealthBar;
    public Image healthBar;
    public Image healthBarEdge;
    public float healthBarStart;
    public float healthBarEnd;
    public Image energyBar;
    public Image xpBar;
	public Image xpEffect;
    private float[] abilityCds = new float[4];
    public Image[] abilityCooldownIcons = new Image[4];
    public Image[] notEnoughEnergyIcons = new Image[4];
	
	// Update is called once per frame
	void Update () {
		xpEffect.rectTransform.Rotate (0, 0, Time.deltaTime * 60);

        for(int i = 0; i < abilityCooldownIcons.Length; i++)
        {
            if(abilityCooldownIcons[i].fillAmount > 0)
            {
                abilityCooldownIcons[i].fillAmount -= (Time.deltaTime / abilityCds[i]);
            }
        }
	}

    public void SetHealthBarPos(float healthBarPosX)
    {
        Vector3 newPos = new Vector2(healthBarPosX, healthBar.rectTransform.anchoredPosition.y);
        maxHealthBar.rectTransform.anchoredPosition = newPos;
        healthBar.rectTransform.anchoredPosition = newPos;
        healthBarEdge.rectTransform.anchoredPosition = newPos;
    }

    public void ShowIconCooldown(int abilityNumber, float abilityCooldown)
    {
        if (abilityCooldown != 0 && abilityNumber < 5)
        {
            abilityNumber -= 1;
            abilityCds[abilityNumber] = abilityCooldown;

            abilityCooldownIcons[abilityNumber].fillAmount = 1;
        }
    }

    public void ShowIconNotEnoughEnergy(int abilityNumber, float abilityEnergyCost, float currentEnergy)
    {
        if (abilityEnergyCost != 0 && abilityNumber < 5)
        {
            abilityNumber -= 1;

            if (currentEnergy >= abilityEnergyCost && notEnoughEnergyIcons[abilityNumber].enabled)
            {
                notEnoughEnergyIcons[abilityNumber].enabled = false;
            }
            else if (currentEnergy < abilityEnergyCost && !notEnoughEnergyIcons[abilityNumber].enabled)
            {
                notEnoughEnergyIcons[abilityNumber].enabled = true;
            }
        }
    }

    public void ShowCanCastAbility(int abilityNumber, bool value)
    {
        if (abilityNumber < 5)
        {
            abilityNumber -= 1;
            if (notEnoughEnergyIcons[abilityNumber].enabled != value)
                notEnoughEnergyIcons[abilityNumber].enabled = value;
        }
    }
}
