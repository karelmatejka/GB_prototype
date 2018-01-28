using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour {
    Rigidbody2D rigidBodyGrass;
	// Use this for initialization
	void Start () {
        rigidBodyGrass = GetComponent<Rigidbody2D>();

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        rigidBodyGrass.AddTorque(-this.transform.localRotation.z, ForceMode2D.Impulse);
    }
}
