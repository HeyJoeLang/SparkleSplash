using System.Collections;
using UnityEngine;

[System.Serializable]
public class PageContent
{
    public float prePageDelay;
    public AudioClip pageVocals;
}
public class StoryController : MonoBehaviour {
    public FadeLoad fade;
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
        fade.FadeOutLoadScene(2);
    }
}
