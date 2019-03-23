using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayEndUI : MonoBehaviour
{
    public Text Text_Head;
    public Text Dead_List;
    public bool AddRIP;
    
    void Start()
    {
        Text_Head = GameObject.Find("Text_Head").GetComponent<Text>();
        Dead_List = GameObject.Find("Dead_List").GetComponent<Text>();
    }
    
    void Update()
    {
        Text_Head.text = "Day End Result \n" +
                         "Remaining Allies = " + GameController.AlliesRamaining +
                         "\n\nDie today:";
        
    }

    

    public void addDeadList()
    {
        Dead_List.text = "";

        for (int i = 0; i < GameController.AlliesDieToday.Length; i++)
        {
            if (GameController.AlliesDieToday[i])
            {
                Dead_List.text += GameController.AlliesName[i];
                Dead_List.text += "\n";
            }

        }
    }
}
