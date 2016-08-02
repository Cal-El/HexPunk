using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerGUICanvas : MonoBehaviour
{

    public Image icon;
    public Image healthBar;
    public Image energyBar;
    public Image xpBar;

    private GameObject myPlayer;
    private ClassAbilities playerStats;

    public GameObject betrayerCanvas;
    private bool isBetrayer = false;
    private OtherPlayerGUI[] guiList;

    public GameObject victoryGUI;
    private bool victory = false;
    public GameObject defeatGUI;
    private bool defeat = false;

    private float visHP;
    private float visEP;
    private float visXP;


    // Use this for initialization
    void Start()
    {

        myPlayer = transform.parent.GetComponent<PlayerCamera>().myPlayer;
        playerStats = myPlayer.GetComponent<ClassAbilities>();

        if (isBetrayer)
        {
            icon.color = Color.red;
        }
        else
        {
            icon.color = Color.blue;
        }
        visHP = playerStats.health;
        visEP = playerStats.energy;

        guiList = new[] { betrayerCanvas.transform.FindChild("OtherPlayerGUI1").GetComponent<OtherPlayerGUI>(),
                    betrayerCanvas.transform.FindChild("OtherPlayerGUI2").GetComponent<OtherPlayerGUI>(),
                    betrayerCanvas.transform.FindChild("OtherPlayerGUI3").GetComponent<OtherPlayerGUI>() };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isBetrayer = !isBetrayer;
        }

        visHP = Mathf.Lerp(visHP, playerStats.health, Time.deltaTime * 10);
        visEP = Mathf.Lerp(visEP, playerStats.energy, Time.deltaTime * 10);
        visXP = Mathf.Lerp(visXP, playerStats.GetLevel(), Time.deltaTime * 10);

        healthBar.fillAmount = visHP / 100;
        energyBar.fillAmount = visEP / playerStats.energyMax;

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
            betrayerCanvas.SetActive(value);
            if (value)
            {
                icon.color = Color.red;
                SetBetrayerGUI();
            }
            else icon.color = Color.blue;
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
