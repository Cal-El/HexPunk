using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStatsGUI : MonoBehaviour
{
    private string _playerName;
    /// <summary>
    /// Player name needs to be set before IsBetrayer
    /// </summary>
    public string PlayerName
    {
        get
        {
            return _playerName;
        }
        set
        {
            _playerName = value;
            SetHUDs(_playerName, "Conduit");
            SetHUDs(_playerName, "Aethersmith");
            SetHUDs(_playerName, "Caldera");
            SetHUDs(_playerName, "Shard");
        }
    }

    private bool _isBetrayer;
    /// <summary>
    /// Player name needs to be set before IsBetrayer
    /// </summary>
    public bool IsBetrayer
    {
        get
        {
            return _isBetrayer;
        }
        set
        {
            _isBetrayer = value;
            betrayerText.SetActive(_isBetrayer);
            SetBetrayerIcon(PlayerName);
        }
    }

    public GameObject betrayerText;

    public GameObject[] playerIcons;

    private Image playerIcon;

    public Sprite conduitBetrayer;
    public Sprite aethersmithBetrayer;
    public Sprite calderaBetrayer;
    public Sprite shardBetrayer;

    public Text damageDealt;
    public Text damageTaken;
    public Text kills;
    public Text playerKills;
    public Text deaths;
    public Text alliesRevived;
    
    private void SetHUDs(string playerName, string className)
    {
        if (playerName.Contains(className))
        {
            foreach (var icon in playerIcons)
            {
                if (!icon.gameObject.name.Contains(className))
                {
                    Destroy(icon);
                }
                else
                {
                    playerIcon = icon.GetComponent<Image>();
                }
            }
        }
    }

    private void SetBetrayerIcon(string playerName)
    {
        if (playerName.Contains("Conduit"))
        {
            playerIcon.sprite = conduitBetrayer;
        }
        else if (playerName.Contains("Aethersmith"))
        {
            playerIcon.sprite = aethersmithBetrayer;
        }
        else if (playerName.Contains("Caldera"))
        {
            playerIcon.sprite = calderaBetrayer;
        }
        else if (playerName.Contains("Shard"))
        {
            playerIcon.sprite = shardBetrayer;
        }
    }
}
