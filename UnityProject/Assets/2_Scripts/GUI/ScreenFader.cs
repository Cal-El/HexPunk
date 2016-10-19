using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour {

    private float startTime;
    private float finishTime;
    private bool toBlack;
    private Image im;
    private float delayTimer = 0;
    private bool hasEnabled = false;

    public PlayerMovement myPlayerMovement;

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
            if (toBlack)
                im.color = new Color(1, 1, 1, (Time.time - startTime) / (finishTime - startTime));
            else
                im.color = new Color(1, 1, 1, 1 - (Time.time - startTime) / (finishTime - startTime));
        }
        else
        {
            if (!toBlack && !hasEnabled)
            {
                if (myPlayerMovement != null)
                {
                    myPlayerMovement.ControlEnabled = true;
                    hasEnabled = true;
                }
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

        startTime = Time.time;
        finishTime = startTime + _timeToFade;
        toBlack = _toBlack;

        if (myPlayerMovement != null && toBlack)
        {
            myPlayerMovement.ControlEnabled = false;
            hasEnabled = false;
        }
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
