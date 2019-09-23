using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TI_FadeLoadMenu : MonoBehaviour {

    public FadeTutorial fade;
    private void OnEnable()
    {
        fade.FadeLoadScene("Menu");
    }
}
