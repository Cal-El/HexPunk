using UnityEngine;
using System.Collections;

[System.Serializable]
public class AudioSourceManager : MonoBehaviour
{    
    public enum Type { TakeDamage, Footsteps, Death, Revive, DeathBurnElectric };
    public Type type;
    public AudioSource source;
    public AudioClip[] audioClips;
}
