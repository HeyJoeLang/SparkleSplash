using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelTrigger : MonoBehaviour {

    private LevelController lvlCont;
    public ParticleSystem endLvlFireworks;

	void Start () {
        lvlCont = GameObject.Find("LevelController").GetComponent<LevelController>();
	}

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "MainCamera")
        {
            endLvlFireworks.Play();
            lvlCont.EndOfLevelReached();
        }
    }
    void Update () {
		
	}
}
