using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Network;
using System.Collections;

public class SceneChangeTrigger : NetworkBehaviour {

    public string nextScene;
    private LobbyManager lobbyManager;
    private float startTime = 0;
    [SerializeField]
    private float timeToChange = 2;
    private int stage = 0;
    [SerializeField]
    Sprite image;
    private bool hasBeenQueued = false;

	// Use this for initialization
	void Start ()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
	}

    void Update()
    {
        if(stage == 1 && startTime != 0 && Time.time > startTime + timeToChange)
        {
            stage = 2;
            if (!hasBeenQueued)
            {
                QueueSceneChange();
                hasBeenQueued = true;
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" && stage == 0) {
            Activate();
        }
    }

    void Activate() {
        startTime = Time.time;
        stage = 1;
        FindObjectOfType<ScreenFader>().Fade(timeToChange, true, image);
    }

    public void OverrideActivate(string s) {
        nextScene = s;
        Activate();
    }

    [ServerCallback]
    void QueueSceneChange()
    {
        if (lobbyManager != null) lobbyManager.ServerChangeScene(nextScene);
        
    }
}
