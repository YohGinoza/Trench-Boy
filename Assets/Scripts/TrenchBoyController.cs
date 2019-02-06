using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TrenchBoyController : MonoBehaviour {
    
    public GameObject player;    
    public bool isMovable = true;
    public bool isCarrying = false;
    public float movementSpeed = 0.0f;
    public float maxSpeed = 0.0f;

    Transform carry;
    Meds med_crate;
    Ammo ammo_crate;
    Rigidbody rb;
    Vector3 refVector = Vector3.zero;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        med_crate = GetComponent<Meds>();
        ammo_crate = GetComponent<Ammo>();
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

    private void move()
    {
        if(isMovable == true)
        {
            if (isCarrying == true)
            {
                maxSpeed *= 80.0f / 100.0f;
            }            
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
        }       
    }
    
    private void interactions()
    {
        if (isCarrying == false && Input.GetKeyDown(KeyCode.Space))
        {
            isMovable = false;
            float holdStartTime = Time.time;
            if (Input.GetKeyUp(KeyCode.Space))
            {
                float delta = Time.time - holdStartTime;
                if(delta < 0.5f)
                {
                    // pick ammo/med pouch
                    ammo_crate.take_pouch();
                    Debug.Log("Picking up ammo/med pouch");
                }
                else if(delta >= 0.5f)
                {
                    // pick up the box itself
                    Debug.Log("Picking up ammo/med crate");
                    ammo_crate.move();
                }
            }
            isMovable = true;
        }else if(isCarrying == true && Input.GetKey(KeyCode.Space)){
            //throw or give item
        }
    }  
    
}
