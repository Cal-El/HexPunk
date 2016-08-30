using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

    public static MainMenuManager singleton;

    public enum MENUSTATES {All, MainMenu, LobbyScreen, SettingsScreen};
    private MENUSTATES currState = MENUSTATES.MainMenu;
    public GameObject lobbyManager;
    private string startScene;

    [System.Serializable]
    public class MenuElement {
        [Tooltip("Drag the menu element here")]
        public RectTransform tr;
        [Tooltip("The time is takes to transition. Must be positive and not zero")]
        public float timeToTransition = 1;
        [Tooltip("A position offscreen so the menu element doesn't appear at the wrong time.")]
        public Vector2 offScreenPos;
        [Tooltip("The place onscreen you want the menu element to appear.")]
        public Vector2 onScreenPos;
        [Tooltip("The state you want the menu element to appear in.")]
        public MENUSTATES appearInState;

        private bool isAButton = false;
        private bool isOnScreen = false;
        private Button myButton = null;
        private bool isTransitioning = false;
        private float transitionTimer = 0;

        public void Start() {
            isOnScreen = false;
            transitionTimer = timeToTransition;
            tr.anchoredPosition = offScreenPos;
            myButton = tr.GetComponent<Button>();
            if (myButton != null) isAButton = true;
            else isAButton = false;

            ChangeState(MENUSTATES.MainMenu);
        }

        public void Update() {
            if(transitionTimer < timeToTransition) {
                transitionTimer += Time.deltaTime;
                isTransitioning = true;
            } else {
                isTransitioning = false;
            }

            if (isTransitioning) {
                if (isOnScreen) {
                    tr.anchoredPosition = Vector3.Lerp(offScreenPos, onScreenPos, singleton.transitionAnimation.Evaluate(transitionTimer / timeToTransition));
                } else {
                    tr.anchoredPosition = Vector3.Lerp(onScreenPos, offScreenPos, singleton.transitionAnimation.Evaluate(transitionTimer / timeToTransition));
                }
            }
        }

        public void ChangeState(MENUSTATES newState) {
            if (appearInState == MENUSTATES.All) return;
            if (isOnScreen && newState != appearInState) {
                transitionTimer = 0;
                isOnScreen = false;
                if (isAButton) {
                    myButton.interactable = false;
                }
            } else if(!isOnScreen && newState == appearInState) {
                transitionTimer = 0;
                isOnScreen = true;
                if (isAButton) {
                    myButton.interactable = true;
                }
            }
        }
    }

    [SerializeField] MenuElement[] menuElements;
    public AnimationCurve transitionAnimation;
    [HideInInspector]public float transitionTimer;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        startScene = SceneManager.GetActiveScene().name;
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach(MenuElement m in menuElements) {
            m.Start();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (SceneManager.GetActiveScene().name != startScene) Destroy(gameObject);
	    foreach (MenuElement m in menuElements) {
            m.Update();
        }
	}

    public void Play()
    {
        currState = MENUSTATES.LobbyScreen;
        foreach (MenuElement m in menuElements) {
            m.ChangeState(currState);
        }
        
    }

    public void Settings()
    {
        currState = MENUSTATES.SettingsScreen;
        foreach (MenuElement m in menuElements) {
            m.ChangeState(currState);
        }
    }

    public void Back() {
        currState = MENUSTATES.MainMenu;
        foreach (MenuElement m in menuElements) {
            m.ChangeState(currState);
        }
    }

    public void Quit() {
        Application.Quit();
    }
}
