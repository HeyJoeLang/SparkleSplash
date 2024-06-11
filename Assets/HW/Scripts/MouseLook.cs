using System.Collections;
using UnityEngine;

public class MouseLook : MonoBehaviour {

    public float sensitivityX = 4;
    public float sensitivityY = 2; 

    private float rotationY = 0f;
    private float minimumY = -60f;
    private float maximumY = 60f;
    public GazeModeEvent gaze;

    void Update()
    {
        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

        rotationY += (Input.GetAxis("Mouse Y") * sensitivityY);
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine("StartMagicBlaster");
        }
    }
    IEnumerator StartMagicBlaster()
    {
        gaze.GazeToMagicBlaster();
        yield return new WaitForSeconds(gaze.magicBlasterDuration);
        gaze.GazeToTimedShot();
    }
}
