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
    private bool[] talkedAboutDeath;
    // once converse, set to true and trigger line Ex
    public bool dayLimit = false;
    public bool specialTrigger = false;
    private bool readyToClearText = false;

    private void Start()
    {
        gc = GetComponent<GameController>();        
        for (int index_Special = 0; index_Special < 20; index_Special++)
        {
            if (GameController.AlliesAliveStatus[index_Special] == false)
            {
                // someone died
                // if the text is blank >>>> NOT related/close to each other = nothing to say
                if (special_text[index_Special] != "")
                {                                        
                    specialTrigger = true;
                }
            }
        }
        
    }

    void Update()
    {
        player = GetComponent<TrenchBoyController>();        
    }

    
    // set dayLimit to true after conversing once
    //+ reset dayLimit to true after every day has passed in GAMECONTROL script
    public void converse()
    {

        if (!dayLimit)
        {           
            if (specialTrigger)
            {
                // if SPECIAL
                //+ trigger this instead of NORMAL
                speech.text = special_text[index_Special]; // don't forget to delete the text
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
            if (dialogue_text[index_Ending] != null)
            {
                index_Ending++;
            }
        }
        
    }

    // turn text box on/off
    public void showText(bool show)
    {
        speech.enabled = show;
        if (readyToClearText)
        {
            special_text[index_Special] = "";
            readyToClearText = false;
        }
    }

}
