using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public float speed;
    Rigidbody2D RibidBodyInstance;
	// Use this for initialization
	void Start () {
        RibidBodyInstance = this.GetComponent<Rigidbody2D>();

    }
	
	// Update is called once per frame
	void Update () {
        RibidBodyInstance.velocity = speed * Vector3.up;
	}
}
