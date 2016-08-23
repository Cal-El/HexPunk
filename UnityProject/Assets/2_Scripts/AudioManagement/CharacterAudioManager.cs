using UnityEngine;
using System.Collections;

public class CharacterAudioManager : MonoBehaviour {

    public AudioSourceManager footstep;
    public AudioSourceManager takeDamage;
    public AudioSourceManager death;

    public void PlayTakeDamageAudio()
    {
        PlayRandomFromClips(takeDamage);
    }

    public void PlayFootstepAudio()
    {
        PlayRandomFromClips(footstep);
    }

    public void PlayDeathAudio()
    {
        PlayRandomFromClips(death);
    }

    protected void PlayRandomFromClips(AudioSourceManager sourceManager)
    {
        if (!sourceManager.source.isPlaying)
        {
            sourceManager.source.PlayOneShot(sourceManager.audioClips[Random.Range(0, sourceManager.audioClips.Length - 1)]);
        }
    }
}
