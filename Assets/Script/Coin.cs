using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    public Animator anim;
	// Use this for initialization
	void Start ()
    {
        float startPoint = Random.Range(0f, 1f);
        anim.Play("CoinAnim", -1, startPoint);
    }

  

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerEnvelope")
        {
            Destroy(this.gameObject);
        }
    }

}
