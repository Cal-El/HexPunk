using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Network;
using System.Collections;

public class SceneChangeTrigger : NetworkBehaviour {

    public string nextScene;
    private LobbyManager lobbyManager;

	// Use this for initialization
	void Start ()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player") QueueSceneChange();
    }

    [ServerCallback]
    void QueueSceneChange()
    {
        if(lobbyManager != null) lobbyManager.ServerChangeScene(nextScene);
    }
}
