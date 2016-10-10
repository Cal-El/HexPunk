using UnityEngine;
using System.Collections;

[System.Serializable]
public class AudioSourceManager : MonoBehaviour
{    
    public enum Type { TakeDamage, Footsteps, Death, Revive, DeathBurnElectric };
    public Type type;
    public AudioSource source;
    public AudioClip[] audioClips;

    void Start() {
        if (type == Type.Footsteps) {
            source.volume = Audiomanager.GetVolume(Audiomanager.SOUNDTYPES.FOOTSTEPS);
        } else {
            source.volume = Audiomanager.GetVolume(Audiomanager.SOUNDTYPES.REACTIONS);
        }
    }
}
