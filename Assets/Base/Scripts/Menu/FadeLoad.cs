using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeLoad : MonoBehaviour
{
    private Animator[] anims;

    void Start()
    {
        anims = GetComponentsInChildren<Animator>();
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
    public void FadeOutLoadScene(int sceneIndex)
    {
        StartCoroutine(FadeOutLoad(sceneIndex));
    }
    private IEnumerator FadeOutLoad(int sceneIndex)
    {
        FadeOut();
        yield return new WaitForSeconds(1.1f);
        SceneManager.LoadScene(sceneIndex);
    }
}
