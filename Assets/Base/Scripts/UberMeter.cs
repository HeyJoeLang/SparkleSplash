using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UberMeter : MonoBehaviour {

    float rayGunTime = 20f;
    ProgressBarPro uberSlider;
    float pointsUntilFull = 5;
    public float currentPercent = 0;
    Animator anim;

	void Start () {
        uberSlider = GetComponent<ProgressBarPro>();
        anim = GetComponent<Animator>();
        anim.speed = 0;
	}
    IEnumerator YieldAnimation()
    {
        yield return new WaitForSeconds(rayGunTime);
        anim.speed = 0;
    }
    public bool AddUberPoint()
    {
        float newPercent = currentPercent + 1 / pointsUntilFull;
        if (currentPercent == 1)
        {
            GetComponent<ParticleSystem>().Play();
            StartCoroutine(YieldAnimation());
            uberSlider.animTime = rayGunTime;
            uberSlider.Value = currentPercent = 0;
            return true;
        }
        currentPercent = newPercent;
        if (currentPercent == 1f)
            anim.speed = 1f;
        uberSlider.animTime = .25f;
        uberSlider.Value = currentPercent;
        return false;
    }
}
