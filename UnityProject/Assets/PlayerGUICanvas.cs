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

    private bool isBetrayer = false;
    private float health = 100;
    private float energy = 100;

    // Use this for initialization
    void Start () {
        healthBar.color = Color.red;
        healthBarStub.color = Color.red;
        energyBar.color = Color.blue;
        energyBarStub.color = Color.blue;
        if (isBetrayer) {
            icon.color = Color.red;
        } else {
            icon.color = Color.blue;
        }
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.P)) {
            isBetrayer = !isBetrayer;
        }
        if (Input.GetKey(KeyCode.O)) {
            health = Mathf.Min(health + Time.deltaTime*10, 100);
        }
        if (Input.GetKey(KeyCode.L)) {
            health = Mathf.Max(health - Time.deltaTime * 10, 0);
        }
        if (Input.GetKey(KeyCode.I)) {
            energy = Mathf.Min(energy + Time.deltaTime * 10, 100);
        }
        if (Input.GetKey(KeyCode.K)) {
            energy = Mathf.Max(energy - Time.deltaTime * 10, 0);
        }

        healthBar.rectTransform.localScale = new Vector3(health/100, healthBar.rectTransform.localScale.y, healthBar.rectTransform.localScale.z);
        healthBarStub.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(healthBarRange.x, healthBarRange.y, health/100), healthBarStub.rectTransform.anchoredPosition.y);
        energyBar.rectTransform.localScale = new Vector3(energy / 100, energyBar.rectTransform.localScale.y, energyBar.rectTransform.localScale.z);
        energyBarStub.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(energyBarRange.x, energyBarRange.y, energy / 100), energyBarStub.rectTransform.anchoredPosition.y);


        if (isBetrayer) {
            icon.color = Color.red;
        } else {
            icon.color = Color.blue;
        }


    }
}
