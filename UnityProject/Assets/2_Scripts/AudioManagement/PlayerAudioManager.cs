using UnityEngine;
using System.Collections;

public class PlayerAudioManager : CharacterAudioManager
{
    public AudioSourceManager revive;

    public void PlayReviveAudio()
    {
        revive.source.PlayOneShot(revive.audioClips[Random.Range(0, revive.audioClips.Length - 1)]);
    }
}
