using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public float speed = -1;
    private bool isMoving = false;
    float movementDelta;

    public void StartMovement()
    {
        isMoving = true;
        movementDelta = 0;
    }
	void FixedUpdate()
    {
        movementDelta = Mathf.Lerp(movementDelta, speed, Time.deltaTime);
        if (isMoving)
           transform.Translate(Vector3.forward * movementDelta * Time.deltaTime, Space.Self);
    }
}
