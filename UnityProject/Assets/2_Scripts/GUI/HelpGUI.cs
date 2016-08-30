using UnityEngine;
using System.Collections;

public class HelpGUI : MonoBehaviour {

    public GameObject[] huds = new GameObject[4];
    public PlayerGUICanvas playerGuiCanvas;

	// Use this for initialization
	void Start () {
        string name = playerGuiCanvas.myPlayer.gameObject.name;

        SetHUDs(name, "Conduit");
        SetHUDs(name, "Aethersmith");
        SetHUDs(name, "Caldera");
        SetHUDs(name, "Shard");
        gameObject.SetActive(false);
    }


    private void SetHUDs(string playerName, string className)
    {
        if (playerName.Contains(className))
        {
            foreach (var hud in huds)
            {
                if (!hud.gameObject.name.Contains(className))
                {
                    Destroy(hud);
                }
            }
        }
    }
}
