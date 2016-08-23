using UnityEngine;
using System.Collections;

public class CharacterAudioManager : MonoBehaviour {

    public AudioSourceManager footstep;
    public AudioSourceManager takeDamage;
    public AudioSourceManager death;
    public SpeechAudioSourceManager speech;

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

    public void PlaySpeechPhrase(Phrase.PhraseType phraseType)
    {

    }

    protected void PlayRandomFromClips(AudioSourceManager sourceManager)
    {
        if (!sourceManager.source.isPlaying)
        {
            sourceManager.source.PlayOneShot(sourceManager.audioClips[Random.Range(0, sourceManager.audioClips.Length - 1)]);
        }
    }

    protected void PlayRandomPhraseOfType(Phrase.PhraseType phraseType)
    {
        if (!speech.source.isPlaying)
        {
            foreach (var phrase in speech.phrases)
            {
                if (!speech.source.isPlaying)
                {
                    if(phrase.phraseType == phraseType)
                    {
                        speech.source.PlayOneShot(phrase.audioClips[Random.Range(0, phrase.audioClips.Length - 1)]);
                    }
                }
            }
        }
    }
}
