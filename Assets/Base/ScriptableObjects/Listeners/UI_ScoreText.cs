using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UI_ScoreText : ScoreChangeListener
{

    private Text text;
    void Start()
    {
        text = GetComponent<Text>();
    }
    public override void OnScoreRaised()
    {
        base.OnScoreRaised();
        UpdateScoreBoard();
    }
    void UpdateScoreBoard()
    {
   //     Debug.Log("Player Score : " + playerScoreEvent.score);
        text.text = playerScoreEvent.score.ToString("#,000");
    }

}
