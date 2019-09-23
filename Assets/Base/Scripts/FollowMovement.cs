using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMovement : MonoBehaviour {

    public Transform player;
    Vector3 delta;

	void Start () {
        delta = transform.position - player.position;
	}

	void Update () {
        transform.position = player.position + delta;
	}
}
