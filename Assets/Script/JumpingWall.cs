using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpingWall : MonoBehaviour {

    public Collider2D[] WallEdgeCollider;
    public Rigidbody2D WallRigidBody;

    // Use this for initialization
    void Start () {
        WallRigidBody = this.GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ActivateWall(bool active)
    {
        int i;
        for (i = 0; i < WallEdgeCollider.Length; i++)
        {
            if (active)
            {
                /*c = WallImage.color;
                c.a = 1;
                WallImage.color = c;*/
                WallEdgeCollider[i].isTrigger = false;
                this.transform.SetAsLastSibling();
            }
            else
            {
                /*c = WallImage.color;
                c.a = 0.25f;
                WallImage.color = c;*/
                WallEdgeCollider[i].isTrigger = true;
                //Debug.Log("Deactivate: " + WallRect);
            }
        }
    }
}
