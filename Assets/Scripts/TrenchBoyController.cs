using UnityEngine;

public class TrenchBoyController : MonoBehaviour {

    // ------------------------------
    // game objects
    // ------------------------------
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject med_pouch;
    [SerializeField] private GameObject ammo_pouch;

    // ------------------------------
    // character movement
    // ------------------------------
    public bool isMovable = true;
    public bool isCarrying = false;
    public float movementSpeed = 0.0f;
    private float maxSpeed = 0.0f;
    public float interaction_time = 0.8f;
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
    private bool carryingPouch = false;
    private bool carryingCrate = false;    



    Rigidbody rb;
    ColliderChercker crate_collider;
    //Crate crate; // waiting for Crate script

    Vector3 refVector = Vector3.zero;
    Vector3 facing = Vector3.zero;
    private Vector3 carryPos = new Vector3(0.0f, 0.5f, 0.0f);

    // for checking delta time interaction
    float delta = 0;
    
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
        GameObject pouch = null; // initialize pouch
        // ------------------
        // isCarrying = FALSE
        // ------------------
        Debug.Log("carry");
        if (isCarrying == false && Input.GetKey(KeyCode.Space))
        {            
            delta += Time.deltaTime; // hold timer               
        }
        else if ((isCarrying == false && Input.GetKeyUp(KeyCode.Space)))
        {            
            if (delta < interaction_time)
            {
                // pick up the POUCH
                if (crate_collider.isInteractable)
                {
                    isMovable = false;  // prevent moving & picking at the same time
                    // spawn pouch in "carry position"
                    pouch = Instantiate(med_pouch, carry.position, carry.rotation);
                    pouch.transform.SetParent(carry.transform);
                    isCarrying = true;
                    carryingPouch = true;
                }
                Debug.Log("Picking up ammo/med pouch");
            }
            else if (delta >= interaction_time)
            {
                // pick up the CRATE
                if (crate_collider.isInteractable)
                {
                    isMovable = false;  // prevent moving & picking at the same time
                    crate_collider.childTransfer(carry.transform);
                    isCarrying = true;
                    carryingCrate = true;
                }
                Debug.Log("Picking up ammo/med crate");
            }
            delta = 0;
            isMovable = true;
        }

        // ------------------
        // isCarrying = TRUE
        // ------------------
        if (isCarrying == true && Input.GetKey(KeyCode.Space))
        {
            // POUCH
            if (carryingPouch)
            {
                Destroy(pouch.gameObject);
                Debug.Log("Destroying pouch");
                carryingPouch = false;
            }
            if (carryingCrate)
            {
                // ************************* will change after engine proof *****************************
                carry.transform.Translate(facing);
                carry.transform.SetParent(world);                
                carry.DetachChildren();                
                carry.transform.SetParent(player.transform);
                carry.transform.localPosition = Vector3.zero;
                carry.transform.Translate(carryPos);
                carryingCrate = false;                   
            }
        }        
    }
       
}
