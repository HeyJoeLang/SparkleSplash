using UnityEngine;

public class WorldController : MonoBehaviour
{
    float movementDelta = 0;
    float speed = -5;
    bool isMoving = false;
    BlockController[] blocks;

    private void Start()
    {
        blocks = FindObjectsOfType<BlockController>();
    }

    public void StartMovement()
    {
        isMoving = true;
        movementDelta = 0;
    }
    private void FixedUpdate()
    {
        movementDelta = Mathf.Lerp(movementDelta, speed, Time.deltaTime);
        if (isMoving)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].transform.Translate(Vector3.left * movementDelta * Time.deltaTime, Space.Self);
                if (blocks[i].transform.position.x > 150f)
                {
                    blocks[i].ResetAnimals();
                    blocks[i].transform.position += new Vector3(-480, 0, 0);
                }
            }
        }
    }
}