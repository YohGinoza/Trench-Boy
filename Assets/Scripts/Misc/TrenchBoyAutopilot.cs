using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrenchBoyAutopilot : MonoBehaviour
{
    [SerializeField] private float TimeBetweenSession;
    private InventorySystem Inventory;

    // ------------------------------
    // character movement
    // ------------------------------
    public bool isMovable = true;
    public bool isCarrying = false;
    //public float movementSpeed = 0.0f;
    private float maxSpeed = 0.0f;
    public float defaultSpeed = 0.0f;
    [Range(0, 1)] [SerializeField] private float carrySpeedMultiplier = 0.8f;
    [SerializeField] private float TimeToReachMaxSpeed = 0.2f;
    private float carrySpeed = 0.0f;
    public bool facingRight = true;
    public bool animatorMoving = false;

    private bool WalkingLeft = true;
    private Vector3 refVector = Vector3.zero;
    [SerializeField] private int carrying = 0;

    private bool TimerRunning = false;
    // ------------------------------
    // transform position
    // ------------------------------
    //public Transform Carrier;
    //private Transform CarriedObject;

    // ------------------------------
    // Interactable Item
    // ------------------------------
    //private float carryDelay = 0.8f;
    ////private bool cooldown = false;              //timer betwween put down crate and pick up pouch
    //public bool carryingCrate = false;
    //public float CratePickUpTime = 0.5f;
    //[SerializeField] private float ItemPickUpTime = 0.1f;
    //public SupplyZone ReSupplyZone = null;
    //public MedicalBed MedBed = null;
    //public SleepingBed Bed = null;
    //public bool ColliderInfront;
    //--------------------------
    //Input control
    //--------------------------
    public bool spacebarUpped = false;             //some situation we don't want the game to register spacebar up twice 
                                                   //(such as when drop/pick up crate) so it won't pick up item at the same time

    //GameController gameController;
    Animator animator;
    Rigidbody rb;
    ColliderChercker Checker;
    //Crate crate; // waiting for Crate script

    //Vector3 refVector = Vector3.zero;
    //public Vector3 facing = Vector3.zero;
    //private Vector3 carryPos = new Vector3(0.0f, 0.5f, 0.0f);

    // for checking time interaction
    //public float delta = 0.0f; // button hold timer
    //float delay = 0.0f; // delay between interactions

    //sound
    public AudioClip unavialable;
    public AudioClip boxDown;
    public AudioClip boxUp;
    public AudioClip converse;

    public AudioClip[] footsteps = new AudioClip[6];

    public AudioSource audioSource;
    private bool footstepPlay = true;

    //tutorialUI
    [SerializeField] private TutorialUI TutorUI;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Checker = this.GetComponentInChildren<ColliderChercker>();
        Inventory = this.GetComponent<InventorySystem>();
        animator = this.GetComponentInChildren<Animator>();
        //gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        //   crate = GetComponent<Crate>(); // wating for Crate script
        facingRight = true;
        carrySpeed = defaultSpeed * carrySpeedMultiplier;
    }

    private void Update()
    {
        //set animator parameter
        animator.SetBool("Moving", true);
        animator.SetInteger("Item", carrying);

    }

    void FixedUpdate()
    {
        //check if movable
        if (isMovable == true)
        {
            Move();
        }

        if (!TimerRunning)
        {
            StartCoroutine(SwitchDirection());
        }
    }

    // ↑↓→← WASD
    private void Move()
    {
        if (isMovable == true)
        {
            // ------------------------------
            // ms check
            // ------------------------------
            if (isCarrying == true)
            {
                // ms lowered while carrying
                maxSpeed = carrySpeed;
            }
            else
            {
                // if !carrying ms = default
                maxSpeed = defaultSpeed;
            }


            //check if input walking
            if (WalkingLeft || !WalkingLeft)
            {
                //play foot step sound
                if (footstepPlay)
                {
                    StartCoroutine(FootStepsPlay());
                }

                // ------------------------------
                // buttons for character controls
                // ------------------------------
                // LEFT
                if (WalkingLeft)
                {
                    rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.left * defaultSpeed, ref refVector, TimeToReachMaxSpeed);
                    //rb.velocity = Vector3.left * defaultSpeed;

                    if (facingRight)
                    {
                        Flip();
                    }
                    animatorMoving = true;
                }
                // RIGHT
                if (!WalkingLeft)
                {
                    rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.right * defaultSpeed, ref refVector, TimeToReachMaxSpeed);
                    //rb.velocity = Vector3.right * defaultSpeed;

                    if (!facingRight)
                    {
                        Flip();
                    }
                    animatorMoving = true;
                }
                //// UP
                //if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                //{
                //    rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.forward * defaultSpeed, ref refVector, TimeToReachMaxSpeed);
                //    //rb.velocity = Vector3.forward * defaultSpeed;

                //    facing = Vector3.forward;
                //    animatorMoving = true;
                //}
                //// DOWN
                //if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                //{
                //    rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.back * defaultSpeed, ref refVector, TimeToReachMaxSpeed);
                //    //rb.velocity = Vector3.back * defaultSpeed;

                //    facing = Vector3.back;
                //    animatorMoving = true;
                //}
                //-------------------------------
            }
            else
            {
                //animation
                animatorMoving = false;

                //slow down
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.zero, ref refVector, 0.2f);
            }
        }
    }

    public void Flip()
    {
        this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y, this.transform.localScale.z);
        facingRight = !facingRight;
    }

    private IEnumerator FootStepsPlay()
    {
        footstepPlay = false;

        int index = Random.Range(0, footsteps.Length);
        this.GetComponent<AudioSource>().clip = footsteps[index];

        this.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(0.5f);


        footstepPlay = true;

    }


    IEnumerator SwitchDirection()
    {
        TimerRunning = true;
        yield return new WaitForSeconds(TimeBetweenSession);
        WalkingLeft = !WalkingLeft;
        if(Random.Range(0.0f,1.0f) > 0.95f)
        {
            //ally
            carrying = 1;
        }
        else
        {
            carrying = Random.Range(2, 4);
        }
        TimerRunning = false;
    }
}