using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMenu : MonoBehaviour {
    public Animator[] anims;

    void Start()
    {
        foreach (Animator anim in anims)
        {
            anim.Play("FadeIn");
        }
    }
    public void FadeOut()
    {
        foreach (Animator anim in anims)
        {
            anim.Play("FadeOut");
        }
    }
    protected virtual IEnumerator Fade()
    {
        foreach (Animator anim in anims)
        {
            anim.Play("Fade");
        }
        yield return new WaitForSeconds(1.1f);
    }
}
