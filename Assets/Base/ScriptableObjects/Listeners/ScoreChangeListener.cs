using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreChangeListener : GazeModeListener {

    public PlayerScoreEvent playerScoreEvent;

    private void OnEnable()
    {
        playerScoreEvent.RegisterListener(this);
    }
    private void OnDisable()
    {
        playerScoreEvent.UnregisterListener(this);
    }
    public virtual void OnScoreRaised()
    {

    }
}
