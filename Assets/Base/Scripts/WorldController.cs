using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public GameObject[] blocks;
    private float blockDeltaX = 120;
    float movementDelta = 0;
    float speed = -5;
    bool isMoving = false;

    private void Start()
    {
        blockDeltaX = blocks[blocks.Length - 1].transform.position.x;
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
                    blocks[i].GetComponent<BlockController>().ResetAnimals();
                    blocks[i].transform.position += new Vector3(-480, 0, 0);
                }
            }
        }
    }
    public void NewBlock()
    {
    }
    void DisableEnvironmentBlock(GameObject block)
    {
        block.SetActive(false);
    }
    void Update()
    {
    }
}