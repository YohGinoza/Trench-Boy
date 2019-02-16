using UnityEngine;

public class TrenchBoyController : MonoBehaviour {

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
    public Transform carry;
    [SerializeField] private Transform world;

    // ------------------------------
    // POUCH or CRATE
    // ------------------------------
    private float carryDelay = 0.8f;
    private bool cooldown = false;
    private bool carryingPouch = false;
    private bool carryingCrate = false;
    public float interaction_time = 0.8f;



    Rigidbody rb;
    ColliderChercker crate_collider;
    //Crate crate; // waiting for Crate script

    Vector3 refVector = Vector3.zero;
    Vector3 facing = Vector3.zero;
    private Vector3 carryPos = new Vector3(0.0f, 0.5f, 0.0f);

    // for checking time interaction
    float delta = 0.0f; // button hold timer
    float delay = 0.0f; // delay between interactions
    
    void Start () {
        rb = GetComponent<Rigidbody>();
        crate_collider = GetComponentInChildren<ColliderChercker>();
     //   crate = GetComponent<Crate>(); // wating for Crate script
	}
		
	void Update ()
    {
        if(isMovable == true)
        {
            move();            
        }
        interactions();
        if (cooldown)
        {
            delay += Time.deltaTime;
            Debug.Log("on cooldown");
            if(delay >= carryDelay)
            {
                Debug.Log("off cooldown");
                cooldown = false;
                delay = 0.0f;
            }
        }
        //Debug.Log(pouch);
    }

    // ↑↓→← WASD
    private void move()
    {
        if(isMovable == true)
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
            // for ↑↓→←
            // LEFT
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.left * movementSpeed, ref refVector, 0.05f, maxSpeed);
                facing = Vector3.left;
            }
            // RIGHT
            if (Input.GetKey(KeyCode.RightArrow))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.right * movementSpeed, ref refVector, 0.05f, maxSpeed);
                facing = Vector3.right;
            }
            // UP
            if (Input.GetKey(KeyCode.UpArrow))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.forward * movementSpeed, ref refVector, 0.05f, maxSpeed);
                facing = Vector3.forward;
            }
            // DOWN
            if (Input.GetKey(KeyCode.DownArrow))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.back * movementSpeed, ref refVector, 0.05f, maxSpeed);
                facing = Vector3.back;
            }
            //-------------------------------
            // for WASD
            if (Input.GetKey(KeyCode.A))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.left * movementSpeed, ref refVector, 0.05f, maxSpeed);
                facing = Vector3.left;
            }
            // RIGHT
            if (Input.GetKey(KeyCode.D))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.right * movementSpeed, ref refVector, 0.05f, maxSpeed);
                facing = Vector3.right;
            }
            // UP
            if (Input.GetKey(KeyCode.W))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.forward * movementSpeed, ref refVector, 0.05f, maxSpeed);
                facing = Vector3.forward;
            }
            // DOWN
            if (Input.GetKey(KeyCode.S))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.back * movementSpeed, ref refVector, 0.05f, maxSpeed);
                facing = Vector3.back;
            }
            // =============================
        }       
    }
    
    private void interactions()
    {        
        // ------------------
        // isCarrying = TRUE
        // ------------------
        if (isCarrying && Input.GetKey(KeyCode.Space))
        {
            // POUCH
            if (carryingPouch)
            {
                Destroy(pouch.gameObject); // delete pouch                
                carryingPouch = false;                
            }
            if (carryingCrate)
            {
                // ************************* will change after engine proof *****************************                
                carry.transform.localPosition = facing;                
                carry.transform.SetParent(world);
                carry.transform.localPosition = new Vector3(carry.position.x, 0.25f, carry.position.z);
                carry.DetachChildren();
                carry.transform.SetParent(player.transform);
                carry.transform.localPosition = carryPos;
                carryingCrate = false;                
            }
            isCarrying = false;
            cooldown = true;
        }

        // ------------------
        // isCarrying = FALSE
        // ------------------        
        if (!isCarrying && Input.GetKey(KeyCode.Space))
        {            
            delta += Time.deltaTime; // hold timer starts after pressing the button              
        }

        if (!isCarrying && !cooldown && Input.GetKeyUp(KeyCode.Space))
        {            
            if (delta < interaction_time)
            {                
                // pick up the POUCH
                if (crate_collider.isInteractable)
                {                    
                    // spawn pouch in "carry position"
                    pouch = Instantiate(med_pouch, carry.position, carry.rotation);
                    pouch.transform.SetParent(carry.transform);
                    isCarrying = true;
                    carryingPouch = true;
                }                
            }
            else if (delta >= interaction_time)
            {
                // pick up the CRATE
                if (crate_collider.isInteractable)
                {                    
                    crate_collider.childTransfer(carry.transform);
                    isCarrying = true;
                    carryingCrate = true;
                }                
            }
            delta = 0; // stops timer            
        }                
    }
       
}
