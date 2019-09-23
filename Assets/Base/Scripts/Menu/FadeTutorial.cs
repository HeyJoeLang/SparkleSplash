using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeTutorial : FadeMenu {
    string scene = "";
    public void FadeLoadScene(string load)
    {
        scene = load;
        StartCoroutine(Fade());
    }
    protected override IEnumerator Fade()
    {
        yield return StartCoroutine(base.Fade());
        SceneManager.LoadScene(scene);
    }
}
