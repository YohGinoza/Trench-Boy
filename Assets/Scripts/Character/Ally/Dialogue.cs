using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    TrenchBoyController player;
    GameController gc;
    public Text speech;    
    private int index_Friendship = 0;
    private int index_Ending = 0;
    private int index_Special = 0; // needs info from game controller

    // once converse, set to true and trigger line Ex
    public bool dayLimit = false;
    public bool specialTrigger = false;

    private void Start()
    {
        gc = GetComponent<GameController>();
    }

    void Update()
    {
        player = GetComponent<TrenchBoyController>();        
    }

    [TextArea(3,10)]
    public string[] dialogue_text;
    public string[] ending_text;
    public string[] special_text;
    
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

                // no more informational conversation
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
    }

}
