using UnityEngine;

public class MoveCameraForward : MonoBehaviour {

    #region variables
    public enum MOVEMENT_TYPE { MOVING, SLOWING_DOWN, STOPPED }
    public MOVEMENT_TYPE movementState = MOVEMENT_TYPE.MOVING;
    public Vector3 direction;
    public float initialSpeed = 0;
    private float initialSlowDownTime;
    private float currentSpeed;
    public Transform endGoal;
    Vector3 startPos;

    #endregion
    #region publicFunctions
    
    public void SlowToHalt()
    {
        movementState = MOVEMENT_TYPE.SLOWING_DOWN;
        initialSlowDownTime = Time.time;
    }

    #endregion
    #region overrideFunctions
    private void Start()
    {
        startPos = transform.position;
        currentSpeed = initialSpeed;
    }

    void FixedUpdate () {
        switch(movementState)
        {
            case MOVEMENT_TYPE.MOVING:
                currentSpeed = initialSpeed * .75f;
                break;
            case MOVEMENT_TYPE.SLOWING_DOWN:
                currentSpeed = initialSpeed * (1 - (Time.time - initialSlowDownTime));
                    if (currentSpeed <= .1f)
                    movementState = MOVEMENT_TYPE.STOPPED;
                break;
            case MOVEMENT_TYPE.STOPPED:
                currentSpeed = 0f;
                break;
        }
        transform.position += new Vector3(-1 * currentSpeed , 0, 0);

    }

    #endregion
}
