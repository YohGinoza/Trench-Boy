using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Day,
    Stalling,
    Night,
    Wait
};

public enum LoseCondition
{
    None,
    AllDead,
    BarbedWire,
};

public enum Tutorials
{
    PickUpCrate = 0,
    PickUpItem,
    RefillingCrate,
    AllyDown,
    InventoryControl,
    GiveItem,
    AllyNight
};

public enum Day
{
    //DAY_0 = 0,
    DAY_1,
    DAY_2,
    DAY_3,
    DAY_4,
    DAY_5,
    DAY_6,
    DAY_7,
};

public enum Ally
{
    Cpt_William_Carter = 0,
    Sgt_Allen_Baker,
    Cpl_Ray_Garrett,
    Cpl_Darren_Reed,
    Pvt_Jason_Riley,
    Pvt_Roy_Watson,
    Pvt_Ellis_Anderson,
    Pvt_Andrew_Summer,
    Pvt_Tim_Barry,
    Pvt_Harper_Scott,
    Pvt_Dwayne_Manning,
    Lt_Henry_Mattis,
    Sgt_Becker_Samson
};

public enum ItemType
{
    Ammo = 20, //this number is the ammo given in pouch
    Med = 5,
    None = 0
};

public class GameController : MonoBehaviour
{
    public static int AlliesRamaining;

    public static bool[] AlliesAliveStatus = new bool[13];
    
    public static bool[] AlliesDieToday = new bool[13];
    public static bool[] AlliesDiePrev = new bool[13];

    public static string[] AlliesName = new string[]
    {
        "Cpt. William Carter",
        "Sgt. Allen Baker",
        "Cpl. Ray Garrett",
        "Cpl. Darren Reed",
        "Pvt. Jason Riley",
        "Pvt. Roy Watson",
        "Pvt. Ellis Anderson",
        "Pvt. Andrew Summer",
        "Pvt. Tim Berry",
        "Pvt. Harper Scott",
        "Pvt. Dwayne Manning",
        "Lt. Henry Mattis",
        "Sgt. Becker Samson"
    };

    //time
    public GameState CurrentState = GameState.Day;
    public Day CurrentDay = Day.DAY_1;
    [Range(0, 1)] public float TimeOfDay = 0;
    public float[] DayLenght;
    public Day DayEndLimit = Day.DAY_7;

    //enemy
    [SerializeField] private EnemySpawner[] EnemySpawners = new EnemySpawner[5];
    [SerializeField] private float EnemySpawnRate;

    //win/lose
    public static LoseCondition loseCondition = LoseCondition.None;
    public Transform lastAlly = null;
    static public bool DayEnded = false;
    static public bool DayEndedCheck = false;

    static public bool GameOver = false;

    //night
    public int NightInteractionLimit = 6;
    public int NightTimeInteractCounter = 0;
    public bool CaptainCall = false;

    private bool CoroutineRunning = false;

    //UI
    private GameObject Inventory_UI;
    private GameObject DayEnd_UI;
    private GameObject News_UI;
    [SerializeField] private TutorialUI tutorialUI;
    [SerializeField] private LetterUI letterUI;
    public static bool reset_pressed;
    public bool uidown = false;

    //Sound
    public GameObject BGmusic;
    public GameObject Endingmusic;

    public AudioClip day_START;
    public AudioClip day_END;
    public AudioClip day_finish;

    public AudioClip day_report;
    
    public AudioClip WinBG;
    public AudioClip LoseBG;

    //News
    public int GrenadeHit = 0;
    public int HelpFriend = 0;

    //camera
    private CameraController cameraController;
    //private Transform Camera;
    //[SerializeField] private Vector3 CamDayZoom;
    //[SerializeField] private Vector3 CamNightZoom;
    //[SerializeField] private float DayCamAngle;
    //[SerializeField] private float NightCamAngle;

    //Tutorial
    public static bool[] TutorialFinished = new bool[7];

    void Start()
    {
        for(int i = 0;i < AlliesAliveStatus.Length; i++)
        {
            AlliesAliveStatus[i] = true; 
        }

        for (int i = 0; i < TutorialFinished.Length; i++)
        {
            TutorialFinished[i] = false;
        }

        Inventory_UI = GameObject.Find("InventoryBar");
        DayEnd_UI = GameObject.Find("DayEndUI");
        News_UI = GameObject.Find("News");
        tutorialUI = FindObjectOfType<TutorialUI>();
        //Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        cameraController = FindObjectOfType<CameraController>();

        BGmusic = GameObject.Find("BGmusic");
        Endingmusic = GameObject.Find("Endingmusic");

        DayStart();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //advance daytime
        switch (CurrentState)
        {
            case GameState.Day:
                {
                    //set camera
                    //Camera.localPosition = CamDayZoom;
                    //Camera.localRotation = Quaternion.Euler(DayCamAngle, 0, 0);

                    Inventory_UI.SetActive(true);
                    DayEnd_UI.SetActive(false);
                    News_UI.SetActive(false);
                    BGmusic.SetActive(false);
                    Endingmusic.SetActive(false);

                    //check if any ally is alive
                    DayEnded = false;

                    //ally living check
                    bool anyAllyLeft = false;
                    for (int i = 1; i < AlliesAliveStatus.Length; i++)
                    {
                        if (AlliesAliveStatus[i])
                        {
                            //get last ally
                            lastAlly = GameObject.FindGameObjectWithTag("Ally").transform;

                            anyAllyLeft = true;
                            break;
                        }
                    }
                    if (!anyAllyLeft)
                    {
                        DayEnded = true;
                        loseCondition = LoseCondition.AllDead;
                        // focus at last ally
                        if (lastAlly != null)
                        {
                            cameraController.SetTarget(lastAlly);
                        }
                    }

                    //time check
                    if (!DayEnded)
                    {
                        TimeOfDay += Time.fixedDeltaTime / DayLenght[(int)CurrentDay];
                        if (TimeOfDay >= 1)
                        {
                            TimeOfDay = 0;
                            DayEnded = true;
                        }
                    }

                    if (DayEnded)
                    {
                        DayEnd();
                    }

                    //if barbed wire is destroyed
                    //dayended = true;
                }
                break;

            case GameState.Stalling:
                {
                    DayEndedCheck = true;

                    //check if there are any enemy left
                    GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    if (Enemies.Length > 0)
                    {
                        DayEndedCheck = false;
                    }
                    //+ focus at last enemy if win

                    //check if there are any downed ally left
                    GameObject[] Allies = GameObject.FindGameObjectsWithTag("Ally");
                    foreach (GameObject ally in Allies)
                    {
                        if (ally.GetComponent<AllyBehaviour>().CurrentState == AllyBehaviour.State.Downed)
                        {
                            DayEndedCheck = false;
                            break;
                        }
                    }

                    //ally living check
                    bool anyAllyLeft = false;
                    for (int i = 1; i < AlliesAliveStatus.Length; i++)
                    {
                        if (AlliesAliveStatus[i])
                        {
                            //get last ally
                            lastAlly = GameObject.FindGameObjectWithTag("Ally").transform;

                            anyAllyLeft = true;
                            break;
                        }
                    }
                    if (!anyAllyLeft)
                    {
                        DayEndedCheck = true;
                        GameOver = true;
                        loseCondition = LoseCondition.AllDead;
                        // focus at last ally
                        if (lastAlly != null)
                        {
                            cameraController.SetTarget(lastAlly);
                        }
                    }

                    if (DayEndedCheck && !CoroutineRunning)
                    {
                        StartCoroutine(StartNight());
                        //StartNight();
                        //CurrentState = GameState.Wait;
                    }
                }
                break;

            case GameState.Wait:


                Debug.Log("Wait");
                PauseGame();

                Inventory_UI.SetActive(false);
                BGmusic.SetActive(false);

                //show day end screen
                switch (loseCondition)
                {
                    case LoseCondition.None:
                        if(CurrentDay != DayEndLimit)
                        {
                            DayEnd_UI.SetActive(true);
                            DayEnd_UI.GetComponent<DayEndUI>().ui_up = true;
                            DayEnd_UI.transform.Find("MainMenuButton").gameObject.SetActive(true);
                            DayEnd_UI.transform.Find("DayEndButton").gameObject.SetActive(true);
                        }
                        else
                        {
                            Endingmusic.GetComponent<AudioSource>().clip = WinBG;
                            Endingmusic.SetActive(true);
                            News_UI.SetActive(true);
                            News_UI.GetComponent<News>().ui_up = true;
                        }
                        break;
                    case LoseCondition.BarbedWire:
                        /*DayEnd_UI.transform.Find("MainMenuButton").gameObject.SetActive(true);
                        DayEnd_UI.transform.Find("DayEndButton").gameObject.SetActive(false);
                        DayEnd_UI.GetComponent<DayEndUI>().addDeadList(true);*/
                        Endingmusic.GetComponent<AudioSource>().clip = LoseBG;
                        Endingmusic.SetActive(true);
                        News_UI.SetActive(true);
                        News_UI.GetComponent<News>().ui_up = true;
                        break;
                    case LoseCondition.AllDead:
                        /*DayEnd_UI.transform.Find("MainMenuButton").gameObject.SetActive(true);
                        DayEnd_UI.transform.Find("DayEndButton").gameObject.SetActive(false);
                        DayEnd_UI.GetComponent<DayEndUI>().addDeadList(true);*/
                        Endingmusic.GetComponent<AudioSource>().clip = LoseBG;
                        Endingmusic.SetActive(true);
                        News_UI.SetActive(true);
                        News_UI.GetComponent<News>().ui_up = true;
                        break;
                }

                break;

            case GameState.Night:
                //set camera
                //Camera.localPosition = CamNightZoom;
                //Camera.localRotation = Quaternion.Euler(NightCamAngle, 0, 0);

                Inventory_UI.SetActive(false);
                News_UI.SetActive(false);
                //DayEnd_UI.SetActive(false);
                BGmusic.SetActive(true);
                CaptainCall = NightTimeInteractCounter >= NightInteractionLimit;
                break;
        }

    }

    public void DayStart()
    {
        //play sfx
        this.GetComponent<AudioSource>().clip = day_START;
        this.GetComponent<AudioSource>().Play();

        //set cam target to player
        cameraController.SetTarget(GameObject.FindGameObjectWithTag("Player").transform);

        //start spawing enemy
        foreach (EnemySpawner spawner in EnemySpawners)
        {
            if (!spawner.CoroutineRunning)
            {
                spawner.StartCoroutine("SpawnEnemy");
            }
        }

        //switch all bahaviour to day and recover all ally
        GameObject[] Allies = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject ally in Allies)
        {
            AllyBehaviour DayBehaviour = ally.GetComponent<AllyBehaviour>();
            DialogueLoader NightBehaviour = ally.GetComponentInChildren<DialogueLoader>();

            //trigger day animation
            ally.GetComponentInChildren<Animator>().SetBool("Night", false);

            //switch behaviour to day
            if (DayBehaviour != null && DayBehaviour.CurrentState == AllyBehaviour.State.Healing)
            {
                //heal
                DayBehaviour.Recover();
                DayBehaviour.CurrentState = AllyBehaviour.State.Shooting;
                //refill ammo
                DayBehaviour.AmmoCount = DayBehaviour.MaxAmmo;
            }

            if (DayBehaviour != null)
            {
                DayBehaviour.enabled = true;
                //change to day position
                DayBehaviour.ChangePosition(0);

                //eneble UI
                DayBehaviour.EnteringNight = false;
            }

            if (NightBehaviour != null)
            {
                NightBehaviour.enabled = false;
            }
        }
    }


    void DayEnd()
    {

        //play sfx
        this.GetComponent<AudioSource>().clip = day_END;
        this.GetComponent<AudioSource>().Play();

        //CurrentDay++;
        //DayEnd_UI.GetComponent<DayEndUI>().Day++;

        //stop spawing enemy
        foreach (EnemySpawner spawner in EnemySpawners)
        {
            if (spawner.CoroutineRunning)
            {
                spawner.StopAllCoroutines();
                spawner.CoroutineRunning = false;
            }
        }

        //set all enemy aggressiveness to max
        EnemyBehaviour[] enemies = FindObjectsOfType<EnemyBehaviour>();
        foreach (EnemyBehaviour enemy in enemies)
        {
            enemy.Aggressiveness = 10;
        }

        Debug.Log("DAY ENDED");
        CurrentState = GameState.Stalling;
    }

    IEnumerator StartNight()
    {
        this.GetComponent<AudioSource>().clip = day_finish;
        this.GetComponent<AudioSource>().Play();

        CoroutineRunning = true;

        AlliesRamaining = 0;

        //check from AlliesAliveStatus and AlliesDiePrev
        for (int i = 1; i < AlliesAliveStatus.Length; i++)
        {
            if (AlliesAliveStatus[i])
            {
                AlliesRamaining++;
            }


            if (!AlliesAliveStatus[i] && !AlliesDiePrev[i])
            {
                AlliesDieToday[i] = true;
            }

        }

        News_UI.GetComponent<News>().Allies_Remaining = AlliesRamaining;
        DayEnd_UI.GetComponent<DayEndUI>().Allies_Remaining = AlliesRamaining;
        AlliesDiePrev = AlliesDieToday;

        for (int i = 0; i < AlliesDieToday.Length; i++)
        {
            if (AlliesDieToday[i])
            {
                DayEnd_UI.GetComponent<DayEndUI>().addDeadList(false);
                break;
            }

        }

        //for debug
        for (int i = 0; i < AlliesDieToday.Length; i++)
        {
            if (AlliesDieToday[i])
            {
                Debug.Log(AlliesName[i] + " Die to day ");
            }

        }

        //switch ally bahaviour to night 
        GameObject[] Allies = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject ally in Allies)
        {
            AllyBehaviour DayBehaviour = ally.GetComponent<AllyBehaviour>();
            DialogueLoader NightBehaviour = ally.GetComponent<DialogueLoader>();


            //except for those who is healing
            if (DayBehaviour != null && DayBehaviour.CurrentState != AllyBehaviour.State.Healing)
            {
                //trigger night animation
                ally.GetComponentInChildren<Animator>().SetBool("Night", true);
                ally.GetComponentInChildren<Animator>().Play("AllyNight");

                //switch behaviour to night
                if (DayBehaviour != null)
                {
                    DayBehaviour.EnterNight();
                    //change to night position
                    DayBehaviour.ChangePosition(1);
                    DayBehaviour.enabled = false;
                }

                if (NightBehaviour != null)
                {
                    // reset daylimit in dialogueLoader script
                    Debug.Log("resetting daylimit");
                    NightBehaviour.dayLimit = false;
                    
                    // update special dialogue
                    NightBehaviour.UpdateConverseData();

                    NightBehaviour.enabled = true;
                }
            }
        }

        //+ deal with te dead
        GameObject[] Fallens = GameObject.FindGameObjectsWithTag("Deceased");
        foreach(GameObject fallen in Fallens)
        {
            
        }

        //clear dead ally array 
        for (int i = 0; i < AlliesDieToday.Length; i++)
        {
            AlliesDieToday[i] = false;
        }

        //checked
        DayEndedCheck = false;

        cameraController.StartCoroutine("FadeInOut", true);
        yield return new WaitForSeconds(cameraController.FadeTime);

        CurrentState = GameState.Wait;

        this.GetComponent<AudioSource>().clip = day_report;

        this.GetComponent<AudioSource>().Play();

        CoroutineRunning = false;
    }

    public void Button_stageChange(int NextState)
    {
        if ((GameState)NextState != GameState.Wait)
        {
            ContinueGame();
            cameraController.StartCoroutine("FadeInOut", false);

            //set cam target to player
            cameraController.SetTarget(GameObject.FindGameObjectWithTag("Player").transform);
        }

        if ((GameState)NextState == GameState.Night)
        {
            //remove day report
            StartCoroutine(disable_UI());

            //queue in letter
            letterUI.StartCoroutine(letterUI.RecieveLetter((int)CurrentDay));

            //fade in tutorial
            if (!TutorialFinished[(int)Tutorials.AllyNight])
            {
                tutorialUI.TurnOn(Tutorials.AllyNight);
            }
        }


        DayEnd_UI.GetComponent<DayEndUI>().ui_down = true;
        CurrentState = (GameState)NextState;
    }

    public IEnumerator disable_UI()
    {
        yield return new WaitForSeconds(1.0f);
        DayEnd_UI.SetActive(false);
    }

    public void Button_MainMenu()
    {
        ContinueGame();
        SceneManager.LoadScene(0);
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
    }

    private void ContinueGame()
    {
        Time.timeScale = 1;
    }

    //void checkRemainingAllies()
    //{
    //    for (int i = 0; i < alliesamount; i++)
    //    {
    //        if (allies[i].isdead)
    //        {
    //            dead[i] = true;
    //        }
    //    }
    //}

}
