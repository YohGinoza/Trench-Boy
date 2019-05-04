using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbedWire : MonoBehaviour
{
    GameController gc;
    BarbedWire bw;

    private bool cuttingBarbedWire = false;
    private bool BarbedWireModeHP = true;

    //============
    // HP
    //============
    public float MaxHP = 100.0f;
    public float HP = 100.0f;
    [SerializeField] private Texture[] HPStages;
    [SerializeField] private Renderer renderer;
    private int previousHPStage = 0;
    private int currentHPStage = 0;

    [SerializeField] private Animator animator;

    //============
    // THRESHOLD
    //============
    private float delta = 0.0f;
    static public float CUTTING_TIME = 10.0f;

    private bool destroyed = false;

    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        HP = MaxHP;
    }

    private void FixedUpdate()
    {
        UpdateTexturebyHP();
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
                //UpdateTexturebyHP();
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

    private void UpdateTexturebyHP()
    {
        previousHPStage = currentHPStage;
        //set for lowest texture
        currentHPStage = 0;

        //climb up HP level
        for (int i = 0; i < HPStages.Length; i++)
        {
            if (HP > i * (MaxHP / HPStages.Length))
            {
                currentHPStage = i;
            }
        }

        if (currentHPStage == 0 || currentHPStage != previousHPStage)
        {
            //play animation
            animator.SetTrigger("Bounce");
            renderer.material.mainTexture = HPStages[currentHPStage];
        }
    }
}
