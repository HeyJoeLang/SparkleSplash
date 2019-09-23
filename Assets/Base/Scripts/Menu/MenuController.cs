using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
    
    public void LoadStory()
    {
        StartCoroutine("LoadLVLStory");
    }
    public void LoadIsland()
    {
        StartCoroutine("LoadLVLIsland");
    }

    IEnumerator LoadLVLIsland()
    {
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene("LoadingScene");
    }
    IEnumerator LoadLVLStory()
    {
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene("Story");
    }
}
