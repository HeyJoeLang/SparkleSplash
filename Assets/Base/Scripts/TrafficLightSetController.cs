using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PhaseState { Walk, DontWalk, Stop, Warn, Go, Off, Flash }

public class TrafficLightSetController : MonoBehaviour
{
    public PhaseState state; // Set this dropdown from the properties panel for the desired traffic light state.

    private PhaseState lastState = PhaseState.Off;

    void Update()
    {

        if (state != lastState)
        {

            switch (state)
            {
                case PhaseState.Walk:
                    lastState = PhaseState.Walk;
                    break;
                case PhaseState.DontWalk:
                    lastState = PhaseState.DontWalk;
                    break;
                case PhaseState.Stop:
                    lastState = PhaseState.Stop;
                    break;
                case PhaseState.Warn:
                    lastState = PhaseState.Warn;
                    break;
                case PhaseState.Go:
                    lastState = PhaseState.Go;
                    break;
                case PhaseState.Off:
                    lastState = PhaseState.Off;
                    break;
                case PhaseState.Flash:
                    lastState = PhaseState.Flash;
                    break;
                default:
                    // DebugLog("Unknown State");
                    break;
            }
            NotifyState(state);
        }
    }

    void NotifyState(PhaseState newState)
    {
        BroadcastMessage("ApplyState", newState, SendMessageOptions.DontRequireReceiver);
    }
}