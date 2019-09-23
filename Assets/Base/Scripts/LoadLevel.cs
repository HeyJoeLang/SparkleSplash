using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour {


    public Scrollbar unicornBar;
    public Scrollbar rainbowBar;
    public FadeEnvironment fade;
    public string level;

    void Start()
    {
        LoadLVL(level);
    }
    public void LoadLVL(string lvlName)
    {
        StartCoroutine(LoadAsync(lvlName));
    }
    IEnumerator LoadAsync(string lvlName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(lvlName);
        while(!operation.isDone)
        {
            float progress = operation.progress;
            unicornBar.value = progress;
            rainbowBar.size = progress;
            //Debug.Log(progress);
            yield return null;
        }

    }
}
