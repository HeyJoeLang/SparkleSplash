using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeModeListener : MonoBehaviour {

    public GazeModeEvent gazeModeEvent;

    private void OnEnable()
    {
        gazeModeEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        gazeModeEvent.UnregisterListener(this);
    }

    public virtual void GazeToTimedShot()
    {
    }

    public virtual void GazeToMagicBlaster()
    {
    }
}
