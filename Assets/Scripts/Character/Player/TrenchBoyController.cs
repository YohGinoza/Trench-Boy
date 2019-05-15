using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrenchBoyController : MonoBehaviour
{

    private InventorySystem Inventory;

    // ------------------------------
    // game objects
    // ------------------------------
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject med_pouch;
    [SerializeField] private GameObject ammo_pouch;
    private GameObject pouch;

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

    // ------------------------------
    // transform position
    // ------------------------------
    public Transform Carrier;
    private Transform CarriedObject;
    [SerializeField] private Transform world;

    // ------------------------------
    // Interactable Item
    // ------------------------------
    private float carryDelay = 0.8f;
    //private bool cooldown = false;              //timer betwween put down crate and pick up pouch
    public bool carryingCrate = false;
    public float CratePickUpTime = 0.5f;
    [SerializeField] private float ItemPickUpTime = 0.1f;
    public SupplyZone ReSupplyZone = null;
    public MedicalBed MedBed = null;
    public SleepingBed Bed = null;
    public bool ColliderInfront;
    //--------------------------
    //Input control
    //--------------------------
    public bool spacebarUpped = false;             //some situation we don't want the game to register spacebar up twice 
                                                   //(such as when drop/pick up crate) so it won't pick up item at the same time

    GameController gameController;
    Animator animator;
    Rigidbody rb;
    ColliderChercker Checker;
    //Crate crate; // waiting for Crate script

    Vector3 refVector = Vector3.zero;
    public Vector3 facing = Vector3.zero;
    private Vector3 carryPos = new Vector3(0.0f, 0.5f, 0.0f);

    // for checking time interaction
    public float delta = 0.0f; // button hold timer
    float delay = 0.0f; // delay between interactions

    //sound
    public AudioClip unavialable;
    public AudioClip boxDown;
    public AudioClip boxUp;
    public AudioClip converse;
    public AudioClip give;

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
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        //   crate = GetComponent<Crate>(); // wating for Crate script
        facingRight = true;
        carrySpeed = defaultSpeed * carrySpeedMultiplier;
    }

    private void Update()
    {
        //other player input
        Interaction();

        //set animator parameter
        animator.SetBool("Moving", animatorMoving);
        if (Carrier.childCount > 0)
        {
            CarriedObject = Carrier.GetChild(0);

            if (CarriedObject.CompareTag("Ally"))
            {
                animator.SetInteger("Item", 1);
            }
            else if (CarriedObject.CompareTag("Crate"))
            {
                switch (CarriedObject.GetComponent<Crates>().Type)
                {
                    case ItemType.Med:
                        animator.SetInteger("Item", 2);
                        break;
                    case ItemType.Ammo:
                        animator.SetInteger("Item", 3);
                        break;
                }
            }
        }
        else
        {
            animator.SetInteger("Item", 0);
        }
    }

    void FixedUpdate()
    {
        //check if movable
        if (isMovable == true)
        {
            Move();
        }

        //register carrying statuses
        if (Carrier.childCount > 0)
        {
            CarriedObject = Carrier.GetChild(0);
            isCarrying = true;

            if (Carrier.GetChild(0).CompareTag("Crate"))
            {
                carryingCrate = true;
            }
            else
            {
                carryingCrate = false;
            }
        }
        else if (!Inventory.isEmpty())
        {
            isCarrying = true;
            carryingCrate = false;
        }
        else
        {
            CarriedObject = null;
            isCarrying = false;
        }

        //clear item at day end
        if (gameController.CurrentState == GameState.Night || gameController.CurrentState == GameState.Wait)
        {
            if (!Inventory.isEmpty())
            {
                Inventory.RemoveAllItem();
            }
        }

        //===============
        //tutorial
        //===============

        if (Checker.ClosestTrigerrer != null)
        {
            //crate pickup
            if (!GameController.TutorialFinished[(int)Tutorials.PickUpCrate] && Checker.ClosestTrigerrer.CompareTag("Crate"))
            {
                //Debug.Log("pickCrate");
                TutorUI.TurnOn(Tutorials.PickUpCrate);
            }

            //pickup item
            if (!GameController.TutorialFinished[(int)Tutorials.PickUpItem] && Checker.ClosestTrigerrer.CompareTag("Crate"))
            {
                //Debug.Log("pickItem");
                TutorUI.TurnOn(Tutorials.PickUpItem);
            }

            //tutorial refilling
            if (!GameController.TutorialFinished[(int)Tutorials.RefillingCrate] && Checker.ClosestTrigerrer.CompareTag("Resupply"))
            {
                //Debug.Log("refill");
                TutorUI.TurnOn(Tutorials.RefillingCrate);
            }
        }

        //inventory control
        if (!GameController.TutorialFinished[(int)Tutorials.InventoryControl] && !Inventory.isEmpty())
        {
            TutorUI.TurnOn(Tutorials.InventoryControl);
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
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.UpArrow)
                || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
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
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                {
                    rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.left * defaultSpeed, ref refVector, TimeToReachMaxSpeed);
                    //rb.velocity = Vector3.left * defaultSpeed;

                    if (facingRight)
                    {
                        Flip();
                    }
                    facing = Vector3.left;
                    animatorMoving = true;
                }
                // RIGHT
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                {
                    rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.right * defaultSpeed, ref refVector, TimeToReachMaxSpeed);
                    //rb.velocity = Vector3.right * defaultSpeed;

                    if (!facingRight)
                    {
                        Flip();
                    }
                    facing = Vector3.right;
                    animatorMoving = true;
                }
                // UP
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                {
                    rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.forward * defaultSpeed, ref refVector, TimeToReachMaxSpeed);
                    //rb.velocity = Vector3.forward * defaultSpeed;

                    facing = Vector3.forward;
                    animatorMoving = true;
                }
                // DOWN
                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                {
                    rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.back * defaultSpeed, ref refVector, TimeToReachMaxSpeed);
                    //rb.velocity = Vector3.back * defaultSpeed;

                    facing = Vector3.back;
                    animatorMoving = true;
                }
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

    private void Interaction()
    {
        // ------------------
        // cycle inventory slot
        // ------------------
        if (Input.GetKeyDown(KeyCode.E))
        {
            Inventory.CycleInventory();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Inventory.RemoveItem();
        }

        // ------------------
        // is not Carrying
        // ------------------
        if (!spacebarUpped && Input.GetKey(KeyCode.Space))
        {
            if (!isCarrying || Inventory.HasEmptySlot())
            {
                //check if any thing is available
                if (Checker.ClosestTrigerrer != null || ReSupplyZone != null || Bed != null)
                {
                    delta += Time.deltaTime; // hold timer starts after pressing the button
                    if (delta >= CratePickUpTime)
                    {

                        if (!isCarrying)
                        {
                            //pick up crate
                            if (Checker.ClosestTrigerrer != null && Checker.ClosestTrigerrer.CompareTag("CargoSlot"))
                            {
                                Checker.ClosestTrigerrer.GetComponent<CargoSlot>().TakeOffCargo(Carrier, Vector3.zero);
                                delta = 0; // stops timer
                                spacebarUpped = true;  //count as key up
                            }
                            else if (Checker.ClosestTrigerrer != null && Checker.ClosestTrigerrer.CompareTag("Crate"))
                            {
                                Checker.childTransfer(Carrier);
                                audioSource.clip = boxUp;
                                audioSource.Play();

                                delta = 0; // stops timer
                                spacebarUpped = true;  //count as key up
                            }
                            else if (Checker.ClosestTrigerrer != null && Checker.ClosestTrigerrer.CompareTag("Ally") && Checker.ClosestTrigerrer.GetComponent<AllyBehaviour>().CurrentState == AllyBehaviour.State.Downed)
                            {
                                Checker.childTransfer(Carrier);
                                delta = 0; // stops timer
                                spacebarUpped = true;  //count as key up

                                //prevent from stuck in ground, will change later
                                Checker.ClosestTrigerrer.GetComponent<Collider>().isTrigger = true;
                            }
                            else if (ReSupplyZone != null)
                            {
                                ReSupplyZone.SpawnNew(Carrier);
                            }
                            //sleep
                            else if (Bed != null)
                            {
                                Bed.StartCoroutine("Sleep");
                            }
                        }
                        else
                        {
                            audioSource.clip = unavialable;
                            audioSource.Play();
                            animator.SetTrigger("Talk");
                        }
                    }
                }
            }
            else if (!Inventory.HasEmptySlot())
            {
                if (Checker.ClosestTrigerrer != null || ReSupplyZone != null || Bed != null)
                {
                    audioSource.clip = unavialable;
                    audioSource.Play();
                    animator.SetTrigger("Talk");
                }
            }
        }

        if (!spacebarUpped && Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("UP");
            if ((!isCarrying || Inventory.HasEmptySlot()) && delta < ItemPickUpTime && (gameController.CurrentState == GameState.Day || gameController.CurrentState == GameState.Stalling))
            {
                // pick up the POUCH
                if (Checker.ClosestTrigerrer != null && Checker.ClosestTrigerrer.CompareTag("Crate"))
                {
                    Crates crate = Checker.ClosestTrigerrer.GetComponent<Crates>();

                    if (crate.Amount > 0)
                    {
                        if (Inventory.Add(crate.Type))
                        {
                            crate.PickOut();
                            isCarrying = true;
                        }
                    }
                    else
                    {
                        audioSource.clip = unavialable;
                        audioSource.Play();
                        animator.SetTrigger("Talk");

                        //tutorial refilling
                        if (!GameController.TutorialFinished[(int)Tutorials.RefillingCrate])
                        {
                            TutorUI.TurnOn(Tutorials.RefillingCrate);
                        }
                    }
                }
            }
            delta = 0; // stops timer 
            spacebarUpped = true;
        }

        if (spacebarUpped && Input.GetKeyDown(KeyCode.Space))
        {
            spacebarUpped = false;
            // ------------------
            // is Carrying
            // ------------------
            if (isCarrying)
            {
                // POUCH
                if (!Inventory.isEmpty() && (gameController.CurrentState == GameState.Day || gameController.CurrentState == GameState.Stalling))
                {
                    if (Checker.ClosestTrigerrer != null && Checker.ClosestTrigerrer.gameObject.layer == 9/*Ally*/)
                    {
                        AllyBehaviour ally = Checker.ClosestTrigerrer.GetComponent<AllyBehaviour>();

                        if (ally.HandItem(Inventory.ItemInventory[Inventory.SelectedItem]))
                        {
                            audioSource.clip = give;
                            audioSource.Play();
                            Inventory.RemoveItem();
                        }
                        else
                        {
                            audioSource.clip = unavialable;
                            audioSource.Play();
                            animator.SetTrigger("Talk");
                        }
                    }
                }
                //CRATE
                else if (carryingCrate)
                {
                    if (Checker.ClosestTrigerrer != null && Checker.ClosestTrigerrer.CompareTag("CargoSlot"))
                    {
                        Checker.ClosestTrigerrer.GetComponent<CargoSlot>().StoreCargo(CarriedObject);
                        carryingCrate = false;
                        //isCarrying = false;
                    }
                    else if (ReSupplyZone != null)
                    {
                        //refill crate
                        if (CarriedObject != null && CarriedObject.GetComponent<Crates>())
                        {
                            ReSupplyZone.RefillCrate(CarriedObject.GetComponent<Crates>());
                        }
                    }
                    else if (!ColliderInfront)
                    {
                        //put crate box
                        // ************************* will change after engine proof *****************************
                        PutDownCarryingObject();
                        audioSource.clip = boxDown;
                        audioSource.Play();

                        carryingCrate = false;
                        //isCarrying = false;
                    }else if (ColliderInfront)
                    {
                        audioSource.clip = unavialable;
                        audioSource.Play();
                        animator.SetTrigger("Talk");
                    }
                    spacebarUpped = true;
                    delta = 0;
                }
                //ALLY
                else if (CarriedObject.CompareTag("Ally"))
                {
                    if (MedBed != null)
                    {
                        //reenable collider, will change later
                        CarriedObject.GetComponent<Collider>().isTrigger = false;

                        //put on med bed
                        MedBed.PutPatient(CarriedObject);

                        //return the x size to normal
                        CarriedObject.localScale = new Vector3(Mathf.Abs(CarriedObject.localScale.x), CarriedObject.localScale.y, CarriedObject.localScale.z);

                        gameController.HelpFriend++;
                    }
                    else
                    {                        
                        //just put down
                        PutDownCarryingObject();

                        //return the x size to normal
                        CarriedObject.localScale = new Vector3(Mathf.Abs(CarriedObject.localScale.x), CarriedObject.localScale.y, CarriedObject.localScale.z);

                        //reenable collider, will change later
                        CarriedObject.GetComponent<Collider>().isTrigger = false;

                        carryingCrate = false;
                    }
                }

                //set bool in animator
                animator.SetInteger("Item", 0);
            }
            else
            {
                if (gameController.CurrentState == GameState.Night)
                {

                    /*rb.velocity = Vector3.zero;
                    isMovable = false;*/

                    if (Checker.ClosestTrigerrer != null && Checker.ClosestTrigerrer.gameObject.layer == 9/*Ally*/
                         && Checker.ClosestTrigerrer.GetComponent<DialogueLoader>().enabled && !GameObject.Find("Main Camera").GetComponent<CameraController>().zoomOut)
                    {
                        //GameObject ally = Checker.ClosestTrigerrer.gameObject.transform.Find("AllyCanvas").Find("Text").gameObject;
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                        isMovable = false;

                        if (rb.velocity == Vector3.zero)
                        {
                            Checker.ClosestTrigerrer.GetComponent<DialogueLoader>().converse();
                        }

                        //ally.GetComponent<DialogueLoader>().converse();
                    }
                }
            }
        }

    }

    private void PutDownCarryingObject()
    {
        CarriedObject.position = this.transform.position + facing;
        CarriedObject.SetParent(world);
        CarriedObject.position = new Vector3(CarriedObject.position.x, CarriedObject.transform.lossyScale.y * 0.5f, CarriedObject.position.z);
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
}
