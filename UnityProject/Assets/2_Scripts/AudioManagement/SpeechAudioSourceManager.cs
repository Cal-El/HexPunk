using UnityEngine;
using System.Collections;


[System.Serializable]
public class SpeechAudioSourceManager : MonoBehaviour
{
    public AudioSource source;
    public Phrase[] phrases = new Phrase[8];
}

[System.Serializable]
public class Phrase
{
    public enum PhraseType { Encounter, Combat, UsingAbility, AfterFight, NearDeath, Revived, DamagedByAlly, BetrayalRevealed }
    public PhraseType phraseType;
    public AudioClip[] audioClips;
}
