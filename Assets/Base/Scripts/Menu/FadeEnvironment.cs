using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeEnvironment : FadeMenu {

    public GameObject storyHeader, tutorialHeader, islandHeader;
    public Material skybox_island, skybox_artic, skybox_desert, skybox_tutorial, skybox_story;
    Material currentSelection;

	void Start (){
        currentSelection = skybox_island;
    }
    public void StorySelect(){
        SelectSkybox(skybox_story);
    }
    public void TutorialSelect(){
        SelectSkybox(skybox_tutorial);
    }
    public void IslandSelect(){
        SelectSkybox(skybox_island);
    }
    public void ArticSelect(){
        SelectSkybox(skybox_artic);
    }
    public void DesertSelect(){
        SelectSkybox(skybox_desert);
    }

    void SelectSkybox(Material mat){
        if (currentSelection != mat){
            currentSelection = mat;
            StartCoroutine(Fade());
        }
    }
    protected override IEnumerator Fade(){
        yield return StartCoroutine(base.Fade());
        RenderSettings.skybox = currentSelection;
        SwitchHeaders();
    }
    private void SwitchHeaders()
    {
        storyHeader.SetActive(false);
        tutorialHeader.SetActive(false);
        islandHeader.SetActive(false);
        if (currentSelection == skybox_island)
            islandHeader.SetActive(true);
        if (currentSelection == skybox_tutorial)
            tutorialHeader.SetActive(true);
        if (currentSelection == skybox_story)
            storyHeader.SetActive(true);
    }
}
