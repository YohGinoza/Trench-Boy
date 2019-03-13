using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbedWire : MonoBehaviour
{
    GameController gc;
    BarbedWire bw;

   
    private bool cuttingBarbedWire = false;
    [SerializeField] private bool BarbedWireModeHP = true;

    //============
    // HP
    //============
    public float HP = 100.0f;
    
    //============
    // THRESHOLD
    //============
    private float delta = 0.0f;
    static public float CUTTING_TIME = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            cuttingBarbedWire = true;
            other.GetComponent<EnemyBehaviour>().StartCoroutine("cutBarbedWire");
        }        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && cuttingBarbedWire)
        {            
            if (BarbedWireModeHP)
            {
                //============
                // HP MODE
                //============
                HP -= Time.deltaTime;
                if (HP <= 0.0f)
                {
                    GameController.DayEnded = true;
                }
            }                    
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyBehaviour>().StopCoroutine("cutBarbedWire");
            cuttingBarbedWire = false;            
        }
    }
}
