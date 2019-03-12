using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbedWire : MonoBehaviour
{
    GameController gc;
    BarbedWire bw;

    private float delta = 0.0f;
    private bool cuttingBarbedWire = false;
    [SerializeField] private bool BarbedWireModeHP = true;
    public float HP = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }    

    void DestroyGameObject()
    {
        Destroy(bw);
    }

    void DestroyScriptInstance()
    {
        // Removes this script instance from the game object
        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            cuttingBarbedWire = true;
        }        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && cuttingBarbedWire)
        {            
            if (BarbedWireModeHP)
            {
                // HP MODE
                HP -= Time.deltaTime;
            }
            else
            {
                // THRESHOLD MODE
                delta += Time.deltaTime;
                if (delta >= 5)
                {
                    DestroyGameObject();
                    DestroyScriptInstance();
                }
            }            
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        cuttingBarbedWire = false;
        delta = 0.0f;
    }
}
