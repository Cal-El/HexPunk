using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerGUICanvas : MonoBehaviour
{
    public HUDProperties myHud;

    [HideInInspector]
    public GameObject myPlayer;
    private ClassAbilities playerStats;
    private float startingMaxHealth;

    public GameObject betrayerCanvas;
    private bool isBetrayer = false;

    public GameObject endGameGUI;
    private bool victory = false;
    private bool defeat = false;
    public ScreenFader fader;
    public GameObject helpGUI;

    public Image bloodEdge;
    private float intensity;

    public bool levelUpNow;
    public Text levelUp;
    private float levelUpTimer;
    private Vector2 levelUpStartPos;
    
    public GameObject[] huds = new GameObject[4];

    private float visHP;
    private float visMaxHp;
    private float visEP;
    private float visXP;


    // Use this for initialization
    void Start()
    {
        myPlayer = transform.parent.GetComponent<PlayerCamera>().myPlayer;
        playerStats = myPlayer.GetComponent<ClassAbilities>();
        startingMaxHealth = playerStats.healthMax;
        string playerName = myPlayer.gameObject.name;
        levelUpStartPos = levelUp.rectTransform.anchoredPosition;


        SetHUDs(playerName, "Conduit");
        SetHUDs(playerName, "Aethersmith");
        SetHUDs(playerName, "Caldera");
        SetHUDs(playerName, "Shard");
        
        visHP = playerStats.health;
        visMaxHp = playerStats.healthMax;
        visEP = playerStats.energy;
    }

    private void SetHUDs(string playerName, string className)
    {
        if (playerName.Contains(className))
        {
            foreach(var hud in huds)
            {
                if (!hud.gameObject.name.Contains(className))
                {
                    Destroy(hud);
                }
                else
                {
                    myHud = hud.GetComponent<HUDProperties>();
                }
            }
        }
    }

    public void ShowHelp(bool value)
    {
        helpGUI.SetActive(value);
    }

    // Update is called once per frame
    void Update()
    {
        if (myHud == null) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            isBetrayer = !isBetrayer;
        }
        float preVisHP = visHP;

        visHP = Mathf.Lerp(visHP, playerStats.health, Time.deltaTime * 10);
        visMaxHp = Mathf.Lerp(visMaxHp, playerStats.healthMax, Time.deltaTime * 10);
        visEP = Mathf.Lerp(visEP, playerStats.energy, Time.deltaTime * 10);
        visXP = Mathf.Lerp(visXP, playerStats.GetLevel(), Time.deltaTime * 10);

        myHud.healthBar.fillAmount = visHP / 100;

        if (startingMaxHealth != 0)
        {
            var currenMaxHpPerc = 1 - visMaxHp / startingMaxHealth;
            var healthbarDistance = myHud.healthBarEnd - myHud.healthBarStart;
            var healthBarPos = myHud.healthBarStart + healthbarDistance * currenMaxHpPerc;
            myHud.SetHealthBarPos(healthBarPos);
        }

        myHud.energyBar.fillAmount = visEP / playerStats.energyMax;

        var xpBar = myHud.xpBar;

        if (visXP < 1) {
            xpBar.fillAmount = 0.175f * visXP;
        } else if(visXP < 2) {
            xpBar.fillAmount = 0.175f  + 0.15f * (visXP - 1);
        } else if (visXP < 3) {
            xpBar.fillAmount = 0.325f + 0.175f * (visXP - 2);
        } else if (visXP < 4) {
            xpBar.fillAmount = 0.5f + 0.175f * (visXP - 3);
        } else if (visXP < 5) {
            xpBar.fillAmount = 0.675f + 0.15f * (visXP - 4);
        } else {
            xpBar.fillAmount = 0.825f + 0.175f * (visXP - 5);
        }

        if (levelUpNow)
        {
            levelUp.rectTransform.anchoredPosition = Vector2.Lerp(levelUp.rectTransform.anchoredPosition, new Vector2(levelUpStartPos.x, levelUpStartPos.y + 120), Time.deltaTime * 2);            
        }
        else
        {
            levelUp.rectTransform.anchoredPosition = Vector2.Lerp(levelUp.rectTransform.anchoredPosition, new Vector2(levelUpStartPos.x, levelUpStartPos.y), Time.deltaTime * 2);
        }

        if(levelUpNow && Time.time > levelUpTimer)
        {
            levelUpNow = false;
        }

        levelUp.color = new Color(levelUp.color.r, levelUp.color.g, levelUp.color.b, Mathf.Pow(((levelUp.rectTransform.anchoredPosition.y-levelUpStartPos.y)/120), 2));

        if (playerStats.IsAlive) {
            intensity *= 0.99f;
            intensity += (preVisHP - visHP) / visHP;

            bloodEdge.color = Color.white - (Color.black * (1 - intensity));
        } else {
            bloodEdge.color = Color.Lerp(bloodEdge.color, Color.white, Time.deltaTime * 10);
        }

    }

    public void LevelUp()
    {
        levelUpNow = true;
        levelUpTimer = Time.time + 2;
    }

    //Set up the betrayer GUI
    public bool IsBetrayer
    {
        get
        {
            return isBetrayer;
        }

        set
        {
            if (value)
            {
                var betrayGUI = Instantiate(betrayerCanvas) as GameObject;
                betrayGUI.transform.SetParent(transform);
                betrayGUI.transform.SetSiblingIndex(myHud.transform.GetSiblingIndex() + 1);
                betrayGUI.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                betrayGUI.transform.rotation = new Quaternion();
                betrayGUI.transform.localScale = Vector3.one;
                var script = betrayGUI.GetComponent<BetrayerGUI>();
                script.myPlayer = myPlayer;
                script.SetBetrayerGUI();
                myHud.icon.sprite = myHud.betrayerIcon;
            }
            isBetrayer = value;
        }
    }    

    public bool Victory
    {
        get
        {
            return victory;
        }

        set
        {
            endGameGUI.SetActive(value);
            endGameGUI.GetComponent<EndGameGUI>().Victory();
            victory = value;
        }
    }

    public bool Defeat
    {
        get
        {
            return defeat;
        }

        set
        {
            endGameGUI.SetActive(value);
            endGameGUI.GetComponent<EndGameGUI>().Defeat();
            defeat = value;
        }
    }
}
