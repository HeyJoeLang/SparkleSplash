using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GazeModeEvent : ScriptableObject
{
    private List<GazeModeListener> listeners = new List<GazeModeListener>();

    public float magicBlasterDuration = 30f;
    public enum GAZE_MODE
    {
        TIMED_SHOT,
        MAGIC_BLASTER
    };
    public GAZE_MODE gaze_state = GAZE_MODE.TIMED_SHOT;
   
    public virtual void GazeToTimedShot()
    {
        gaze_state = GAZE_MODE.TIMED_SHOT;
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].GazeToTimedShot();
    }

    public virtual void GazeToMagicBlaster()
    {
        gaze_state = GAZE_MODE.MAGIC_BLASTER;
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].GazeToMagicBlaster();
    }

    public void RegisterListener(GazeModeListener listener)
    {
        listeners.Add(listener);
    }
    public void UnregisterListener(GazeModeListener listener)
    {
        listeners.Remove(listener);
    }
}
