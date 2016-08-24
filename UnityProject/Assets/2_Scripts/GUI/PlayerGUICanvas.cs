using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerGUICanvas : MonoBehaviour
{
    public HUDProperties myHud;

    private GameObject myPlayer;
    private ClassAbilities playerStats;

    public GameObject betrayerCanvas;
    private bool isBetrayer = false;
    private OtherPlayerGUI[] guiList = new OtherPlayerGUI[3];

    public GameObject victoryGUI;
    private bool victory = false;
    public GameObject defeatGUI;
    private bool defeat = false;

    [Header ("Don't change the order")]
    public GameObject[] huds = new GameObject[4];

    private float visHP;
    private float visEP;
    private float visXP;


    // Use this for initialization
    void Start()
    {

        myPlayer = transform.parent.GetComponent<PlayerCamera>().myPlayer;
        playerStats = myPlayer.GetComponent<ClassAbilities>();

        string name = myPlayer.gameObject.name;
        
        SetHUDs(name, "Conduit");
        SetHUDs(name, "Aethersmith");
        SetHUDs(name, "Caldera");
        SetHUDs(name, "Shard");
        
        visHP = playerStats.health;
        visEP = playerStats.energy;

        guiList = new[] { betrayerCanvas.transform.FindChild("OtherPlayerGUI1").GetComponent<OtherPlayerGUI>(),
                    betrayerCanvas.transform.FindChild("OtherPlayerGUI2").GetComponent<OtherPlayerGUI>(),
                    betrayerCanvas.transform.FindChild("OtherPlayerGUI3").GetComponent<OtherPlayerGUI>() };
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

    // Update is called once per frame
    void Update()
    {
        if (myHud == null) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            isBetrayer = !isBetrayer;
        }

        visHP = Mathf.Lerp(visHP, playerStats.health, Time.deltaTime * 10);
        visEP = Mathf.Lerp(visEP, playerStats.energy, Time.deltaTime * 10);
        visXP = Mathf.Lerp(visXP, playerStats.GetLevel(), Time.deltaTime * 10);

        myHud.healthBar.fillAmount = visHP / 100;
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
            SetBetrayerGUI();
            betrayerCanvas.SetActive(value);
            myHud.icon.sprite = myHud.betrayerIcon;
            isBetrayer = value;
        }
    }

    private void SetBetrayerGUI()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 1)
        {
            var x = 0;
            foreach (var player in players)
            {
                if (player != myPlayer)
                {
                    if (guiList[x].player == null)
                        guiList[x].player = player;
                    if (x < players.Length - 1) x++;
                }
            }
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
            victoryGUI.SetActive(value);
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
            defeatGUI.SetActive(value);
            defeat = value;
        }
    }
}
