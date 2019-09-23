using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerScoreEvent : ScriptableObject {

    public int score = 0;
    private List<ScoreChangeListener> listeners = new List<ScoreChangeListener>();

    public void Raise( int amount)
    {
        score += amount;
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnScoreRaised();
    }

    public void RegisterListener(ScoreChangeListener listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(ScoreChangeListener listener)
    {
        listeners.Remove(listener);
    }
}
