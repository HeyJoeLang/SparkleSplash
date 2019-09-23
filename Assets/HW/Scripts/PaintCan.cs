using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintCan : MonoBehaviour {
    //public Vector3 axis;
    //public float speed = .1f;
    public Color color; 

	void Start () {
        if(gameObject.GetComponent<Renderer>() != null)
            gameObject.GetComponent<Renderer>().material.color = color; 
	}
    void Update()
    {
        //transform.Rotate(axis, Time.deltaTime * speed);
    }
}
