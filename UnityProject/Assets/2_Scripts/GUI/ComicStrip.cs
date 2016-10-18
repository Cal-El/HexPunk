using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

public class ComicStrip : MonoBehaviour {
    
    public AnimationCurve transitionAnimation;
    public float totalComicTime = 40;
    private float currentTotalTime;
    public int numberOfSlides = 5;
    private float timeToTransition;
    private float transitionTimer = 0;
    private float pastYPos = 0;
    private float currentYPos = 0;
    private bool betrayerChosen = false;


    public PlayerGUICanvas playerGui;
    public PlayerMovement myPlayerMovement;
    private bool hasEnabled;
    public List<GameObject> classes = new List<GameObject>();
    public List<GameObject> betrayerPanels = new List<GameObject>();
    public List<GameObject> nonBetrayerPanels = new List<GameObject>();


    // Use this for initialization
    void Start () {
        timeToTransition = totalComicTime / numberOfSlides;
        if (playerGui.myPlayer != null)
        {
            Setup();
            myPlayerMovement = playerGui.myPlayer.GetComponent<PlayerMovement>();
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
    void Update()
    {
        if (myPlayerMovement == null && playerGui != null && playerGui.myPlayer != null)
        {
            myPlayerMovement = playerGui.myPlayer.GetComponent<PlayerMovement>();
        }

        if (playerGui.IsBetrayer && !betrayerChosen)
        {
            foreach (GameObject panel in nonBetrayerPanels)
            {
                if (panel != null) Destroy(panel);
            }
        }

        if (currentTotalTime < totalComicTime)
        {
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

            currentTotalTime += Time.deltaTime;
        }

        if (currentTotalTime >= totalComicTime - totalComicTime / numberOfSlides)
        {
            if (!hasEnabled)
            {
                //Enable player
                if (!myPlayerMovement.ControlEnabled)
                {
                    myPlayerMovement.ControlEnabled = true;
                    hasEnabled = true;
                }
            }
        }
        else
        {
            //Disable player
            if (myPlayerMovement.ControlEnabled)
            {
                myPlayerMovement.ControlEnabled = false;
            }
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
