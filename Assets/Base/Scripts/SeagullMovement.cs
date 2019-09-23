using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullMovement : MonoBehaviour {

    public float speed = .2f;
	void Start () {
		
	}
	
	void FixedUpdate () {
        transform.Rotate(transform.rotation.eulerAngles.x,  speed *-1, transform.rotation.eulerAngles.z);
	}
}
