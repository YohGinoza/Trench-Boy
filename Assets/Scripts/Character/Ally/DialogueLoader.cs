using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueLoader : MonoBehaviour
{
    //public event Action talkEvent; //trigger everything under that event
    [SerializeField] private Ally ThisPerson;
    
    public string[,] dialogue_text = new string[7,20];
    public string[,] ending_text = new string[5,1];
    public string[,] special_text = new string[20,16];

    public bool[,] D_NPCSpeaking = new bool[7,20];    
    public bool[,] S_NPCSpeaking = new bool[20,10];

    public int index_Friendship = 0;
    public int index_Ending = 0;

    GameObject player;
    GameController gc;

    public GameObject Canvas;
    public Text NPC;
    public Text PLAYER;
    public Image iNPC;
    public Image iPLAYER;

    private Animator animator;

    private int lineCounter = 0;
    // once converse, set to true and trigger line Ex
    public bool dayLimit = false;
    public bool specialTrigger = false;    

    [SerializeField] private Ally[] friends;
    private int diedToday;
    
    bool[] dietoday; // ??????????????????????????????????????????????????????????? day_end script
    //======================================================

    public const string path = "dialogues";  

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = this.GetComponentInChildren<Animator>();

        NPC = Canvas.transform.GetChild(3).GetComponentInChildren<Text>();
        iNPC = Canvas.transform.GetChild(3).GetComponent<Image>();
        //get component from player canvas
        PLAYER = player.transform.GetChild(4).GetChild(1).GetComponentInChildren<Text>();
        iPLAYER = player.transform.GetChild(4).GetChild(1).GetComponent<Image>();

        gc = GetComponent<GameController>();

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
                    ending_text[dialogue.id,dialogue.line] = dialogue.text;
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

    void UpdateConverseData()
    {
        dietoday = GameController.AlliesDieToday;

        for (int i = 0; i < friends.Length; i++)
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
        Text speaker;
        Image iSpeaker;
        if (!dayLimit)
        {
            if (specialTrigger)
            {
                // if SPECIAL
                //+ trigger this instead of NORMAL

                // set the speaker
                if (S_NPCSpeaking[diedToday, lineCounter])
                {
                    speaker = NPC;
                    iSpeaker = iNPC;
                    ShowSpeechBubble(true);
                }
                else
                {
                    speaker = PLAYER;
                    iSpeaker = iPLAYER;
                    ShowSpeechBubble(false);
                }

                // text body
                speaker.text = special_text[diedToday,lineCounter];

                // check further conversation
                if (special_text[diedToday, lineCounter + 1] == null)
                {
                    lineCounter++;
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
                // set the speaker
                if(D_NPCSpeaking[index_Friendship, lineCounter])
                {
                    speaker = NPC;
                    iSpeaker = iNPC;
                    ShowSpeechBubble(true);
                }
                else
                {
                    speaker = PLAYER;
                    iSpeaker = iPLAYER;
                    ShowSpeechBubble(false);
                }
                // if NORMAL

                speaker.text = dialogue_text[index_Friendship, lineCounter];
                if (dialogue_text[index_Friendship, lineCounter+1] != null)
                {                    
                    lineCounter++;
                }
                else
                {
                    // no more informational conversation
                    dayLimit = true;
                    lineCounter = 0;
                    index_Friendship++;
                }                
            }
            //gc.NightTimeInteractCounter++; // increment night limit counter
        }
        else
        {
            // ending lines / goodbye
            // NPC one-liner
            if (ending_text[index_Ending, lineCounter] != null)
            {
                ShowSpeechBubble(true);
                NPC.text = ending_text[index_Ending, lineCounter];
            }

            if (ending_text[index_Ending+1,lineCounter] != null)
            {
                index_Ending++;                
            }
            else
            {
                NPC.enabled = false;
                iNPC.enabled = false;
            }
        }
    }
    
    private void ShowSpeechBubble(bool NPCTalking)
    {
        PLAYER.enabled = !NPCTalking;
        iPLAYER.enabled = !NPCTalking;
        NPC.enabled = NPCTalking;
        iNPC.enabled = NPCTalking;

        //play animation
        if (NPCTalking)
        {
            //(I) you may play npc talking sound here
            player.GetComponent<TrenchBoyController>().audioSource.Stop();

            this.GetComponent<AllyBehaviour>().callSource.clip = this.GetComponent<AllyBehaviour>().converse;
            this.GetComponent<AllyBehaviour>().callSource.Play();
            animator.SetTrigger("Talk");
        }
        else
        {
            this.GetComponent<AllyBehaviour>().callSource.Stop();

            player.GetComponent<TrenchBoyController>().audioSource.clip = player.GetComponent<TrenchBoyController>().converse;
            player.GetComponent<TrenchBoyController>().audioSource.Play();

            player.GetComponentInChildren<Animator>().SetBool("Talking", true);
            player.GetComponentInChildren<Animator>().SetTrigger("Talk");
        }
    }
}