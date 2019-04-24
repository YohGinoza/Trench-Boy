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

    private bool destroyed = false;

    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
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
            if (!BarbedWireModeHP)
            {
                other.GetComponent<EnemyBehaviour>().StartCoroutine("cutBarbedWire");
            }
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
                if (HP <= 0.0f && !destroyed)
                {
                    //GameController.DayEnded = true;
                    StartCoroutine(Destroyed());
                }
            }                    
        }        
    }

    IEnumerator Destroyed()
    {
        destroyed = true;

        CameraController cameraController = FindObjectOfType<CameraController>();
        cameraController.SetTarget(this.transform);

        //let the player look
        cameraController.StartCoroutine("FadeInOut", true);
        yield return new WaitForSeconds(1.5f);

        gc.CurrentState = GameState.Wait;
        GameController.loseCondition = LoseCondition.BarbedWire;
        GameController.GameOver = true;
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
