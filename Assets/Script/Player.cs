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
        public WallCollider wallCollider;
    }

    public Animator PlayerAnim;
    public GameObject Shadow;
    public WallCollider[] WallColiders;

    public WallCollider AttachedObject;
    CameraArea CameraAreaScript;

    public Image PlayerImage;

    Vector2 ActualGravity;
    Vector2 ComputedGravityVector;
    Vector2 LastMovementVector;

    Rigidbody2D PlayerRigidBody;    

    float SpeedConstant;
    float MaxSpeedConstant;
    float GravityConstant;
    float JumpGravityReductionConstant;
    float JumpConstant;
    int MaxTrackingRays;

    public bool PlayerInCollision;
    public bool Jumping;
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

        go = GameObject.FindGameObjectsWithTag("WallColliders");
        WallColiders = new WallCollider[go.Length];
        for (i = 0; i < go.Length; i++)
        {
            WallColiders[i] = go[i].GetComponent<WallCollider>();
        }

        ActualGravity.x = 0;
        ActualGravity.y = 0;
        MaxTrackingRays = 128;

        SpeedConstant = 35000;
        MaxSpeedConstant = 0.07f;
        GravityConstant = 600;
        JumpGravityReductionConstant = 0.125f;
        JumpConstant = 2500;

        ComputedGravityVector = Vector2.down * GravityConstant;

        PlayerInCollision = false;
        Jumping = true;
        Walking = false;

        PlayerRigidBody = this.GetComponent<Rigidbody2D>();
        PlayerImage = this.GetComponent<Image>();

        SetWallCollidersActive(true, null);
    }

    // Update is called once per frame


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

        Vector2 dir = CheckGravityAround().normalized * GravityConstant;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        cross = Vector3.Cross(dir, ProjectedVectorToSurface(lastjoypad, dir));

        transform.localScale = new Vector3(transform.localScale.x, Mathf.Sign(cross.z) * transform.localScale.x, transform.localScale.z);

    }

    void FixedUpdate()
    {
        Vector3 velocity = Vector3.zero;
        float delta;
        Vector2 movementVector;
        float gravityMultiplier;

        delta = Time.deltaTime;

        movementVector = Vector2.zero;

        gravityMultiplier = 1;

        ComputedGravityVector = CheckGravityAround();

        if (PlayerInCollision)
        {
            movementVector = ProjectedVectorToSurface(joypad , ComputedGravityVector) * SpeedConstant;

            Debug.DrawRay(this.transform.position, movementVector/1000);

            PlayerRigidBody.AddForce(movementVector * delta);
            LastMovementVector = movementVector;
            
            Shadow.SetActive(true);
        }
        else
        {
            Shadow.SetActive(false);
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

            PlayerRigidBody.AddForce(movementVector * delta);
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

        /*Debug.Log("Player Velocity: " + PlayerRigidBody.velocity.magnitude);
        if (PlayerRigidBody.velocity.magnitude < 0.1)
        {
            PlayerRigidBody.velocity = Vector2.zero;
        }*/

        ActualGravity = ComputedGravityVector.normalized * GravityConstant * gravityMultiplier;

        Physics2D.gravity = ActualGravity;

        Debug.DrawRay(this.transform.position, ActualGravity);

        if (ButtonFire > 0)
        {
            ButtonFire = ButtonFire - 1;
            if /*(PlayerInCollision)*/(!Jumping)
            {
                //jump

                PlayerRigidBody.AddForce(-ActualGravity.normalized * JumpConstant);

                Debug.DrawRay(this.transform.position, -ActualGravity.normalized * JumpConstant);

                Debug.Log("Jump");
                Jumping = true;
                Walking = false;
                PlayerInCollision = false;
                SetWallCollidersActive(true, null);
                PlayerAnim.SetTrigger("Jump");
                this.transform.SetParent(null);
            }
        }

        if (AttachedObject != null && AttachedObject.WallRigidBody != null && !Jumping)
        {
            PlayerRigidBody.velocity += AttachedObject.WallRigidBody.velocity;
        }

        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, CameraAreaScript.GetCameraPos(), ref velocity, 0.2f);
        setAnimState();
    }

    void SetWallCollidersActive(bool active, WallCollider stayActive)
    {
        int i;
        for (i = 0; i < WallColiders.Length; i++)
        {
            //Debug.Log("Stay Active Colider: " + stayActive.gameObject);
            if (stayActive != WallColiders[i])
                WallColiders[i].ActivateWall(active);
        }
    }

    /*    void OnCollisionExit2D(Collision2D coll)
        {

            if (coll.gameObject == AttachedObject.gameObject)
            {
                PlayerInCollision = false;
            }
        }
    */

    void setAnimState()
    {
        if (Jumping) return;

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
        if ((AttachedObject == null || coll.gameObject != AttachedObject.gameObject || Jumping) && ButtonFire == 0)
        {
            AttachedObject = coll.gameObject.GetComponent<WallCollider>();
            this.transform.SetParent(coll.gameObject.transform);
            SetWallCollidersActive(false, AttachedObject);
            ComputedGravityVector = -coll.contacts[0].normal * GravityConstant;
            //Debug.Log("New Collision" + coll.gameObject.transform);
            //setAnimState();
            Jumping = false;
            PlayerInCollision = true;
        }
    }
    
    /*void OnCollisionEnter2D(Collision2D coll)
    {
        setAnimState();
    }*/
    

    Vector2 CheckGravityAround()
    {
        int i;
        bool foundAttachedObject;
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
                hit = Physics2D.RaycastAll(this.transform.position, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));
            }
            else
            {
                hit = Physics2D.RaycastAll(this.transform.position, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)), 1);
            }

            if (hit.Length > 0)
            {
                ////Debug.Log("Hit:" + impactPoints[i].wallCollider);

                Debug.DrawRay(this.transform.position, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * 10, Color.green);

                foundAttachedObject = false;

                if (AttachedObject != null)
                {
                    if (hit[0].collider.gameObject == AttachedObject.gameObject)
                    {
                        foundAttachedObject = true;
                    }
                }

                //if (Jumping/* || foundAttachedObject || AttachedObject == null*/)
                //{
                    Debug.DrawLine(new Vector3(hit[0].point.x, hit[0].point.y - 1, 0), new Vector3(hit[0].point.x, hit[0].point.y + 1, 0), Color.red, Time.deltaTime, false);
                    Debug.DrawLine(new Vector3(hit[0].point.x - 1, hit[0].point.y, 0), new Vector3(hit[0].point.x + 1, hit[0].point.y, 0), Color.red, Time.deltaTime, false);

                    distanceVector = hit[0].point - new Vector2(this.transform.position.x, this.transform.position.y);
                    impactPoints[i].distance = distanceVector.magnitude;
                    impactPoints[i].gravity = -hit[0].normal;
                    impactPoints[i].wallCollider = hit[0].collider.gameObject.GetComponent<WallCollider>();

                    sumOfDistances = sumOfDistances + (1 / (impactPoints[i].distance * impactPoints[i].distance * impactPoints[i].distance * impactPoints[i].distance));
                /*}
                else
                {
                    impactPoints[i].distance = -1;
                }*/
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

                Debug.DrawRay(this.transform.position, (1 / (impactPoints[i].distance * impactPoints[i].distance) / sumOfDistances) * impactPoints[i].gravity * 1000, Color.green);
            }
        }

        //Debug.Log("Sum of Gravity:" + SumOfGravity);

        return SumOfGravity;

        /*
        Vector3 hit3d = Vector3.zero;
        hit3d.x = hit.point.x;
        hit3d.y = hit.point.y;*/




        //Line[i].SetPosition(1, hit.point);
    }

    void SetControlValues()
    {
        joypad.x = Input.GetAxis("Horizontal");
        joypad.y = Input.GetAxis("Vertical");

        if (joypad.magnitude >= 0.1)
        {
            lastjoypad = joypad;
            //Walking = true;
            //Debug.Log("WalkingSet");
        } else
        {
            joypad = Vector2.zero;
        }

        //joypad = joypad + MainScript.GetInstance().GuiInstance.VirtualJoystick.InputDirection;

        if (Input.GetButtonDown("Jump"))
        {
            ButtonFire = 3;
            //ActualFireRate = 100;
        }
        if (Input.GetButtonUp("Jump"))
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




