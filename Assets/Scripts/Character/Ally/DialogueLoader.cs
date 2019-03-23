using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueLoader : MonoBehaviour
{
    //public event Action talkEvent; //trigger everything under that event
    [SerializeField] private Ally ThisPerson;

    [TextArea(3, 10)]
    public string[,] dialogue_text = new string[3,10];
    public string[] ending_text;
    public string[,] special_text;

    public bool[,] D_NPCSpeaking = new bool[3,10];    
    public bool[,] S_NPCSpeaking;

    public int index_Friendship = 0;
    public int index_Ending = 0;

    public string[,] D = new string[7, 30];
    

    TrenchBoyController player;
    GameController gc;
    public Text speech;
    private int lineCounter = 0;
    // once converse, set to true and trigger line Ex
    public bool dayLimit = false;
    public bool specialTrigger = false;
    private bool readyToClearText = false;

    [SerializeField] private Ally[] friends;
    private int diedToday;
    
    bool[] dietoday; // ??????????????????????????????????????????????????????????? day_end script
    //======================================================

    public const string path = "Test";  

    private void Start()
    {
        gc = GetComponent<GameController>();

        for (int i = 0; i < friends.Length; i++)
        {
            if (dietoday[(int)friends[i]])
            {
                specialTrigger = true;
                diedToday = i;
                break;
            }
        }

        DialogueContainer dc = DialogueContainer.Load(path);

        foreach(Dialogue dialogue in dc.dialogues)
        {
            if (dialogue.who == (int)ThisPerson)
            {      
                if(dialogue.type == "D")
                {
                    dialogue_text[dialogue.id, dialogue.line] = dialogue.text;
                    D_NPCSpeaking[dialogue.id, dialogue.line] = dialogue.speaker == "NPC";
                }
                else if (dialogue.type == "E")
                {
                    ending_text[dialogue.id] = dialogue.text;
                }
                else if (dialogue.type == "S")
                {
                    special_text[dialogue.id, dialogue.line] = dialogue.text;
                    S_NPCSpeaking[dialogue.id, dialogue.line] = dialogue.speaker == "NPC";
                }
                else
                {
                    print("UYA");
                }
            }
        }

        //========================================================
    }

    private void Update()
    { 
        
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
                speech.text = special_text[diedToday,lineCounter++]; // don't forget to delete the text
                // no more informational conversation
                readyToClearText = true;
                if (special_text[diedToday, lineCounter] == null)
                {
                    
                }
                else
                {
                    specialTrigger = false;
                    dayLimit = true;
                    lineCounter = 0;
                }
            }
            else
            {
                // if NORMAL
                speech.text = dialogue_text[index_Friendship, lineCounter];
                if (dialogue_text[index_Friendship, lineCounter] != null)
                {
                    index_Friendship++;
                    lineCounter++;                    
                }
                else
                {
                    // no more informational conversation
                    dayLimit = true;
                    lineCounter = 1;
                }
            }
            gc.NightTimeInteractCounter++; // increment night limit counter
        }
        else
        {
            // ending lines
            // goodbye quotes
            speech.text = ending_text[index_Ending];
            if (ending_text[index_Ending + 1] != null)
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