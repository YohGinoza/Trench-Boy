using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class News : MonoBehaviour
{
    [SerializeField] private Sprite[] header = new Sprite[2];
    [SerializeField] private Sprite[] slot1 = new Sprite[3];
    [SerializeField] private Sprite[] slot2 = new Sprite[3];

    [SerializeField] private Image[] slot = new Image[3];
    
    public int n_AllyRemainLimit;
    public int n_GrenadeHit;
    public int n_AllyHelp;



    public float UI_speed;

    GameController gc;

    GameObject photograph;
    GameObject mainmenuButton;

    public bool ui_up = false;
    public bool ui_down = false;

    public int Allies_Remaining;


    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.Find("GameControl").GetComponent<GameController>();
        photograph = GameObject.Find("Photograph");

        mainmenuButton = this.transform.Find("MainMenuButton").gameObject;
        mainmenuButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.loseCondition == LoseCondition.BarbedWire || GameController.loseCondition == LoseCondition.AllDead)
        {
            this.transform.localPosition = new Vector3(0.0f, this.transform.localPosition.y, this.transform.localPosition.z);
            mainmenuButton.transform.localPosition = new Vector3(1000.0f, mainmenuButton.transform.localPosition.y, mainmenuButton.transform.localPosition.z);

            slot[0].sprite = header[1];
            slot[1].sprite = slot1[2];
            slot[2].sprite = slot2[0];

            photograph.SetActive(false);
        }
        else if (GameController.loseCondition == LoseCondition.None)
        {
            slot[0].sprite = header[0];
            photograph.SetActive(true);

            if (Allies_Remaining >= n_AllyRemainLimit)
            {
                slot[1].sprite = slot1[0];
            }
            else if (Allies_Remaining < n_AllyRemainLimit)
            {
                slot[1].sprite = slot1[1];
            }

            if (gc.GrenadeHit >= n_GrenadeHit)
            {
                slot[2].sprite = slot2[1];
            }
            else if (gc.HelpFriend >= n_AllyHelp)
            {
                slot[2].sprite = slot2[2];
            }
            else
            {
                slot[2].sprite = slot2[0];
            }
        }

        if (ui_up)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + UI_speed, this.transform.position.z);

            if (this.transform.localPosition.y >= 0.0f)
            {
                if (this.transform.localPosition.y > 0.0f)
                {
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, 0.0f, this.transform.position.z);
                }

                mainmenuButton.SetActive(true);
                ui_up = false;
            }
        }
        
    }

}
