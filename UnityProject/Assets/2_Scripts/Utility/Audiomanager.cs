using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Audiomanager : MonoBehaviour {

    public static Audiomanager AM;

    public enum SOUNDTYPES { SFX, MUSIC, VOICELINE, REACTIONS, FOOTSTEPS};

    [Range(0,1)]
    public float SFXVolume = 1;
    [Range(0, 1)]
    public float MusicVolume = 1;
    [Range(0, 1)]
    public float VoiceLineVolume = 1;
    [Range(0, 1)]
    public float ReactionVolume = 1;
    [Range(0, 1)]
    public float FootstepsVolume = 1;

    // Use this for initialization
    void Start () {
        if (AM == null) {
            AM = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static float GetVolume(SOUNDTYPES type) {
        switch (type) {
            case SOUNDTYPES.MUSIC:
                return AM.MusicVolume;
            case SOUNDTYPES.SFX:
                return AM.SFXVolume;
            case SOUNDTYPES.VOICELINE:
                return AM.VoiceLineVolume;
            case SOUNDTYPES.REACTIONS:
                return AM.ReactionVolume;
            case SOUNDTYPES.FOOTSTEPS:
                return AM.FootstepsVolume;
            default:
                return 0;
        }
    }
}
