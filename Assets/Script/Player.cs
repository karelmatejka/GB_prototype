using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public struct ImpactCheck
    {
        public float distance;
        public Vector2 gravity;
    }

    public Animator PlayerAnim;
    public GameObject Shadow;
    public JumpingWall[] JumpingWalls;

    CameraArea CameraAreaScript;

    public Image PlayerImage;
    public int ControllerIndex;
    Vector2 ActualGravity;
    Vector2 ComputedGravityVector;
    Vector2 GravityAndJump;

    Rigidbody2D PlayerRigidBody;    

    float SpeedConstant;
    float GravityConstant;
    float MaxSpeed;
    float JumpConstant;
    int MaxTrackingRays;

    public bool PlayerInCollision;
    public bool Jumping;
    float JumpingTime;
    float JumpingTimeMinus;
    public bool Walking;

    Vector2 joypad;
    Vector2 lastjoypad;
    int ButtonFire;

    // Use this for initialization
    void Start()
    {
        InitPlayer();
    }

    void InitPlayer()
    {
        int i;
        GameObject[] go;

        CameraAreaScript = GameObject.FindObjectOfType(typeof(CameraArea)) as CameraArea;

        go = GameObject.FindGameObjectsWithTag("JumpingColliders");
        JumpingWalls = new JumpingWall[go.Length];
        for (i = 0; i < go.Length; i++)
        {
            JumpingWalls[i] = go[i].GetComponent<JumpingWall>();
        }

        ActualGravity.x = 0;
        ActualGravity.y = 0;
        MaxTrackingRays = 128;
        SpeedConstant = 14;
        GravityConstant = 60f;
        JumpConstant = 23;
        MaxSpeed = 30;

        ComputedGravityVector = Vector2.down * GravityConstant;

        PlayerInCollision = false;
        Jumping = true;
        JumpingTimeMinus = -0.25f;
        JumpingTime = JumpingTimeMinus;
        Walking = false;

        PlayerRigidBody = this.GetComponent<Rigidbody2D>();
        PlayerImage = this.GetComponent<Image>();

        SetJumpingWallCollidersActive(true, null);
    }

    Vector2 ProjectedVectorToSurface(Vector2 direction, Vector2 gravity)
    {
        Vector2 finalvector;

        float param;

        param = (direction.x * gravity.y - direction.y * gravity.x) / (gravity.x * gravity.x + gravity.y * gravity.y);

        finalvector.x = gravity.y * param;
        finalvector.y = - gravity.x * param;

        return finalvector;
    }

    void Update()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 cross;

        GetControls();

        Vector2 dir = ComputedGravityVector.normalized * GravityConstant;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime*20);


        /*--follow move direction--*/
        //cross = Vector3.Cross(dir, ProjectedVectorToSurface(lastjoypad, dir));
        //transform.localScale = new Vector3(transform.localScale.x, Mathf.Sign(cross.z) * transform.localScale.x, transform.localScale.z);

        setAnimState();
    }

    void FixedUpdate()
    {
        Vector3 velocity = Vector3.zero;
        float delta;
        Vector2 movementVector;

        delta = Time.deltaTime;

        movementVector = Vector2.zero;


        ComputedGravityVector = CheckGravityAround();

        /*

        if (PlayerInCollision)
        {
            movementVector = ProjectedVectorToSurface(joypad , ComputedGravityVector) * SpeedConstant;

            Debug.DrawRay(this.transform.position, movementVector/1000);

            //Debug.Log("MovementVector: " + movementVector + " Gravity: " + ComputedGravityVector);

            PlayerRigidBody.AddForce(movementVector * delta);
            
            //Shadow.SetActive(true);
        }
        else
        {
            //Shadow.SetActive(false);
            if (Jumping)
            {
                gravityMultiplier = JumpGravityReductionConstant;
                //PlayerRigidBody.AddForce(LastMovementVector * delta / 8);
                LastMovementVector = LastMovementVector * delta * 20;
            }
            else
            {
                //PlayerRigidBody.AddForce(LastMovementVector * delta / 2);
                LastMovementVector = LastMovementVector * delta;
                Debug.Log("Out but not jumping");
            }

            

            //movementVector = ProjectedVectorToSurface(joypad, ComputedGravityVector) * SpeedConstant / 50;

            movementVector = joypad * SpeedConstant / 30;

            //PlayerRigidBody.AddForce(movementVector * delta);
            PlayerRigidBody.velocity = movementVector;
            LastMovementVector = movementVector;
        }

        if (PlayerRigidBody.velocity.magnitude > MaxSpeedConstant && PlayerInCollision)
        {
            PlayerRigidBody.velocity = PlayerRigidBody.velocity.normalized * MaxSpeedConstant;
        }

        if (PlayerRigidBody.velocity.magnitude > MaxSpeedConstant *300)
        {
            PlayerRigidBody.velocity = PlayerRigidBody.velocity.normalized * MaxSpeedConstant * 300;
        }
        */
        //Debug.Log("Player Velocity: " + PlayerRigidBody.velocity.magnitude);




        if (ActualGravity != ComputedGravityVector)
        {
            ActualGravity = ComputedGravityVector.normalized * GravityConstant * delta;
        }

        if (Jumping)
        {
            JumpingTime += delta;
            GravityAndJump += ActualGravity * (1 + JumpingTime* JumpingTime * 5);
            movementVector = (joypad * SpeedConstant)*0.3f + ProjectedVectorToSurface(joypad, ComputedGravityVector) * SpeedConstant * 0.6f;
            //movementVector = ProjectedVectorToSurface(joypad, ComputedGravityVector) * SpeedConstant;
        }
        else
        {
            JumpingTime = JumpingTimeMinus;
            GravityAndJump = ActualGravity;
            movementVector = ProjectedVectorToSurface(joypad, ComputedGravityVector) * SpeedConstant;
        }
        
        

        //Physics2D.gravity = ActualGravity;
        //Physics.gravity = ActualGravity.normalized * 10;

        /*

        if (PlayerInCollision && joypad == Vector2.zero)
        {
            if (PlayerRigidBody.transform.parent != null && PlayerRigidBody.transform.parent.tag != "MovingColliders")
            {
                PlayerRigidBody.velocity = Vector2.zero;
                //Physics2D.gravity = Vector2.zero;
            }
        }*/

        //Debug.DrawRay(this.transform.position, ActualGravity);

        if (ButtonFire > 0)
        {
            ButtonFire = ButtonFire - 1;
            if /*(PlayerInCollision)*/(!Jumping)
            {
                //jump
                //PlayerRigidBody.AddForce(-ActualGravity.normalized * JumpConstant);
                GravityAndJump += -ActualGravity.normalized * JumpConstant;
                Debug.DrawRay(this.transform.position, -ActualGravity.normalized * JumpConstant);
                SetJump();
            }
        }

        Vector2 finalVector  = movementVector + GravityAndJump;

        if (finalVector.magnitude > MaxSpeed) finalVector = finalVector.normalized * MaxSpeed;

        PlayerRigidBody.velocity = finalVector;

        Debug.DrawRay(this.transform.position, movementVector, Color.green);
        Debug.DrawRay(this.transform.position, ActualGravity, Color.blue);
        Debug.DrawRay(this.transform.position, PlayerRigidBody.velocity, Color.red);


        if (PlayerRigidBody.transform.parent != null && PlayerRigidBody.transform.parent.tag == "MovingColliders" && !Jumping)
        {
            PlayerRigidBody.velocity += PlayerRigidBody.transform.parent.GetComponent<MovingObstacle>().MovingRigidBody.velocity;
        }

        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, CameraAreaScript.GetCameraPos(), ref velocity, 0.2f);
    }

    public void SetJump()
    {
        //Debug.Log("Jump");
        Jumping = true;
        Walking = false;
        PlayerInCollision = false;
        SetJumpingWallCollidersActive(true, null);
        PlayerAnim.SetTrigger("Jump");
        this.transform.SetParent(null);
        //Debug.Log("Jump Set");
    }

    void SetJumpingWallCollidersActive(bool active, JumpingWall stayActive)
    {
        int i;
        for (i = 0; i < JumpingWalls.Length; i++)
        {
            //Debug.Log("Stay Active Colider: " + stayActive.gameObject);
            if (stayActive != JumpingWalls[i])
                JumpingWalls[i].ActivateWall(active);
        }
    }

    void setAnimState()
    {
        if (Jumping && !PlayerInCollision)
        {
            return;
        }
        

        if (Walking && joypad.magnitude < 0.1)
        {
            Walking = false;
            PlayerAnim.SetTrigger("Idle");
            Debug.Log("Idle");
        }
        else if (!Walking && joypad.magnitude >= 0.1)
        {
            PlayerAnim.SetTrigger("Move");
            Walking = true;
            Debug.Log("Moving");
        }
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        //if ((Jumping && ButtonFire == 0) || coll.transform.tag == "MovingColliders")
        if ((coll.transform.tag == "JumpingColliders" && ButtonFire > 0) || (coll.transform.tag == "FallingAppartColliders" && coll.gameObject.GetComponent<FallingAppartObstacle>().ActualSegment == -1))
        {
            return;
        }
        if (coll.transform.tag == "PlayerEnvelope")
        {
            return;
        }
        
        //AttachedObject = coll.gameObject.GetComponent<WallCollider>();
        this.transform.SetParent(coll.gameObject.transform);
        if (coll.gameObject.tag == "JumpingColliders")
        {
            SetJumpingWallCollidersActive(false, coll.gameObject.GetComponent<JumpingWall>());
        }
        ComputedGravityVector = -coll.contacts[0].normal * GravityConstant;

        //Debug.Log("New Collision: " + coll.gameObject.transform);

        PlayerInCollision = true;
        setAnimState();
        Jumping = false;
            
        
    }
    
    Vector2 CheckGravityAround()
    {
        int i;
        
        float radian;
        Vector2 distanceVector;
        float sumOfDistances;

        Vector2 SumOfGravity;

        sumOfDistances = 0;

        ImpactCheck[] impactPoints;

        impactPoints = new ImpactCheck[MaxTrackingRays];

        for (i = 0; i < MaxTrackingRays; i++)
        {
            radian = i * Mathf.PI * 2 / MaxTrackingRays;

            RaycastHit2D[] hit;
            if (Jumping)
            {
                if (JumpingTime > 0)
                {
                    hit = Physics2D.RaycastAll(this.transform.position, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)), 1 + JumpingTime * JumpingTime * 30);
                } else
                {
                    hit = Physics2D.RaycastAll(this.transform.position, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)), 0.1f);
                }
            }
            else
            {
                hit = Physics2D.RaycastAll(this.transform.position, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)), 2);
            }

            if (hit.Length > 0 && hit[0].transform.tag != "PlayerEnvelope")
            {
                ////Debug.Log("Hit:" + impactPoints[i].wallCollider);

                //Debug.DrawRay(this.transform.position, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * 10, Color.green);

                Debug.DrawLine(new Vector3(hit[0].point.x, hit[0].point.y - 1, 0), new Vector3(hit[0].point.x, hit[0].point.y + 1, 0), Color.red, Time.deltaTime, false);
                Debug.DrawLine(new Vector3(hit[0].point.x - 1, hit[0].point.y, 0), new Vector3(hit[0].point.x + 1, hit[0].point.y, 0), Color.red, Time.deltaTime, false);

                distanceVector = hit[0].point - new Vector2(this.transform.position.x, this.transform.position.y);
                impactPoints[i].distance = distanceVector.magnitude;
                impactPoints[i].gravity = -hit[0].normal;

                sumOfDistances = sumOfDistances + (1 / (impactPoints[i].distance * impactPoints[i].distance * impactPoints[i].distance * impactPoints[i].distance));
                
            }
            else
            {
                impactPoints[i].distance = -1;
            }
        }

        //Debug.Log("Sum of Distances:" + sumOfDistances);

        SumOfGravity = Vector2.zero;

        for (i = 0; i < MaxTrackingRays; i++)
        {
            if (impactPoints[i].distance != -1)
            {
                SumOfGravity = SumOfGravity + (1 / (impactPoints[i].distance * impactPoints[i].distance * impactPoints[i].distance * impactPoints[i].distance) / sumOfDistances) * impactPoints[i].gravity;

                //Debug.DrawRay(this.transform.position, (1 / (impactPoints[i].distance * impactPoints[i].distance) / sumOfDistances) * impactPoints[i].gravity * 1000, Color.green);
            }
        }

        //Debug.Log("Sum of Gravity:" + SumOfGravity);

        if (SumOfGravity == Vector2.zero)
        {
            return ComputedGravityVector;
            //return Vector2.down * GravityConstant;
        }

        return SumOfGravity;
    }

    void SetControlValues()
    {
        joypad.x = Input.GetAxis("Horizontal" + ControllerIndex);
        joypad.y = Input.GetAxis("Vertical" + ControllerIndex);

        if (joypad.magnitude >= 0.1)
        {
            lastjoypad = joypad;
            //Debug.Log("WalkingSet");
        } else
        {
            joypad = Vector2.zero;
        }

        //joypad = joypad + MainScript.GetInstance().GuiInstance.VirtualJoystick.InputDirection;

        if (Input.GetButtonDown("Jump" + ControllerIndex))
        {
            ButtonFire = 3;
            //ActualFireRate = 100;
            Debug.Log("JumpSet for controller: " + ControllerIndex);
        }
        if (Input.GetButtonUp("Jump" + ControllerIndex))
        {
            ButtonFire = 0;
        }
    }

    void GetControls()
    {
        SetControlValues();

        /*if (MainScript.GetInstance().LoaderInstance == null)
        {
            SetControlValues();
        }
        else if (MainScript.GetInstance().LoaderInstance.activemenu == -1)
        {
            SetControlValues();
        }
        */
    }

}




