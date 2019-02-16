using UnityEngine;

public class TrenchBoyController : MonoBehaviour {
    
    public GameObject player;
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


    Rigidbody rb;
    ColliderChercker cc;
    //Crate crate; // waiting for Crate script

    Vector3 refVector = Vector3.zero;

    // for checking delta time interaction
    float delta = 0;
    
    void Start () {
        rb = GetComponent<Rigidbody>();
        cc = GetComponentInChildren<ColliderChercker>();
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
            }
            // RIGHT
            if (Input.GetKey(KeyCode.RightArrow))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.right * movementSpeed, ref refVector, 0.05f, maxSpeed);
            }
            // UP
            if (Input.GetKey(KeyCode.UpArrow))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.forward * movementSpeed, ref refVector, 0.05f, maxSpeed);
            }
            // DOWN
            if (Input.GetKey(KeyCode.DownArrow))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.back * movementSpeed, ref refVector, 0.05f, maxSpeed);
            }
            //-------------------------------
            // for WASD
            if (Input.GetKey(KeyCode.A))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.left * movementSpeed, ref refVector, 0.05f, maxSpeed);
            }
            // RIGHT
            if (Input.GetKey(KeyCode.D))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.right * movementSpeed, ref refVector, 0.05f, maxSpeed);
            }
            // UP
            if (Input.GetKey(KeyCode.W))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.forward * movementSpeed, ref refVector, 0.05f, maxSpeed);
            }
            // DOWN
            if (Input.GetKey(KeyCode.S))
            {
                rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.back * movementSpeed, ref refVector, 0.05f, maxSpeed);
            }
            // =============================
        }       
    }
    
    private void interactions()
    {
        Debug.Log("carry");
        if (isCarrying == false && Input.GetKey(KeyCode.Space))
        {
            isMovable = false;
            delta += Time.deltaTime;            
            isMovable = true;
        }
        else if ((isCarrying == false && Input.GetKeyUp(KeyCode.Space)))
        {            
            if (delta < interaction_time)
            {
                // pick up the POUCH           
                Debug.Log("Picking up ammo/med pouch");
                // -----------------
            }
            else if (delta >= interaction_time)
            {
                // pick up the CRATE
                if (cc.isInteractable)
                {
                    cc.childTransfer(carry.transform);
                    isCarrying = true;
                }
                Debug.Log("Picking up ammo/med crate");
                // ------------------
            }
            delta = 0;
        }
        else if (isCarrying == true && Input.GetKey(KeyCode.Space))
        {
            //throw or give item
        }        
    }  
    
}
