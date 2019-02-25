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
    public float movementSpeed = 0.0f;
    private float maxSpeed = 0.0f;
    public float carrySpeed = 0.0f;
    public float defaultSpeed = 0.0f;

    // ------------------------------
    // transform position
    // ------------------------------
    public Transform Carrier;
    private Transform CarriedObject;
    [SerializeField] private Transform world;

    // ------------------------------
    // POUCH or CRATE
    // ------------------------------
    private float carryDelay = 0.8f;
    //private bool cooldown = false;              //timer betwween put down crate and pick up pouch
    public bool carryingCrate = false;
    public float interaction_time = 0.5f;



    Rigidbody rb;
    ColliderChercker Checker;
    //Crate crate; // waiting for Crate script

    Vector3 refVector = Vector3.zero;
    Vector3 facing = Vector3.zero;
    private Vector3 carryPos = new Vector3(0.0f, 0.5f, 0.0f);

    // for checking time interaction
    float delta = 0.0f; // button hold timer
    float delay = 0.0f; // delay between interactions

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Checker = this.GetComponentInChildren<ColliderChercker>();
        Inventory = this.GetComponent<InventorySystem>();
        //   crate = GetComponent<Crate>(); // wating for Crate script
    }

    void FixedUpdate()
    {
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
        }else if (!Inventory.isEmpty())
        {
            isCarrying = true;
            carryingCrate = false;
        }
        else
        {
            CarriedObject = null;
            isCarrying = false;
        }

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
        // ------------------
        // cycle inventory slot
        // ------------------
        if (Input.GetKeyDown(KeyCode.E))
        {
            Inventory.CycleInventory();
        }

        // ------------------
        // is not Carrying
        // ------------------        
        if (!isCarrying && Input.GetKey(KeyCode.Space))
        {
            Debug.Log("not carry");
            delta += Time.deltaTime; // hold timer starts after pressing the button

            if (delta >= interaction_time)
            {
                //pick up crate
                if (Checker.ClosestTrigerrer != null && Checker.ClosestTrigerrer.CompareTag("CargoSlot"))
                {
                    Checker.ClosestTrigerrer.GetComponent<CargoSlot>().TakeOffCargo(Carrier, Vector3.zero);
                    //isCarrying = true;
                    carryingCrate = true;
                }
                else if (Checker.ClosestTrigerrer != null && Checker.ClosestTrigerrer.CompareTag("Crate"))
                {
                    Checker.childTransfer(Carrier.transform);
                    //isCarrying = true;
                    carryingCrate = true;
                }
                delta = 0; // stops timer    
            }
        }

        if ((!isCarrying || Inventory.HasEmptySlot()) && Input.GetKeyUp(KeyCode.Space))
        {
            if (delta < interaction_time)
            {
                // pick up the POUCH
                if (Checker.ClosestTrigerrer != null && Checker.ClosestTrigerrer.CompareTag("Crate"))
                {
                    Crates crate = Checker.ClosestTrigerrer.GetComponent<Crates>();

                    if (crate.amount > 0)
                    {
                        if (Inventory.Add(crate.Type))
                        {
                            crate.amount--;
                            isCarrying = true;
                        }
                    }
                }
            }
            delta = 0; // stops timer            
        }


        // ------------------
        // is Carrying
        // ------------------
        if (isCarrying && Input.GetKeyDown(KeyCode.Space))
        {
            // POUCH
            if (!Inventory.isEmpty())
            {
                if (Checker.ClosestTrigerrer != null && Checker.ClosestTrigerrer.gameObject.layer == 9/*Ally*/)
                {
                    AllyTempBehaviour ally = Checker.ClosestTrigerrer.GetComponent<AllyTempBehaviour>();

                    if (ally.HandItem(Inventory.ItemInventory[Inventory.SelectedItem]))
                    {
                        Inventory.RemoveItem();
                    }
                }
            }
            //CRATE
            else if (carryingCrate)
            {
                if (Checker.ClosestTrigerrer != null && Checker.ClosestTrigerrer.CompareTag("CargoSlot"))
                {
                    Debug.Log("F");
                    Checker.ClosestTrigerrer.GetComponent<CargoSlot>().StoreCargo(CarriedObject);
                    carryingCrate = false;
                    //isCarrying = false;
                }
                else
                {
                    //put down box
                    // ************************* will change after engine proof *****************************
                    CarriedObject.localPosition = facing;
                    CarriedObject.SetParent(world);
                    CarriedObject.position = new Vector3(CarriedObject.position.x, CarriedObject.transform.lossyScale.y * 0.5f, CarriedObject.position.z);

                    carryingCrate = false;
                    //isCarrying = false;
                }
            }
        }

        
    }

}
