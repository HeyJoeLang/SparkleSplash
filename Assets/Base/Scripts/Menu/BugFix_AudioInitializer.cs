using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugFix_AudioInitializer : MonoBehaviour {

	// Bug Fix : Sometimes audio does not start when set to PlayOnStart
	void Start () {
        StartCoroutine(StartMusic());
	}
    IEnumerator StartMusic()
    {
        yield return new  WaitForSeconds(.5f);
        GetComponent<AudioSource>().enabled = true;

    }
	
}
