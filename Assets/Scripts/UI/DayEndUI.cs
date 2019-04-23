using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayEndUI : MonoBehaviour
{
    public Text Text_Head;
    public Text Dead_List;
    public bool AddRIP;

    public int Day = 0;
    public int Allies_Remaining;

    void Start()
    {
    }
    
    void Update()
    {
        if (GameController.loseCondition != LoseCondition.BarbedWire)
        {
            Text_Head.text = "REPORT " + "DAY " + Day + "\n" +
                         "Remaining Allies = " + Allies_Remaining;
        }
        else
        {
            Text_Head.text = "BarbedWire has been Destroyed !!";
        }
        
        
    }

    

    public void addDeadList(bool alldead)
    {
        Dead_List.text = "Casualties List:\n";

        if (!alldead)
        {
            for (int i = 0; i < GameController.AlliesDieToday.Length; i++)
            {
                if (GameController.AlliesDieToday[i])
                {
                    Dead_List.text += GameController.AlliesName[i];
                    Dead_List.text += "\n";
                }

            }
        }
        else
        {
            for (int i = 0; i < GameController.AlliesName.Length; i++)
            {
                Dead_List.text += GameController.AlliesName[i];
                Dead_List.text += "\n";
            }
        }
    }
}
