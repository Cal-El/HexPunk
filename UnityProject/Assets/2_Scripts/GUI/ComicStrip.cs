using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ComicStrip : MonoBehaviour {
    
    public AnimationCurve transitionAnimation;
    public float timeToTransition = 5;
    private float transitionTimer = 0;
    private float pastYPos = 0;
    private float currentYPos = 0;
    private bool betrayerChosen = false;


    public PlayerGUICanvas playerGui;
    public List<GameObject> classes = new List<GameObject>();
    public List<GameObject> betrayerPanels = new List<GameObject>();
    public List<GameObject> nonBetrayerPanels = new List<GameObject>();


    // Use this for initialization
    void Start () {
        if (playerGui.myPlayer != null)
        {
            Setup();
        }
    }

    private void Setup()
    {
        SetHUDs(playerGui.myPlayer.name, "Conduit");
        SetHUDs(playerGui.myPlayer.name, "Aethersmith");
        SetHUDs(playerGui.myPlayer.name, "Caldera");
        SetHUDs(playerGui.myPlayer.name, "Shard");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(playerGui.IsBetrayer && !betrayerChosen)
        {
            foreach (GameObject panel in nonBetrayerPanels)
            {
                if (panel != null) Destroy(panel);
            }
        }

        if (transitionTimer >= timeToTransition)
        {
            transitionTimer = 0;
            pastYPos = currentYPos;
            currentYPos += 1080;
        }
        else
        {
            transitionTimer += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(new Vector3(0, pastYPos, 0), new Vector3(0, currentYPos, 0), transitionAnimation.Evaluate(transitionTimer / timeToTransition));
        }
    }

    private void SetHUDs(string playerName, string className)
    {
        if (playerName.Contains(className))
        {
            foreach (var classPanel in classes)
            {
                if (!classPanel.gameObject.name.Contains(className))
                {
                    Destroy(classPanel);
                }
            }
        }
    }
}
