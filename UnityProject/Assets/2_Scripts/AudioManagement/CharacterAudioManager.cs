using UnityEngine;
using System.Collections;

public class CharacterAudioManager : MonoBehaviour {

    public AudioSourceManager footstep;
    public AudioSourceManager takeDamage;
    public AudioSourceManager death;

    public void PlayTakeDamageAudio()
    {
        takeDamage.source.PlayOneShot(takeDamage.audioClips[Random.Range(0, takeDamage.audioClips.Length - 1)]);
    }

    public void PlayFootstepAudio()
    {
        footstep.source.PlayOneShot(footstep.audioClips[Random.Range(0, footstep.audioClips.Length - 1)]);
    }

    public void PlayDeathAudio()
    {
        death.source.PlayOneShot(death.audioClips[Random.Range(0, death.audioClips.Length - 1)]);
    }
}
