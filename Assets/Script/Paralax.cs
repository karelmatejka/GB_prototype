using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour {

    public float ParalaxMultiplier;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos;
        pos = Camera.main.transform.position *  ParalaxMultiplier;
        pos.z = 0;
        transform.position = pos;
	}
}
