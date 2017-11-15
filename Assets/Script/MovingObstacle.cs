using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour {

    Rigidbody2D MovingRigidBody;
    public float ActualTime;
    int ActualSegment;
    public float TimeShift;
    public float LoopTime;
    [System.Serializable]
    public struct TrackPoint
    {
        public GameObject TrackObject;
        public float Time;
    }
    public TrackPoint[] TrackPoints;

	// Use this for initialization
	void Start () {
        InitMovingObstacle();
    }

    int GetSegment(float time)
    {
        int i;
        int segmentFound;
        segmentFound = TrackPoints.Length - 1;
        for (i = 0; i < TrackPoints.Length - 1; i++)
        {
            if (time >= TrackPoints[i].Time && time < TrackPoints[i + 1].Time)
            {
                segmentFound = i;
            }
        }
            return segmentFound;
    }

    public void InitMovingObstacle()
    {
        int i;
        float percentage;
        MovingRigidBody = this.GetComponent<Rigidbody2D>();
        ActualTime = TimeShift % (TrackPoints[TrackPoints.Length - 1].Time + LoopTime);
        if (ActualTime < 0)
        {
            ActualTime = TrackPoints[TrackPoints.Length - 1].Time + LoopTime + ActualTime;
        }
        ActualSegment = GetSegment(ActualTime);
        Debug.Log("StartingObstacleTime: " + ActualTime + " Segment: " + ActualSegment);

        for (i = 0; i < TrackPoints.Length; i++)
        {
            TrackPoints[i].TrackObject.SetActive(false);
        }

        if (ActualTime < TrackPoints[TrackPoints.Length - 1].Time)
        {
            percentage = (ActualTime - TrackPoints[ActualSegment].Time) / (TrackPoints[ActualSegment + 1].Time - TrackPoints[ActualSegment].Time);
            MovingRigidBody.MovePosition(TrackPoints[ActualSegment].TrackObject.transform.position + percentage * (TrackPoints[ActualSegment + 1].TrackObject.transform.position - TrackPoints[ActualSegment].TrackObject.transform.position));
        }
        else
        {
            percentage = (ActualTime - TrackPoints[ActualSegment].Time) / (TrackPoints[TrackPoints.Length - 1].Time + LoopTime - TrackPoints[ActualSegment].Time);
            MovingRigidBody.MovePosition(TrackPoints[ActualSegment].TrackObject.transform.position + percentage * (TrackPoints[ActualSegment].TrackObject.transform.position - TrackPoints[0].TrackObject.transform.position));
        }

    }

	// Update is called once per frame
	void FixedUpdate ()
    {

        if (ActualTime < TrackPoints[TrackPoints.Length - 1].Time && TrackPoints[ActualSegment + 1].Time - ActualTime !=0)
        {
            MovingRigidBody.velocity = (TrackPoints[ActualSegment + 1].TrackObject.transform.position - MovingRigidBody.transform.position) / (TrackPoints[ActualSegment + 1].Time - ActualTime) * 1000;

            //Debug.Log("TrackPercent " + trackPercent + "Position: " + TrackPoints[ActualSegment].TrackObject.transform.position + (TrackPoints[ActualSegment + 1].TrackObject.transform.position - TrackPoints[ActualSegment].TrackObject.transform.position) * trackPercent);
            //Debug.Log("Moving With Platform - Speed: " + MovingRigidBody.velocity);
        }
        else if (TrackPoints[TrackPoints.Length - 1].Time + LoopTime - ActualTime != 0)
        {
            MovingRigidBody.velocity = (TrackPoints[0].TrackObject.transform.position - MovingRigidBody.transform.position) / (TrackPoints[TrackPoints.Length - 1].Time + LoopTime - ActualTime) * 1000;
            //Debug.Log("Looping Segment " + MovingRigidBody);
        }
        ActualTime += Time.deltaTime * 1000;

        if (ActualTime > TrackPoints[TrackPoints.Length - 1].Time + LoopTime)
        {
            ActualSegment = 0;
            ActualTime = ActualTime - (TrackPoints[TrackPoints.Length - 1].Time + LoopTime);
            Debug.Log("EndOfObstacleLoop - time set to: " + ActualTime);
            if (LoopTime == 0)
            {
                MovingRigidBody.MovePosition(TrackPoints[0].TrackObject.transform.position);
            }
        }
        else if (ActualSegment < TrackPoints.Length - 1 && ActualTime > TrackPoints[ActualSegment + 1].Time)
        {
            ActualSegment += 1;
            
            //Debug.Log("ActualObstacleTime: " + ActualTime + " ActualSegment: " + ActualSegment);
        }
        //Debug.Log("ObstaclePosition: " + transform.forward * Time.deltaTime);
    }
}
