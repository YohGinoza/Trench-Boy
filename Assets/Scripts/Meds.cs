using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meds : Ammo {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (amount <= 0)
        {
            access = false;
        }
    }

}
