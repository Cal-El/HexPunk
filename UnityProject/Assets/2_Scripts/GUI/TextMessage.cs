using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextMessage : MonoBehaviour {

    public PlayerGUICanvas pGUI;
    private string message;
    private Text messageArea;
    [SerializeField]
    private float timer = 0;
    private bool isPlaying = false;
    private RectTransform rectTr;
    [SerializeField] private Vector2 hiddenPosition;
    [SerializeField] private Vector2 displayedPosition;
    [SerializeField] private AnimationCurve frameAnimation;

	// Use this for initialization
	void Start () {
        rectTr = GetComponent<RectTransform>();
        messageArea = GetComponentInChildren<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        if (isPlaying) {
            timer += Time.deltaTime;
            Vector2 lerpB = new Vector2((displayedPosition.x-hiddenPosition.x), (displayedPosition.y- hiddenPosition.y));
            rectTr.anchoredPosition = Vector2.Lerp(hiddenPosition, lerpB, frameAnimation.Evaluate(timer)/2);
            if (timer >= frameAnimation.keys[frameAnimation.length-1].time) isPlaying = false;
        }
	}

    public void SendText(string _message) {
        if (pGUI.IsBetrayer) {
            timer = 0;
            message = _message;
            messageArea.text = _message;
            isPlaying = true;
        }
    }
}
