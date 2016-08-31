using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIPopup : MonoBehaviour {

    [SerializeField]private Text myText;
    [SerializeField] private RectTransform parent;
    [SerializeField] private AnimationCurve anim;
    [SerializeField]
    private Vector3 onScreenPos;
    [SerializeField]
    private Vector3 offScreenPos;

    private Vector3 doubleDistanceOnscreen;
    [SerializeField]private string message;
    private float startTime;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        myText.text = message;
        doubleDistanceOnscreen = (onScreenPos - offScreenPos) * 2 + offScreenPos;

    }
	
	// Update is called once per frame
	void Update () {
        parent.anchoredPosition = Vector3.Lerp(offScreenPos, doubleDistanceOnscreen, anim.Evaluate(Time.time - startTime)/2);
        if((Time.time - startTime) > anim.keys[anim.keys.Length - 1].time) {
            Destroy(this.gameObject);
        }
	}
}
