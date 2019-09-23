using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

    #region publicVariables
    
    public FadeEnvironment fade;

    #endregion
    #region privateVariables

    #endregion
    #region overrideFunction

    void Start()
    {
    }

    void Update()
    {

    }

#endregion
#region publicFunctions

    public void EndOfLevelReached()
    {
        StartCoroutine(EndLevel());
    }
    public void StartLevell()
    {
        StartCoroutine(StartLvl());
    }

#endregion
#region privateFunctions
    
    private IEnumerator StartLvl()
    {
        yield return new WaitForSeconds(2);
        GameObject.FindGameObjectWithTag("Player").GetComponent<MoveCameraForward>().initialSpeed = .05f;
    }
    private IEnumerator EndLevel()
    {
        yield return new WaitForSeconds(5);
        fade.FadeOut();
        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene(0);
    }


#endregion
}
