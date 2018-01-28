using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour {

    public Rigidbody2D MovingRigidBody;
    List<Collider2D> WallEdgeColliders;
    public float ActualTime;
    bool KillingTrigger = false;
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

    bool playerInBounds;

    // Use this for initialization
    void Start () {
        int i;

        MovingRigidBody = this.GetComponent<Rigidbody2D>();

        if (this.GetComponent<PlayerKiller>() != null)
        {
            KillingTrigger = true;
        }

        WallEdgeColliders = new List<Collider2D>();
        for (i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).GetComponent<Collider2D>() != null)
            {
                WallEdgeColliders.Add(this.transform.GetChild(i).GetComponent<Collider2D>());
            }

            if (KillingTrigger)
            {
                WallEdgeColliders[i].isTrigger = true;
            }
        }
        if (this.GetComponent<Collider2D>() != null)
        {
            WallEdgeColliders.Add(this.GetComponent<Collider2D>());
            if (KillingTrigger)
            {
                WallEdgeColliders[WallEdgeColliders.Count-1].isTrigger = true;
            }
        }


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

        ActualTime = TimeShift % (TrackPoints[TrackPoints.Length - 1].Time + LoopTime);
        if (ActualTime < 0)
        {
            ActualTime = TrackPoints[TrackPoints.Length - 1].Time + LoopTime + ActualTime;
        }
        ActualSegment = GetSegment(ActualTime);
        //Debug.Log("StartingObstacleTime: " + ActualTime + ", Segment: " + ActualSegment + ", Segments: " + TrackPoints.Length);

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

    void SetTrigger(bool trigger)
    {
        int i;
        for (i = 0; i < WallEdgeColliders.Count; i++)
        {
            WallEdgeColliders[i].isTrigger = trigger;
        }

    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        if (LoopTime == 0 && ActualTime < TrackPoints[TrackPoints.Length - 1].Time && !KillingTrigger)
        {
            //Debug.Log("Is Player Inside moving object: " + playerInBounds); 
            if (!playerInBounds)
            {
                SetTrigger(false);
            }
            
        }
        if (ActualTime < TrackPoints[TrackPoints.Length - 1].Time && TrackPoints[ActualSegment + 1].Time - ActualTime != 0)
        {
            MovingRigidBody.velocity = (TrackPoints[ActualSegment + 1].TrackObject.transform.position - MovingRigidBody.transform.position) * 1000 / (TrackPoints[ActualSegment + 1].Time - ActualTime);
            //Debug.Log("ObstacleSegmentVector: " + (TrackPoints[ActualSegment + 1].TrackObject.transform.position - MovingRigidBody.transform.position) * 1000);
        }
        else if (TrackPoints[TrackPoints.Length - 1].Time + LoopTime - ActualTime != 0)
        {
            MovingRigidBody.velocity = (TrackPoints[0].TrackObject.transform.position - MovingRigidBody.transform.position) * 1000 / (TrackPoints[TrackPoints.Length - 1].Time + LoopTime - ActualTime);
        }

        //Debug.Log("ActualObstacleVelocity: " + MovingRigidBody.velocity);

        ActualTime += Time.deltaTime * 1000;

        if (ActualTime > TrackPoints[TrackPoints.Length - 1].Time + LoopTime)
        {
            ActualSegment = 0;
            ActualTime = ActualTime - (TrackPoints[TrackPoints.Length - 1].Time + LoopTime);
            //Debug.Log("EndOfObstacleLoop - time set to: " + ActualTime);
            if (LoopTime == 0)
            {
                //reset position
                SetTrigger(true);
                UnlinkPlayer();
                MovingRigidBody.MovePosition(TrackPoints[0].TrackObject.transform.position);



            } else
            {
                MovingRigidBody.MovePosition(TrackPoints[ActualSegment].TrackObject.transform.position);
            }
        }
        else if (ActualSegment < TrackPoints.Length - 1 && ActualTime > TrackPoints[ActualSegment + 1].Time)
        {
            ActualSegment += 1;
            MovingRigidBody.MovePosition(TrackPoints[ActualSegment].TrackObject.transform.position);
            //Debug.Log("ActualObstacleTime: " + ActualTime + " ActualSegment: " + ActualSegment);
        }
        //Debug.Log("ObstaclePosition: " + transform.forward * Time.deltaTime);
    }

    void UnlinkPlayer ()
    {
        int i;
        for (i = 0; i < MainScript.GetInstance().PlayersToFollow.Count; i++)
        {
            //Debug.Log("UnlinkingParent: " + this.gameObject);
            //Debug.Log(" LinkedParent: " + MainScript.GetInstance().PlayersToFollow[i].transform.parent);

            if (MainScript.GetInstance().PlayersToFollow[i].transform.parent != null && MainScript.GetInstance().PlayersToFollow[i].LinkedMovingPlatform.gameObject == this.gameObject)
            {
                MainScript.GetInstance().PlayersToFollow[i].SetJump(true);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        //Debug.Log("Rigid body inside moving object: " + coll.tag);
        if (coll.tag == "PlayerEnvelope")
        {
            playerInBounds = true;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.tag == "PlayerEnvelope")
        {
            playerInBounds = false;
        }
    }
}
