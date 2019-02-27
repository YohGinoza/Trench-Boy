using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillCircle : MonoBehaviour
{

    private Image fillImg;
    private bool fillComplete = false;
    private bool canFill = false;
    private float fill;
    private float UIdelta;

    ColliderChercker crate_collider;

    void Start()
    {
        crate_collider = GameObject.Find("Player").GetComponentInChildren<ColliderChercker>();
        fillImg = gameObject.GetComponent<Image>();
    }

    void Update()
    {

        //------ Get delta and interaction_time from player --------------
        UIdelta = GameObject.Find("Player").GetComponent<TrenchBoyController>().delta;
        fill = UIdelta / GameObject.Find("Player").GetComponent<TrenchBoyController>().interaction_time;


        //------ Check if Interactacle or not ------
        /*if (!crate_collider.isInteractable)
        {
            delta = 0;
            fillImg.fillAmount = 0;
            canFill = false;
        }
        else if (crate_collider.isInteractable)
        {
            canFill = true;
        }*/
        //------------------------------------------
        

        if (UIdelta == 0) //if delta is 0 reset fill amount 
        {
            fillImg.fillAmount = 0;
            fillComplete = false;
        }
        else if (fillImg.fillAmount < 1 && !fillComplete) //if fill amout is less than 1, fill incomplete and canfill start fill circle
        {
            fillImg.fillAmount = fill; 
        }
        else if(fillImg.fillAmount >= 1) //if fill amout is more or = 1, fill complete and reset fill amount 
        {
            fillComplete = true;
            fillImg.fillAmount = 0;
        }
    }
}
