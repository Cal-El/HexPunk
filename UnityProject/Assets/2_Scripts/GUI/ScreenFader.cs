using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour {

    private float startTime;
    private float finishTime;
    private bool toBlack;
    private Image im;
    private float delayTimer = 0;

    public PlayerMovement myPlayerMovement;
    private bool hasEnabled;
    private int currentStage;

    // Use this for initialization
    void Start () {
        im = GetComponent<Image>();
        //Fade(1, false);
	}
	
	// Update is called once per frame
	void Update () {
        FindLocalPlayerMovement();

        if (Time.time < finishTime)
        {
            //Disable player
            if (myPlayerMovement.ControlEnabled)
            {
                myPlayerMovement.ControlEnabled = false;
            }

            if (toBlack)
                im.color = new Color(1, 1, 1, (Time.time - startTime) / (finishTime - startTime));
            else
                im.color = new Color(1, 1, 1, 1 - (Time.time - startTime) / (finishTime - startTime));
        }
        else
        {
            //Enable player
            if (!hasEnabled)
            {
                currentStage++;
                hasEnabled = true;
                if(currentStage % 2 == 0 && !myPlayerMovement.ControlEnabled) myPlayerMovement.ControlEnabled = true;
            }
        }

        if(delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
            if(delayTimer <= 0)
            {
                Fade(1, false);
            }
        }
        if (Megamanager.MM.SceneHasChanged)
        {
            Megamanager.MM.SceneHasChanged = false;
            delayTimer = 3;
        }
	}

    public void Fade (float _timeToFade, bool _toBlack, Sprite _img = null) {
        if(_img != null) {
            im.sprite = _img;
        }

        hasEnabled = false;

        startTime = Time.time;
        finishTime = startTime + _timeToFade;
        toBlack = _toBlack;
    }

    void FindLocalPlayerMovement()
    {
        if (myPlayerMovement == null)
        {
            foreach (PlayerMovement pm in FindObjectsOfType<PlayerMovement>())
            {
                if (pm.isLocalPlayer)
                {
                    myPlayerMovement = pm;
                }
            }
        }
    }
}
