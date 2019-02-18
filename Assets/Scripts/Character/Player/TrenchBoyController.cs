using UnityEngine;

public class TrenchBoyController : MonoBehaviour {
    
    public GameObject player;    
    public bool isMovable = true;
    public bool isCarrying = false;
    public float movementSpeed = 0.0f;
    public float maxSpeed = 0.0f;
    public float interaction_time = 0.8f;
    public float carrySpeed = 0.0f;
    public float defaultSpeed = 0.0f;

    public Transform carry;
    Rigidbody rb;
    ColliderChercker cc;
    //Crate crate;
    Vector3 refVector = Vector3.zero;
    float delta = 0;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        cc = GetComponentInChildren<ColliderChercker>();
    //    crate = GetComponent<Crate>();
	}
	
	// Update is called once per frame
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
            if (isCarrying == true)
            {
                maxSpeed = carrySpeed;
            }
            else
            {
                maxSpeed = defaultSpeed;
            }    
            // ↑↓→←
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
            //========================
            // WASD
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
                // pick ammo/med pouch                    
                Debug.Log("Picking up ammo/med pouch");
            }
            else if (delta >= interaction_time)
            {
                // pick up the box itself
                if (cc.isInteractable)
                {
                    cc.childTransfer(carry.transform);
                    isCarrying = true;
                }
                Debug.Log("Picking up ammo/med crate");
            }
            delta = 0;
        }
        else if (isCarrying == true && Input.GetKey(KeyCode.Space))
        {
            //throw or give item
        }        
    }  
    
}
