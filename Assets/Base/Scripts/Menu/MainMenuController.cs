using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    bool startTutorial = true;
    bool startStory = true;
    public GameObject tutorialOn, tutorialOff, storyOn, storyOff;
    public Image tutorialBackground, storyBackground;
    public Color on, off;

    public SceneLoader loader;
    public bool GetTutorial()
    {
        return startTutorial;
    }
    public void ToggleStory(bool toggle)
    {
        startStory = toggle;
        storyOn.SetActive(toggle);
        storyOff.SetActive(!toggle);
        storyBackground.color = toggle ? on : off;
    }
    public void ToggleTutorial(bool toggle)
    {
        startTutorial = toggle;
        tutorialOn.SetActive(toggle);
        tutorialOff.SetActive(!toggle);
        tutorialBackground.color = toggle ? on : off;
    }
    public void PlayGame()
    {
        string sceneToLoad = "Menu";
        if (startTutorial)
            sceneToLoad = "Tutorial";
        if (startStory)
        {
            sceneToLoad = "Story";
            DontDestroyOnLoad(this.gameObject);
        }
        loader.LoadScene(sceneToLoad);
    }
}
