using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIPopup : MonoBehaviour {

	public static GUIPopup Popup;
    [SerializeField]private Text myText;
    [SerializeField] private RectTransform parent;
    [SerializeField] private AnimationCurve anim;
    [SerializeField]
    private Vector3 onScreenPos;
    [SerializeField]
    private Vector3 offScreenPos;

    //Play after comic ends
    private ComicStrip comic;
    private float timer;

    private Vector3 doubleDistanceOnscreen;
    [SerializeField]private string message;
    private float startTime;
	private bool active;

	// Use this for initialization
	void Start () {
		if (Popup == null) {
			Popup = this;
			DontDestroyOnLoad (this.gameObject);
		} else {
			Destroy (this.gameObject);
		}
        myText.text = message;
        doubleDistanceOnscreen = (onScreenPos - offScreenPos) * 2 + offScreenPos;
        comic = FindObjectOfType<ComicStrip>();
    }
	
	// Update is called once per frame
	void Update () {
        //Active after comic ends
        if (!active)
        {
            timer += Time.deltaTime;
            if(comic == null)
            {
                comic = FindObjectOfType<ComicStrip>();
            }
            else
            {
                if (timer >= comic.totalComicTime - comic.totalComicTime / comic.numberOfSlides)
                {
                    startTime = Time.time + 3;
                    active = true;
                }
            }
        }

		if (active) {
			myText.text = message;
			parent.anchoredPosition = Vector3.Lerp (offScreenPos, doubleDistanceOnscreen, anim.Evaluate (Time.time - startTime) / 2);
			if ((Time.time - startTime) > anim.keys [anim.keys.Length - 1].time) {
				active = false;
			}
		}
	}

	public static void ShowMessage(string message){
		Popup.message = message;
		Popup.active = true;
		Popup.startTime = Time.time;
	}
}
