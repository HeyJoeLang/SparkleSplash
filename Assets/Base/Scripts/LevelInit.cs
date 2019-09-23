using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInit : MonoBehaviour {
    public AudioSource source;
    public AudioClip introAudio;
    public WorldController move;
    public GazeCaster gaze;

    public void StartLevel()
    {
        StartCoroutine("PlayAudioThenStart");
    }
    IEnumerator PlayAudioThenStart()
    {
        yield return new WaitForSeconds(2);
        source.PlayOneShot(introAudio);
        yield return new WaitForSeconds(introAudio.length + 2);
        move.StartMovement();
        yield return new WaitForSeconds(2);
        gaze.enabled = true;
    }
}
