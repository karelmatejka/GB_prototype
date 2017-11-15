using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArea : MonoBehaviour
{
    Player[] PlayersToFollow;

    float CamVertExtent;
    float CamHorzExtent;
    Vector2 BottomLeft;
    Vector2 TopRight;
    public Collider2D BoundingBoxCollider;

    // Use this for initialization
    void Start ()
    {
        int i;
        GameObject[] go;

        go = GameObject.FindGameObjectsWithTag("PlayerEnvelope");
        PlayersToFollow = new Player[go.Length];
        for (i = 0; i < go.Length; i++)
        {
            PlayersToFollow[i] = go[i].GetComponent<Player>();
        }

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
       
        for (i = 0; i < PlayersToFollow.Length; i++)
        {
            position = position + PlayersToFollow[i].transform.position;
        }
        position = position / PlayersToFollow.Length;
        position.z = -10;

        //Camera.main.CalculateFrustumCorners

        if (position.x < BottomLeft.x) position.x = BottomLeft.x;
        if (position.y < BottomLeft.y) position.y = BottomLeft.y;
        if (position.x > TopRight.x) position.x = TopRight.x;
        if (position.y > TopRight.y) position.y = TopRight.y;

        return position;

        //Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, position, ref velocity, 0.7f);

        //Camera.main.transform.position = Camera.main.transform.position + (position - Camera.main.transform.position) * 4 * Time.deltaTime;
        //Camera.main.transform.position = position;
    }
}
