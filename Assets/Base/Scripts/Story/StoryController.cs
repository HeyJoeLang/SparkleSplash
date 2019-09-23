using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PageContent
{
    public float prePageDelay;
    public AudioClip pageVocals;
}

public class StoryController : MonoBehaviour {
    public FadeTutorial fade;
    public SceneLoader loader;
    AudioSource source;
    public PageContent[] content;
    public AutoFlip flipper;

    void Start()
    {
        source = GetComponent<AudioSource>();
        StartCoroutine("TurnNextPage");
    }

    public IEnumerator TurnNextPage()
    {
        for(int i = 0; i < content.Length; i++)
        {
            yield return new WaitForSeconds(content[i].prePageDelay);
            if (content[i].pageVocals != null)
            {
                source.PlayOneShot(content[i].pageVocals);
                yield return new WaitForSeconds(content[i].pageVocals.length + 1);
            }
            flipper.FlipRightPage();
        }
        fade.FadeOut();
        yield return new WaitForSeconds(1.2f);
        loader.LoadScene("LoadingScene");
    }
}
