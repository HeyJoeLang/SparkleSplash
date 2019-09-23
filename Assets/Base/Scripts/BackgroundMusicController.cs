using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicController : GazeModeListener {

    public AudioSource standard, magicMode;
    float transitionTime = 1f;
    float maxVolume = .1f;
	void Start () {
        StartCoroutine(StartAudio(standard, maxVolume));
	}
	IEnumerator StartAudio(AudioSource source, float destination)
    {
        float totalTime = 0;
        while(totalTime < transitionTime)
        {
            source.volume = totalTime / transitionTime * destination;
            totalTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        source.volume = destination;
    }
    IEnumerator SwitchAudio(AudioSource source1, AudioSource source2, float destination)
    {
        float totalTime = 0;
        while (totalTime < transitionTime)
        {
            source1.volume = totalTime / transitionTime * destination;
            source2.volume = destination - source1.volume;
            totalTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        source1.volume = destination;
        source2.volume = 0;
    }
    public override void GazeToTimedShot()
    {
        base.GazeToTimedShot();
        StartCoroutine(SwitchAudio(standard, magicMode, maxVolume));
    }
    public override void GazeToMagicBlaster()
    {
        base.GazeToMagicBlaster();
        StartCoroutine(SwitchAudio(magicMode, standard, maxVolume));
    }
}
