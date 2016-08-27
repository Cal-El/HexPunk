using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

    public static MainMenuManager singleton;

    public enum MENUSTATES {All, MainMenu, LobbyScreen, SettingsScreen};
    [HideInInspector]
    public MENUSTATES currState = MENUSTATES.MainMenu;
    public GameObject lobbyManager;
    private string startScene;

    [System.Serializable]
    public class MenuElement {
        public RectTransform tr;
        public float timeToTransition;
        public Vector2 offScreenPos;
        public Vector2 onScreenPos;
        public MENUSTATES appearInState;
        [HideInInspector] public bool isOnScreen;
        [HideInInspector] public bool isTransitioning;
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
	}
	
	// Update is called once per frame
	void Update () {
        if (SceneManager.GetActiveScene().name != startScene) Destroy(gameObject);
        transitionTimer += Time.deltaTime;
	    foreach (MenuElement m in menuElements) {
            if (m.appearInState == MENUSTATES.All) {
                m.isOnScreen = true;
                m.tr.anchoredPosition = Vector3.Lerp(m.offScreenPos, m.onScreenPos, transitionAnimation.Evaluate(Time.timeSinceLevelLoad / m.timeToTransition));
            } else {
                m.isOnScreen = (currState == m.appearInState);
                //if (m.isTransitioning) {
                    if (m.isOnScreen) {
                        m.tr.anchoredPosition = Vector3.Lerp(m.offScreenPos, m.onScreenPos, transitionAnimation.Evaluate(transitionTimer / m.timeToTransition));
                    } else {
                        m.tr.anchoredPosition = Vector3.Lerp(m.onScreenPos, m.offScreenPos, transitionAnimation.Evaluate(transitionTimer / m.timeToTransition));
                    }
                //}
            }
        }
	}

    public void Play()
    {
        foreach(var element in menuElements)
        {
            if(element.appearInState == MENUSTATES.LobbyScreen)
            {
                element.timeToTransition = 1.75f;
            }
        }
        transitionTimer = 0;
        currState = MENUSTATES.LobbyScreen;
    }

    public void Settings()
    {
        foreach (var element in menuElements)
        {
            if (element.appearInState == MENUSTATES.SettingsScreen)
            {
                element.timeToTransition = 1.75f;
            }
        }
        transitionTimer = 0;
        currState = MENUSTATES.SettingsScreen;
    }

    public void Back() {
        transitionTimer = 0;
        currState = MENUSTATES.MainMenu;
    }

    public void Quit() {
        Application.Quit();
    }
}
