using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteImageOnStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SpriteRenderer sprite;
        sprite = this.GetComponent<SpriteRenderer>();
        sprite.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
