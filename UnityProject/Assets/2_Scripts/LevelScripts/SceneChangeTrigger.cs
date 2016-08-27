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
            QueueSceneChange();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" && stage == 0) {
            startTime = Time.time;
            stage = 1;
            FindObjectOfType<ScreenFader>().Fade(timeToChange, true);
        }
    }

    [ServerCallback]
    void QueueSceneChange()
    {
        if (lobbyManager != null) lobbyManager.ServerChangeScene(nextScene);
        
    }
}
