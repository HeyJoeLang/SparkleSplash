using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointSoundCollection
{
    public AudioClip[] clips;
    int index = 0;

    public AudioClip NextAudio()
    {
        index = (index == clips.Length - 1) ? 0 : index + 1;
        return clips[index];
    }
}

[RequireComponent(typeof(AudioSource))]
public class PointSoundsManager : ScoreChangeListener {
    public float pitchRangeMin = 0;
    public float pitchRangeMax = 0;
    public PointSoundCollection animalSounds, environmentSounds;
    AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public override void OnScoreRaised()
    {
        base.OnScoreRaised();
        PlayNextAnimalSound();
    }

    void PlayNextAnimalSound()
    {
        if (!source.isPlaying)
        {
            source.pitch = Random.Range(pitchRangeMin, pitchRangeMax);
            source.PlayOneShot(animalSounds.NextAudio());
        }
    }

    void PlayNextEnvironmentSound()
    {
        source.PlayOneShot(environmentSounds.NextAudio());
    }
}
