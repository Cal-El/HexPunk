using UnityEngine;
using System.Collections;

public class BasementMusic : MonoBehaviour {

    [System.Serializable] public class MusicTrack {
        [HideInInspector]
        public AudioSource adsc;
        [Tooltip("Music Track")]
        public AudioClip track;
        [Tooltip("x = No. of Enemies; y = Volume |clamp(0,1)|")]
        public AnimationCurve volumeCurve;
    }

    public MusicTrack[] tracks;
    private float lerper = 0;

	// Use this for initialization
	void Start () {
        lerper = Megamanager.GetAllCharacters().Length;

        foreach (MusicTrack m in tracks) {
            m.adsc = gameObject.AddComponent<AudioSource>();
            m.adsc.spatialBlend = 0;
            m.adsc.clip = m.track;
            m.adsc.volume = m.volumeCurve.Evaluate(lerper);
            m.adsc.loop = true;
            m.adsc.Play();
        }
	}
	
	// Update is called once per frame
	void Update () {
        lerper = Mathf.Lerp(lerper, Megamanager.GetAllCharacters().Length, Time.deltaTime);
        foreach (MusicTrack m in tracks) {
            m.adsc.volume = m.volumeCurve.Evaluate(lerper);
        }
    }
}
