﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OtherPlayerGUI : MonoBehaviour
{
    public GameObject player;

    public Image icon;
    public Sprite[] sprites = new Sprite[4];
    public Image healthBar;

    private ClassAbilities playerStats;

    private float visHP;

    // Use this for initialization
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        if(playerStats == null)
        {
            Setup();
        }
        healthBar.fillAmount = playerStats.health / 100;
    }

    void Setup()
    {
        if (player == null) return;
        playerStats = player.GetComponent<ClassAbilities>();

        if (player.name.Contains("Conduit"))
        {
            icon.sprite = sprites[0];
        }
        if (player.name.Contains("Aethersmith"))
        {
            icon.sprite = sprites[1];
        }
        if (player.name.Contains("Caldera"))
        {
            icon.sprite = sprites[2];
        }
        if (player.name.Contains("Shard"))
        {
            icon.sprite = sprites[3];
        }
    }
}
