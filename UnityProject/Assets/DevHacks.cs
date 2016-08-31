using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class DevHacks : MonoBehaviour {

    [SerializeField]
    private string password = "i have the power";
    private string typedMessage = "";
    protected bool hacksEnabled;
    public static DevHacks HAXOR;
    private GameObject child;
    public Text debugLog;

	// Use this for initialization
	void Start () {
	    if(HAXOR == null) {
            HAXOR = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        child = transform.GetChild(0).gameObject;
        child.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (!hacksEnabled && Input.anyKeyDown) {
            string nextRequiredLetter = password[typedMessage.Length].ToString().ToLower();
            if(nextRequiredLetter == " ") {
                nextRequiredLetter = "space";
            } else if (nextRequiredLetter == "4") {
                nextRequiredLetter = "left";
            } else if (nextRequiredLetter == "8") {
                nextRequiredLetter = "up";
            } else if (nextRequiredLetter == "6") {
                nextRequiredLetter = "right";
            } else if (nextRequiredLetter == "2") {
                nextRequiredLetter = "down";
            }

            if (Input.GetKeyDown(nextRequiredLetter)) {
                typedMessage += password[typedMessage.Length].ToString();
                if(typedMessage == password) {
                    child.SetActive(true);
                    hacksEnabled = true;
                }
            } else {
                typedMessage = "";
            }
        }
	}

    public static bool DevHacksEnabled {
        get {
            if (HAXOR != null)
                return HAXOR.hacksEnabled;
            else
                return false;
        }
    }

    

    public void ConsoleInput (string s) {
        string[] brokenUp = s.Split(' ');
        debugLog.text = "";

        switch (brokenUp[0]) {
            case "LoadScene":
                debugLog.text += "\nLoading Scene...";
                string sceneName = "";
                for(int i = 1; i < brokenUp.Length; i++) {
                    if(i > 1) {
                        sceneName += " ";
                    }
                    sceneName += brokenUp[i];
                }
                debugLog.text += "\nLoading Scene \"" + sceneName + "\"";
                ChangeScene(sceneName);
                break;
            case "SetMinPlayers":
                debugLog.text += "\nSetting minimum players...";
                try {
                    debugLog.text += "\nSetting minimum players to " + brokenUp[1];
                    SetMinPlayers(int.Parse(brokenUp[1]));
                } catch {
                    debugLog.text += "\nERROR: numOfPlayers = NaN";
                }
                break;
            case "ListPlayerIDs":
                ListCharactersIDs();
                break;
            case "ListStats":
                if (brokenUp.Length == 2) {
                    debugLog.text += "\nStats:";
                    try {
                        CheckStats(int.Parse(brokenUp[1]));
                    } catch {
                        debugLog.text += "\nERROR: NaN";
                    }
                } else {
                    debugLog.text += "\nNot a valid input. Type \"?\" to get a list of inputs";
                }
                break;
            case "God":
                if (brokenUp.Length == 2) {
                    debugLog.text += "\nMaking player #" + brokenUp[1] + " into a god...";
                    try {
                        HealPlayer(int.Parse(brokenUp[1]));
                    } catch {
                        debugLog.text += "\nERROR: NaN";
                    }
                } else {
                    debugLog.text += "\nNot a valid input. Type \"?\" to get a list of inputs";
                }
                break;
            case "Heal":
                if (brokenUp.Length == 3) {
                    debugLog.text += "\nHealing player #" + brokenUp[1];
                    try {
                        HealPlayer(int.Parse(brokenUp[1]), int.Parse(brokenUp[2]));
                    } catch {
                        debugLog.text += "\nERROR: NaN";
                    }
                } else {
                    debugLog.text += "\nNot a valid input. Type \"?\" to get a list of inputs";
                }
                break;
            case "Damage":
                if (brokenUp.Length == 3) {
                    debugLog.text += "\nDamaging player #" + brokenUp[1];
                    try {
                        DamagePlayer(int.Parse(brokenUp[1]), int.Parse(brokenUp[2]));
                    } catch {
                        debugLog.text += "\nERROR: NaN";
                    }
                } else {
                    debugLog.text += "\nNot a valid input. Type \"?\" to get a list of inputs";
                }
                break;
            case "LevelUp":
                if (brokenUp.Length == 2)
                {
                    debugLog.text += "\nAwarding +1 level...";
                    try
                    {
                        LevelUp(int.Parse(brokenUp[1]));
                    }
                    catch
                    {
                        debugLog.text += "\nERROR: NaN";
                    }
                }
                else
                {
                    debugLog.text += "\nNot a valid input. Type \"?\" to get a list of inputs";
                }
                break;
            case "DisableHacks":
                child.SetActive(false);
                hacksEnabled = false;
                typedMessage = "";
                break;
            case "?":
                Help();
                break;
            default:
                debugLog.text += "\nNot a valid input. Type \"?\" to get a list of inputs";
                break;
        }
    }

    public void ChangeScene(string sceneName) {
        if (SceneManager.GetActiveScene().buildIndex != 0) {
            if (SceneManager.GetSceneByName(sceneName) != null) {
                SceneChangeTrigger trig = FindObjectOfType<SceneChangeTrigger>();
                if (trig != null) {
                    debugLog.text += "\nQueued new scene";
                    trig.OverrideActivate(sceneName);
                } else {
                    debugLog.text += "\nUnable to find scene changer";
                }
            } else {
                debugLog.text += "\nScene does not exist in build settings";
            }
        } else {
            debugLog.text += "\nCannot load scene from main menu";
        }
    }

    public void SetMinPlayers (int numOfPlayers) {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            try {
                debugLog.text += "\nNew minimum set";
                FindObjectOfType<NetworkLobbyManager>().minPlayers = numOfPlayers;
            } catch {
                debugLog.text += "\nERROR: LobbyManager not found";
            }
        else
            debugLog.text += "\nCannot change minimum players when in-game";
    }

    public void ListCharactersIDs() {
        debugLog.text += "\nListing Players...";
        if (Megamanager.MM.players.Length == 0) {
            debugLog.text += "\nNo players in scene";
        } else {
            foreach (ClassAbilities c in Megamanager.MM.players) {
                debugLog.text += "\n" + c.name + ": " + c.ID;
            }
        }
    }

    public void CheckStats(int ID) {
        if (Megamanager.MM.players.Length > 0) {
            ClassAbilities c = Megamanager.MM.players[ID];
            debugLog.text += "\nName: " + c.name;
            debugLog.text += "\nHP: " + c.GetHealth() + "/" + c.healthMax;
            debugLog.text += "\nEnergy: " + c.energy + "/" + c.energyMax;
            debugLog.text += "\nPosition: (" + c.Position.x + ", " + c.Position.y + ", " + c.Position.z + ")";
        } else {
            debugLog.text += "\nNo player characters in scene";
        }
    }

    public void HealPlayer(int ID) {
        if (Megamanager.MM.players.Length > 0) {
            ClassAbilities c = Megamanager.MM.players[ID];
            if (!c.isActuallyGod) {
                debugLog.text += "\nMaking " + c.name + " a god";
                c.BecomeGod();
                debugLog.text += "\n" + c.name + " is now a god";
            } else {
                debugLog.text += "\nRevoking " + c.name + "'s godhood";
                c.BecomeGod();
                debugLog.text += "\n" + c.name + " is no longer a god";
            }
        } else {
            debugLog.text += "\nNo player characters in scene";
        }
    }

    public void LevelUp(int ID)
    {
        if (Megamanager.MM.players.Length > 0)
        {
            ClassAbilities c = Megamanager.MM.players[ID];
            debugLog.text += "\nGiving " + c.name + " +1 level";
            c.GainXP(1);
            debugLog.text += "\n" + c.name + " is now level " + c.GetLevel();
        }
        else
        {
            debugLog.text += "\nNo player characters in scene";
        }
    }

    public void DamagePlayer (int ID, int damage) {
        if (Megamanager.MM.players.Length > 0) {
            debugLog.text += "\nApplying " + damage + " of damage to " + Megamanager.MM.players[ID].name;
            float remainingHP = Megamanager.MM.players[ID].TakeDmg(damage, Character.DamageType.Standard);
            debugLog.text += "\n" + remainingHP + "HP remaining";
        } else {
            debugLog.text += "\nNo player characters in scene";
        }
    }

    public void HealPlayer (int ID, int heal) {
        if (Megamanager.MM.players.Length > 0) {
            debugLog.text += "\nApplying " + heal + " of health to " + Megamanager.MM.players[ID].name;
            Megamanager.MM.players[ID].Heal(heal);
            debugLog.text += "\n" + Megamanager.MM.players[ID].health + "HP remaining";
        } else {
            debugLog.text += "\nNo player characters in scene";
        }
    }

    public void Help() {
        debugLog.text += "\nLoadScene [SceneName] - Loads the scene that matches that name";
        debugLog.text += "\nSetMinPlayers [numOfPlayers] - Sets the minimum players to the input";
        debugLog.text += "\nListPlayerIDs - Lists the IDs of the players in the Megamanger";
        debugLog.text += "\nListStats [PlayerID] - Lists the name, current health, current energy, and position of the player";
        debugLog.text += "\nGod [PlayerID] - Switches player from god to mortal or mortal to god";
        debugLog.text += "\nHeal [PlayerID] [Damage] - Heals a player";
        debugLog.text += "\nDamage [PlayerID] [Damage] - Damages a player";
        debugLog.text += "\nDisableHacks - Disables devhacks";
        debugLog.text += "\n? - List all commands";
    }
}
