using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public Texture walkEmissionTexture;
    public Texture dontwalkEmissionTexture;
    public Texture StopEmissionTexture;
    public Texture WarnEmissionTexture;
    public Texture GoEmissionTexture;
    public GameObject redParticles;
    public GameObject yellowParticles;
    public GameObject greenParticles;

    public bool lightsEnabled = true; // You can disable a light if requried.
    public float flashInterval = 1.0f; // When lights are flashing this value is the flash period in seconds.

    private Material mat;
    private float timer;
    private bool toggleOn = true;
    private PhaseState currentState;

    void Start() {
        mat = GetComponent<Renderer>().material;
        timer = flashInterval;
    }

    void Update() {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            timer = flashInterval;
            toggleOn = !toggleOn;
            ApplyState(currentState);
        }
    }

    void ApplyEmissionTexture(Texture texture) {
        mat.SetColor("_EmissionColor", lightsEnabled ? Color.white : Color.black); // Use this to turn off the lights as necessary
        mat.EnableKeyword("_EMISSION");
    }

    void ApplyState(PhaseState newState) {
        currentState = newState;
        lightsEnabled = true;
        switch (newState) {
            case PhaseState.Walk:
                if (walkEmissionTexture != null) {
                    ApplyEmissionTexture(walkEmissionTexture);
                }
                break;
            case PhaseState.DontWalk:
                if (dontwalkEmissionTexture != null) {
                    ApplyEmissionTexture(dontwalkEmissionTexture);
                }
                break;
            case PhaseState.Stop:
                redParticles.SetActive(true);
                yellowParticles.SetActive(false);
                greenParticles.SetActive(false);
                if (StopEmissionTexture != null) {
                    ApplyEmissionTexture(StopEmissionTexture);
                }
                break;
            case PhaseState.Warn:
                redParticles.SetActive(false);
                yellowParticles.SetActive(true);
                greenParticles.SetActive(false);
                if (WarnEmissionTexture != null) {
                    ApplyEmissionTexture(WarnEmissionTexture);
                }
                break;
            case PhaseState.Flash:
                if (!toggleOn) {
                    lightsEnabled = false;
                }
                if (WarnEmissionTexture != null) {
                    ApplyEmissionTexture(WarnEmissionTexture);
                }
                break;
            case PhaseState.Go:
                redParticles.SetActive(false);
                yellowParticles.SetActive(false);
                greenParticles.SetActive(true);
                if (GoEmissionTexture != null) {
                    ApplyEmissionTexture(GoEmissionTexture);
                }
                break;
            case PhaseState.Off:
                lightsEnabled = false;
                if (StopEmissionTexture != null) {
                    ApplyEmissionTexture(StopEmissionTexture);
                }
                break;
            default:
                Debug.Log("Unknown Traffic Light State");
                break;
        }
    }
}