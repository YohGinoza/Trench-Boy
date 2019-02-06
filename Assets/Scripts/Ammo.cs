using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour {
    
    public Transform carry;
    public int amount = 100;
    public int pick = 10;
    public bool access = true;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(amount <= 0)
        {
            access = false;
        }    
	}

    public void take_pouch()
    {
        amount = amount - pick; 
        Debug.Log("Ammo -" + pick);
    }

    public void move()
    {
        this.transform.position = carry.position;
        this.transform.parent = GameObject.Find("Player").transform;
        this.transform.parent = GameObject.Find("carry").transform;
    }

}
