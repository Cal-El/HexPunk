using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OtherPlayerGUI : MonoBehaviour
{
    public GameObject player;

    public Image icon;
    public Image healthBar;
    public Image healthBarStub;
    public Vector2 healthBarRange; //x value is minimum bar size, y is max

    private ClassAbilities playerStats;

    private float visHP;

    // Use this for initialization
    void Start()
    {
        healthBar.color = Color.red;
        healthBarStub.color = Color.red;

        if (player == null) return;
        playerStats = player.GetComponent<ClassAbilities>();
        visHP = playerStats.health;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        visHP = Mathf.Lerp(visHP, playerStats.health, Time.deltaTime * 10);

        healthBar.rectTransform.localScale = new Vector3(visHP / 100, healthBar.rectTransform.localScale.y, healthBar.rectTransform.localScale.z);
        healthBarStub.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(healthBarRange.x, healthBarRange.y, visHP / 100), healthBarStub.rectTransform.anchoredPosition.y);
    }
}
