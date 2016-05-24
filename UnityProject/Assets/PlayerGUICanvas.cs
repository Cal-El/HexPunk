using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerGUICanvas : MonoBehaviour {

    public Image icon;
    public Image healthBar;
    public Image healthBarStub;
    public Image energyBar;
    public Image energyBarStub;
    public Vector2 healthBarRange; //x value is minimum bar size, y is max
    public Vector2 energyBarRange; //x value is minimum bar size, y is max

    private ClassAbilities playerStats;
    private bool isBetrayer = false;
    private float visHP;
    private float visEP;

    // Use this for initialization
    void Start () {
        playerStats = transform.parent.GetComponent<PlayerCamera>().myPlayer.GetComponent<ClassAbilities>();

        healthBar.color = Color.red;
        healthBarStub.color = Color.red;
        energyBar.color = Color.blue;
        energyBarStub.color = Color.blue;
        if (isBetrayer) {
            icon.color = Color.red;
        } else {
            icon.color = Color.blue;
        }
        visHP = playerStats.Health;
        visEP = playerStats.Energy;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.P)) {
            isBetrayer = !isBetrayer;
        }

        visHP = Mathf.Lerp(visHP, playerStats.Health, Time.deltaTime * 10);
        visEP = Mathf.Lerp(visEP, playerStats.Energy, Time.deltaTime * 10);

        healthBar.rectTransform.localScale = new Vector3(visHP / 100, healthBar.rectTransform.localScale.y, healthBar.rectTransform.localScale.z);
        healthBarStub.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(healthBarRange.x, healthBarRange.y, visHP / 100), healthBarStub.rectTransform.anchoredPosition.y);
        energyBar.rectTransform.localScale = new Vector3(visEP / playerStats.energyMax, energyBar.rectTransform.localScale.y, energyBar.rectTransform.localScale.z);
        energyBarStub.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(energyBarRange.x, energyBarRange.y, visEP / playerStats.energyMax), energyBarStub.rectTransform.anchoredPosition.y);


        if (isBetrayer) {
            icon.color = Color.red;
        } else {
            icon.color = Color.blue;
        }


    }
}
