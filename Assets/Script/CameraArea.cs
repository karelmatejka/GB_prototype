using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraArea : MonoBehaviour
{
    float CamVertExtent;
    float CamHorzExtent;
    float defaultCamSize = 25;
    Vector2 BottomLeftRect;
    Vector2 TopRightRect;
    public Collider2D BoundingBoxCollider;

    // Use this for initialization
    void Start ()
    {
        MainScript.GetInstance();
        CamVertExtent = defaultCamSize;
        BottomLeftRect = BoundingBoxCollider.bounds.min;
        TopRightRect = BoundingBoxCollider.bounds.max;

        //Debug.Log("Cropping camera bounding box: " + BottomLeftRect + ", " + TopRightRect);
    }

    public Vector3 GetCroppedCameraPos(Vector3 position, Vector3 MaxDistance)
    {
        Vector2 BottomLeft;
        Vector2 TopRight;
        Vector2 NewCamRectangle = Vector3.zero;

        if (MaxDistance.y > defaultCamSize)
        {
            NewCamRectangle.y = MaxDistance.y;
            if (NewCamRectangle.y > (TopRightRect.y - BottomLeftRect.y) / 2)
            {
                NewCamRectangle.y = (TopRightRect.y - BottomLeftRect.y) / 2;
            }

            NewCamRectangle.x = NewCamRectangle.y * Camera.main.aspect;

            if (NewCamRectangle.x > (TopRightRect.x - BottomLeftRect.x) / 2)
            {
                NewCamRectangle.x = (TopRightRect.x - BottomLeftRect.x) / 2;
                NewCamRectangle.y = NewCamRectangle.x / Camera.main.aspect;
                //Debug.Log("Camera Vertically cropped");
            }
        }
        else
        {
            NewCamRectangle.y = defaultCamSize;
            NewCamRectangle.x = Camera.main.aspect * NewCamRectangle.y;
        }



        if (MaxDistance.x > defaultCamSize * Camera.main.aspect && NewCamRectangle.y >= defaultCamSize)
        {
            NewCamRectangle.x = MaxDistance.x;
            if (NewCamRectangle.x > (TopRightRect.x - BottomLeftRect.x) / 2)
            {
                NewCamRectangle.x = (TopRightRect.x - BottomLeftRect.x) / 2;
            }
            NewCamRectangle.y = NewCamRectangle.x / Camera.main.aspect;

            if (NewCamRectangle.y > (TopRightRect.y - BottomLeftRect.y) / 2 && NewCamRectangle.y > defaultCamSize)
            {
                NewCamRectangle.y = (TopRightRect.y - BottomLeftRect.y) / 2;
                NewCamRectangle.x = NewCamRectangle.x * Camera.main.aspect;
                //Debug.Log("Camera Vertically cropped");
            }
        }

        //NewCamRectangle.y = defaultCamSize;

        //CamVertExtent = Camera.main.orthographicSize + (Camera.main.orthographicSize - NewCamRectangle.y) * Time.deltaTime;
        CamVertExtent = NewCamRectangle.y;
        //Debug.Log("Camera size: " + CamVertExtent);
        Camera.main.orthographicSize = CamVertExtent;
        CamHorzExtent = Camera.main.aspect * Camera.main.orthographicSize;

        position = position / MainScript.GetInstance().PlayersToFollow.Count;
        position.z = -200;

        BottomLeft = BottomLeftRect;
        TopRight = TopRightRect;

        BottomLeft.x = BottomLeft.x + CamHorzExtent;
        BottomLeft.y = BottomLeft.y + CamVertExtent;

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


        //Camera.main.CalculateFrustumCorners

        if (position.x < BottomLeft.x) position.x = BottomLeft.x;
        if (position.y < BottomLeft.y) position.y = BottomLeft.y;
        if (position.x > TopRight.x) position.x = TopRight.x;
        if (position.y > TopRight.y) position.y = TopRight.y;

        return position;
    }


    public Vector3 GetCameraPos()
    {
        int i;
        Vector3 MaxDistance = Vector3.zero;
        List<float> x;
        List<float> y;

        x = new List<float>();
        y = new List<float>();

        Vector3 position = Vector3.zero;

        for (i = 0; i < MainScript.GetInstance().PlayersToFollow.Count; i++)
        {
            position = position + MainScript.GetInstance().PlayersToFollow[i].transform.position;

            x.Add(MainScript.GetInstance().PlayersToFollow[i].transform.position.x);
            y.Add(MainScript.GetInstance().PlayersToFollow[i].transform.position.y);

            //Debug.Log("Player Position[" + i + "]: " + x[i] + ", " + y[i]);

        }
        if (MainScript.GetInstance().PlayersToFollow.Count == 0)
        {
            //return Vector3.zero;
            position = MainScript.GetInstance().LoaderInstance.LevelDefinitions[MainScript.GetInstance().LoaderInstance.ActiveLevelId].StartingPosition.transform.position;
            x.Add(MainScript.GetInstance().LoaderInstance.LevelDefinitions[MainScript.GetInstance().LoaderInstance.ActiveLevelId].StartingPosition.transform.position.x);
            y.Add(MainScript.GetInstance().LoaderInstance.LevelDefinitions[MainScript.GetInstance().LoaderInstance.ActiveLevelId].StartingPosition.transform.position.y);
        }

        MaxDistance.x = (x.Max() - x.Min()) / 2 + 12;
        MaxDistance.y = (y.Max() - y.Min()) / 2 + 12;

        //Debug.Log("Max Distance Distance Between Players: " + MaxDistance + " Players: " + x.Count);

        return GetCroppedCameraPos(position, MaxDistance);
    }

        
}
