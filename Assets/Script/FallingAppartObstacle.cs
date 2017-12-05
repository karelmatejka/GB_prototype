using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingAppartObstacle : MonoBehaviour {

    [System.Serializable]
    public struct FallingAppartObject
    {
        public GameObject Visual;
        public float Time;
    }
    public FallingAppartObject[] FallingAppartObjects;

    bool IsInCollision;
    float ActualTime;
    [HideInInspector] public int ActualSegment;
    Collider2D WallEdgeCollider;
    public bool Reappear;
    public float ReappearTime;
    bool playerInBounds;

    // Use this for initialization
    void Start () {
        WallEdgeCollider = this.GetComponent<Collider2D>();
        InitFallingAppartObstacle();
    }

    void InitFallingAppartObstacle()
    {
        ActualTime = 0;
        ActualSegment = 0;
        SetVisual(0);
        IsInCollision = false;
        WallEdgeCollider.isTrigger = false;
    }
	// Update is called once per frame
	void Update () {
        if (ActualSegment == -1)
        {
            ActualTime += Time.deltaTime * 1000;
            //Debug.Log("Is player Inside falling appart object: " + playerInBounds);
            if (!playerInBounds && ActualTime > ReappearTime && Reappear)
            {
                InitFallingAppartObstacle();
            }
            return;
        }
        if (IsInCollision)
        {
            ActualTime += Time.deltaTime * 1000;
            //Debug.Log("Actual Falling Appart Time: " + ActualTime);
            if (ActualTime > FallingAppartObjects[ActualSegment].Time)
            {
                if (ActualSegment < FallingAppartObjects.Length - 1)
                {
                    ActualSegment += 1;
                    SetVisual(ActualSegment);
                }
                else
                {
                    UnlinkPlayer();
                }

            }
        }
    }
    void SetVisual(int index)
    {
        int i;
        for (i = 0; i < FallingAppartObjects.Length; i++)
        {
            FallingAppartObjects[i].Visual.SetActive(false);
        }
        if (index != -1)
        {
            FallingAppartObjects[index].Visual.SetActive(true);
        }
    }

    void UnlinkPlayer()
    {
        int i;
        ActualSegment = -1;
        ActualTime = 0;
        SetVisual(-1);
        WallEdgeCollider.isTrigger = true;
        playerInBounds = true;

        for (i = 0; i < MainScript.GetInstance().PlayersToFollow.Count; i++)
        {
            if (MainScript.GetInstance().PlayersToFollow[i].transform.parent != null && MainScript.GetInstance().PlayersToFollow[i].transform.parent.gameObject == this.gameObject)
            {
                MainScript.GetInstance().PlayersToFollow[i].SetJump(true);
                //Debug.Log("Unlink Successful: " + MainScript.GetInstance().PlayersToFollow[i].transform.parent.gameObject);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.tag == "PlayerEnvelope")
        {
            IsInCollision = true;
        }
    }
    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.transform.tag == "PlayerEnvelope")
        {
            IsInCollision = false;
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
