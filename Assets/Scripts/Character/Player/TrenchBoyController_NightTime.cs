using UnityEngine;

public class TrenchBoyController_NightTime : MonoBehaviour
{

    private InventorySystem Inventory;

    // ------------------------------
    // game objects
    // ------------------------------
    [SerializeField] private GameObject player;

    // ------------------------------
    // character movement
    // ------------------------------
    public bool isMovable = true;
    public bool isCarrying = false;
    public float movementSpeed = 0.0f;
    private float maxSpeed = 0.0f;
    public float carrySpeed = 0.0f;
    public float defaultSpeed = 0.0f;

    //--------------------------
    //Input control
    //--------------------------
    public bool spacebarUpped = false;             //some situation we don't want the game to register spacebar up twice 
                                                   //(such as when drop/pick up crate) so it won't pick up item at the same time


    Rigidbody rb;
    ColliderChercker Checker;
    //Crate crate; // waiting for Crate script

    Vector3 refVector = Vector3.zero;
    Vector3 facing = Vector3.zero;
    private Vector3 carryPos = new Vector3(0.0f, 0.5f, 0.0f);

    // for checking time interaction
    public float delta = 0.0f; // button hold timer
    float delay = 0.0f; // delay between interactions

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Checker = this.GetComponentInChildren<ColliderChercker>();
        Inventory = this.GetComponent<InventorySystem>();
        //   crate = GetComponent<Crate>(); // wating for Crate script
    }

    private void Update()
    {
        //check if movable
        if (isMovable == true)
        {
            Move();
        }

        //other player input
        Interaction();
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

            // ------------------------------
            // buttons for character controls
            // ------------------------------
            // LEFT
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.left * movementSpeed, ref refVector, 0.05f, maxSpeed);
                facing = Vector3.left;
            }
            // RIGHT
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.right * movementSpeed, ref refVector, 0.05f, maxSpeed);
                facing = Vector3.right;
            }
            // UP
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.forward * movementSpeed, ref refVector, 0.05f, maxSpeed);
                facing = Vector3.forward;
            }
            // DOWN
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.back * movementSpeed, ref refVector, 0.05f, maxSpeed);
                facing = Vector3.back;
            }
            //-------------------------------
        }
    }

    private void Interaction()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (Checker.ClosestTrigerrer != null && Checker.ClosestTrigerrer.gameObject.layer == 9/*Ally*/)
            {
                GameObject ally = Checker.ClosestTrigerrer.gameObject.transform.Find("AllyCanvas").Find("Text").gameObject;

                ally.GetComponent<Dialogue>().converse();
            }
            
        }


    }

}

