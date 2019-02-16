using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Crates_Type { Ammo, Med};

public class Crates : MonoBehaviour
{
    public int amount = 100;
    public int pick = 10;
    public bool access = true;
    public Crates_Type type;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if box is empty access will be false and box will change color to white
        if (amount <= 0)
        {
            access = false;
            GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
