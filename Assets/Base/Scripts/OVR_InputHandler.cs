using System.Collections;
using UnityEngine;

public class OVR_InputHandler : MonoBehaviour {

    public Transform cam, cameraParent;
    [SerializeField]
    private OVRInput.RawButton resetButton = OVRInput.RawButton.Back;

    public GazeModeEvent gaze;

    private void Start()
    {
        gaze.gaze_state = GazeModeEvent.GAZE_MODE.TIMED_SHOT;
        OVRTouchpad.Create();
        OVRTouchpad.AddListener(LocalTouchEventCallback);
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && Application.isEditor)
            StartCoroutine("StartMagicBlaster");
        if (Input.GetKeyDown(KeyCode.W) && Application.isEditor)
            gaze.GazeToTimedShot();
        if(Input.GetKeyDown(KeyCode.E) && Application.isEditor)
            cameraParent.transform.Rotate(0, -cam.transform.eulerAngles.y - 90, 0);
    }
    private void LocalTouchEventCallback(OVRTouchpad.TouchEvent button)
    {
        switch (button)
        {
            case OVRTouchpad.TouchEvent.Right:
                break;

            case OVRTouchpad.TouchEvent.Left:
                StartCoroutine("StartMagicBlaster");
                break;

            case OVRTouchpad.TouchEvent.Up:
                cameraParent.transform.Rotate(0,-cam.transform.eulerAngles.y- 90,0);
                break;

            case OVRTouchpad.TouchEvent.Down:
                break;
        }
    }
    IEnumerator StartMagicBlaster()
    {
        gaze.GazeToMagicBlaster();
        yield return new WaitForSeconds(gaze.magicBlasterDuration);
        gaze.GazeToTimedShot();
    }
}
