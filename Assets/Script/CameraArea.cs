using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArea : MonoBehaviour
{
    float CamVertExtent;
    float CamHorzExtent;
    Vector2 BottomLeft;
    Vector2 TopRight;
    public Collider2D BoundingBoxCollider;

    // Use this for initialization
    void Start ()
    {
        MainScript.GetInstance();

        CamVertExtent = Camera.main.orthographicSize;
        CamHorzExtent = Camera.main.aspect * CamVertExtent;
        Debug.Log(CamHorzExtent + ", " + CamVertExtent);

        BottomLeft = BoundingBoxCollider.bounds.min;
        BottomLeft.x = BottomLeft.x + CamHorzExtent;
        BottomLeft.y = BottomLeft.y + CamVertExtent;
        TopRight = BoundingBoxCollider.bounds.max;
        TopRight.x = TopRight.x - CamHorzExtent;
        TopRight.y = TopRight.y - CamVertExtent;
        if (TopRight.x < BottomLeft.x)
        {
            TopRight.x = BoundingBoxCollider.bounds.center.x;
            BottomLeft.x = TopRight.x;
        }
        if (TopRight.y < BottomLeft.y)
        {
            TopRight.y = BoundingBoxCollider.bounds.center.y;
            BottomLeft.y = TopRight.y;
        }
        Debug.Log(BottomLeft + ", " + TopRight);
    }

    public Vector3 GetCameraPos()
    {
        int i;
        Vector3 position = Vector3.zero;
       
        for (i = 0; i < MainScript.GetInstance().PlayersToFollow.Length; i++)
        {
            position = position + MainScript.GetInstance().PlayersToFollow[i].transform.position;
        }
        position = position / MainScript.GetInstance().PlayersToFollow.Length;
        position.z = -10;

        //Camera.main.CalculateFrustumCorners

        if (position.x < BottomLeft.x) position.x = BottomLeft.x;
        if (position.y < BottomLeft.y) position.y = BottomLeft.y;
        if (position.x > TopRight.x) position.x = TopRight.x;
        if (position.y > TopRight.y) position.y = TopRight.y;

        return position;
    }
}
