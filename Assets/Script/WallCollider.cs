using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallCollider : MonoBehaviour {

    public Collider2D[] WallEdgeCollider;
    public bool Disappears;
    public Rigidbody2D WallRigidBody;
    //public RectTransform WallRect;
    //public Image WallImage;

    // Use this for initialization
    void Start () {

        float sizeX;
        float sizeY;

        //WallRect = this.GetComponent<RectTransform>();
        WallRigidBody = this.GetComponent<Rigidbody2D>();

/*        Vector2[] tempArray1 = WallEdgeCollider[0].points;
        Vector2[] tempArray2 = WallEdgeCollider[1].points;
        Vector2[] tempArray3 = WallEdgeCollider[2].points;
        Vector2[] tempArray4 = WallEdgeCollider[3].points;

        Vector2[] tempArray5 = WallEdgeCollider[4].points;
        Vector2[] tempArray6 = WallEdgeCollider[5].points;
        Vector2[] tempArray7 = WallEdgeCollider[6].points;
        Vector2[] tempArray8 = WallEdgeCollider[7].points;

        sizeX = WallRect.rect.width / 2 - 0.3f;
        sizeY = WallRect.rect.height / 2 - 0.3f;
        
        tempArray1[0].x = -sizeX;
        tempArray1[0].y = sizeY;
        tempArray1[1].x = sizeX;
        tempArray1[1].y = sizeY;
        WallEdgeCollider[0].points = tempArray1;

        tempArray2[0].x = sizeX;
        tempArray2[0].y = sizeY;
        tempArray2[1].x = sizeX;
        tempArray2[1].y = -sizeY;
        WallEdgeCollider[1].points = tempArray2;

        tempArray3[0].x = sizeX;
        tempArray3[0].y = -sizeY;
        tempArray3[1].x = -sizeX;
        tempArray3[1].y = -sizeY;
        WallEdgeCollider[2].points = tempArray3;

        tempArray4[0].x = -sizeX;
        tempArray4[0].y = -sizeY;
        tempArray4[1].x = -sizeX;
        tempArray4[1].y = sizeY;
        WallEdgeCollider[3].points = tempArray4;

        sizeX = WallRect.rect.width / 2 - 0.4f;
        sizeY = WallRect.rect.height / 2 - 0.4f;

        tempArray5[0].x = -sizeX;
        tempArray5[0].y = sizeY;
        tempArray5[1].x = sizeX;
        tempArray5[1].y = sizeY;
        WallEdgeCollider[4].points = tempArray5;

        tempArray6[0].x = sizeX;
        tempArray6[0].y = sizeY;
        tempArray6[1].x = sizeX;
        tempArray6[1].y = -sizeY;
        WallEdgeCollider[5].points = tempArray6;

        tempArray7[0].x = sizeX;
        tempArray7[0].y = -sizeY;
        tempArray7[1].x = -sizeX;
        tempArray7[1].y = -sizeY;
        WallEdgeCollider[6].points = tempArray7;

        tempArray8[0].x = -sizeX;
        tempArray8[0].y = -sizeY;
        tempArray8[1].x = -sizeX;
        tempArray8[1].y = sizeY;
        WallEdgeCollider[7].points = tempArray8;
*/
        //ActivateWall(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ActivateWall(bool active)
    {
        int i;
        Color c;

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
                if (Disappears) WallEdgeCollider[i].isTrigger = true;
                //Debug.Log("Deactivate: " + WallRect);
            }
        }
    }
}
