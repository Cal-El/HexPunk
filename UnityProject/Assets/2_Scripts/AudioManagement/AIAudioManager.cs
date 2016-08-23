using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIAudioManager : CharacterAudioManager {
    
    public AudioSourceManager deathBurnElectric;

    public void PlayDeathBurnElectricAudio()
    {
        PlayRandomFromClips(deathBurnElectric);
    }
}
