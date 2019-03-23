using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    //public event Action talkEvent; //trigger everything under that event

    [TextArea(3, 10)]
    public string[] dialogue_text;
    public string[] ending_text;
    public string[] special_text;

    TrenchBoyController player;
    GameController gc;
    public Text speech;    
    private int index_Friendship = 0;
    private int index_Ending = 0;
    private int index_Special = 0;    
    // once converse, set to true and trigger line Ex
    public bool dayLimit = false;
    public bool specialTrigger = false;
    private bool readyToClearText = false;

    [SerializeField] private Ally[] friends;
    private int diedToday;
    
    bool[] dietoday; // ??????????????????????????????????????????????????????????? day_end script
    private void Start()
    {
        gc = GetComponent<GameController>();     
        
        for(int i =0;i<friends.Length;i++)
        {
            if (dietoday[(int)friends[i]])
            {
                specialTrigger = true;
                diedToday = i;
                break;
            }
        }        
    }

    // set dayLimit to true after conversing once
    //+ reset dayLimit to true after every day has passed in GAMECONTROL script
    public void converse()
    {
        if (!speech.enabled)
        {
            speech.enabled = true;
        }
        if (!dayLimit)
        {           
            if (specialTrigger)
            {
                // if SPECIAL
                //+ trigger this instead of NORMAL
                speech.text = special_text[diedToday]; // don't forget to delete the text
                // no more informational conversation
                readyToClearText = true;
                specialTrigger = false;
                dayLimit = true;
            }
            else
            {
                // if NORMAL
                speech.text = dialogue_text[index_Friendship];
                if (dialogue_text[index_Friendship + 1] != null)
                {
                    index_Friendship++;
                }                
                // no more informational conversation
                dayLimit = true;
            }
            gc.NightTimeInteractCounter++; // increment night limit counter
        }
        else
        {
            // ending lines
            // goodbye quotes
            speech.text = dialogue_text[index_Friendship];
            if (dialogue_text[index_Ending + 1] != null)
            {
                index_Ending++;
            }
            else
            {
                speech.enabled = false;
            }
            
        }
        
    }
}
