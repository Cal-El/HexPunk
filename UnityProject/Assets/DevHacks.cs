using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class DevHacks : MonoBehaviour {

    [SerializeField]
    private string password = "i have the power";
    private string typedMessage = "";
    protected bool hacksEnabled;
    public static DevHacks HAXOR;
    private GameObject child;

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
            string nextRequiredLetter = password[typedMessage.Length].ToString();
            if(nextRequiredLetter == " ") {
                nextRequiredLetter = "space";
            }

            if (Input.GetKeyDown(nextRequiredLetter)) {
                typedMessage += password[typedMessage.Length].ToString(); ;
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

        switch (brokenUp[0]) {
            case "LoadScene":
                string sceneName = "";
                for(int i = 1; i < brokenUp.Length; i++) {
                    if(i > 1) {
                        sceneName += " ";
                    }
                    sceneName += brokenUp[i];
                }
                ChangeScene(sceneName);
                break;
            case "SetMinPlayers":
                try {
                    SetMinPlayers(int.Parse(brokenUp[1]));
                } catch {
                    Debug.LogError("ERROR: numOfPlayers = NaN");
                }
                break;
            default:
                Debug.Log("Not a valid input");
                break;
        }
    }

    public void ChangeScene(string sceneName) {
        if (SceneManager.GetSceneByName(sceneName) != null && SceneManager.GetActiveScene().buildIndex != 0) {
            SceneChangeTrigger trig = FindObjectOfType<SceneChangeTrigger>();
            if (trig != null) {
                trig.OverrideActivate(sceneName);
            } else {
                Debug.LogError("Unable to find scene changer");
            }
        } else {
            Debug.LogError("Cannot Load Scene: " + sceneName);
        }
    }

    public void SetMinPlayers (int numOfPlayers) {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            try {
                FindObjectOfType<NetworkLobbyManager>().minPlayers = numOfPlayers;
            } catch {
                Debug.LogError("ERROR: LobbyManager not found");
            }
        else
            Debug.LogError("Cannot change minimum players when in-game");
    }
}
