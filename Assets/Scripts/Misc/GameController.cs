using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Day,
    Night,
    Wait
};

public enum Day
{
    DAY_1 = 0,
    DAY_2,
    DAY_3,
    DAY_4,
    DAY_5,
    DAY_6,
    DAY_7,
};

public enum Ally
{
    COL_WC = 0,
    SGT_AB,
    SGT_JC,
    SGT_BS,
    CPL_RG,
    CPL_DR,
    PVT_JR,
    PVT_TB,
    PVT_RW,
    PVT_GS,
    PVT_AS,
    PVT_EA
};

public enum ItemType
{
    Ammo = 10,
    Med = 5,
    None = 0
};

public class GameController : MonoBehaviour
{

    public static int AlliesRamaining;

    public static bool[] AlliesAliveStatus = new bool[12];
    
    public static bool[] AlliesDieToday = new bool[12];
    public static bool[] AlliesDiePrev = new bool[12];

    public static string[] AlliesName = new string[]
    {
        "COL_WC",
        "SGT_AB",
        "SGT_JC",
        "SGT_BS",
        "CPL_RG",
        "CPL_DR",
        "PVT_JR",
        "PVT_TB",
        "PVT_RW",
        "PVT_GS",
        "PVT_AS",
        "PVT_EA"
    };

    //time
    public GameState CurrentState = GameState.Day;
    private Day CurrentDay = Day.DAY_1;
    [Range(0, 1)] public float TimeOfDay = 0;
    public float DayLenght = 180;

    //enemy
    [SerializeField] private EnemySpawner[] EnemySpawners = new EnemySpawner[5];
    [SerializeField] private float EnemySpawnRate;

    static public bool DayEnded = false;
    static public bool DayEndedCheck = false;

    static public bool BarbedWireDestroyed = false;

    //night
    public int NightInteractionLimit = 6;
    public int NightTimeInteractCounter = 0;
    public bool CaptainCall = false;

    //UI
    public GameObject Inventory_UI;
    public GameObject DayEnd_UI;
    public static bool reset_pressed;


    void Start()
    {
        for(int i = 0;i < AlliesAliveStatus.Length; i++)
        {
            AlliesAliveStatus[i] = true; 
        }

        Inventory_UI = GameObject.Find("InventoryBar");
        DayEnd_UI = GameObject.Find("DayEndUI");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //advance daytime
        switch (CurrentState)
        {
            case GameState.Day:

                Inventory_UI.SetActive(true);
                DayEnd_UI.SetActive(false);

                if (!DayEnded)
                {
                    TimeOfDay += Time.fixedDeltaTime / DayLenght;
                }

                if (TimeOfDay >= 1)
                {
                    TimeOfDay = 0;
                    DayEnded = true;
                }
                else
                {
                    //will move to day triggerer when we have it
                    foreach (EnemySpawner spawner in EnemySpawners)
                    {
                        if (!spawner.CoroutineRunning)
                        {
                            spawner.StartCoroutine("SpawnEnemy");
                        }
                    }
                }

                if (DayEnded)
                {
                    Debug.Log("DAY ENDED");
                    DayEndedCheck = true;
                    CurrentState = GameState.Wait;
                }

                //check if any ally is alive
                DayEnded = true;
                foreach(bool alive in AlliesAliveStatus)
                {
                    if (alive)
                    {
                        DayEnded = false;
                    }
                }

                if (DayEnded)
                {
                    Debug.Log("DAY ENDED");
                    DayEndedCheck = true;
                    CurrentState = GameState.Wait;
                }

                //if barbed wire is destroyed
                //dayended = true;

                break;
            case GameState.Night:
                Inventory_UI.SetActive(false);
                DayEnd_UI.SetActive(false);
                CaptainCall = NightTimeInteractCounter >= NightInteractionLimit;
                break;
            case GameState.Wait:
                Debug.Log("Wait");
                PauseGame();

                Inventory_UI.SetActive(false);
                DayEnd_UI.SetActive(true);

                if (DayEndedCheck)
                {
                   DayEnd();
                    if (!BarbedWireDestroyed)
                    {
                        DayEnd_UI.transform.Find("MainMenuButton").gameObject.SetActive(false);
                        DayEnd_UI.transform.Find("DayEndButton").gameObject.SetActive(true);
                        DayEnd_UI.GetComponent<DayEndUI>().addDeadList();
                    }
                    else
                    {
                        DayEnd_UI.transform.Find("MainMenuButton").gameObject.SetActive(true);
                        DayEnd_UI.transform.Find("DayEndButton").gameObject.SetActive(false);
                    }
                }

                

                break;
        }

        
    }

    void DayStart()
    {
        //start spawing enemy
        foreach (EnemySpawner spawner in EnemySpawners)
        {
            if (!spawner.CoroutineRunning)
            {
                spawner.StartCoroutine("SpawnEnemy");
            }
        }

        //switch all bahaviour to day
        GameObject[] Allies = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject ally in Allies)
        {
            AllyBehaviour DayBehaviour = ally.GetComponent<AllyBehaviour>();
            //DialogueLoader NightBehaviour;

            if (DayBehaviour != null)
            {
                DayBehaviour.enabled = true;
            }

            //if(NightBehavior != null)
            //{
            //    NightBehaviour.enabled = false;
            //}
        }
        //switch player behaviour to day
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in Players)
        {
            TrenchBoyController DayBehaviour = player.GetComponent<TrenchBoyController>();
            TrenchBoyController_NightTime NightBehaviour = player.GetComponent<TrenchBoyController_NightTime>();

            if (DayBehaviour != null)
            {
                DayBehaviour.enabled = true;
            }

            if (NightBehaviour != null)
            {
                NightBehaviour.enabled = false;
            }
        }
    }


    void DayEnd()
    {
        //switch all bahaviour to night
        GameObject[] Allies = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject ally in Allies)
        {
            AllyBehaviour DayBehaviour = ally.GetComponent<AllyBehaviour>();
            //DialogueLoader NightBehaviour;

            if (DayBehaviour != null)
            {
                DayBehaviour.enabled = false;
            }

            //if(NightBehavior != null)
            //{
            //    NightBehaviour.enabled = true;
            //}
        }
        //switch player behaviour to night
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in Players)
        {
            TrenchBoyController DayBehaviour = player.GetComponent<TrenchBoyController>();
            TrenchBoyController_NightTime NightBehaviour = player.GetComponent<TrenchBoyController_NightTime>();

            if (DayBehaviour != null)
            {
                DayBehaviour.enabled = false;
            }

            if (NightBehaviour != null)
            {
                NightBehaviour.enabled = true;
            }
        }

        //stop spawing enemy
        foreach (EnemySpawner spawner in EnemySpawners)
        {
            if (spawner.CoroutineRunning)
            {
                spawner.StopAllCoroutines();
            }
        }


        //clear array 
        for (int i = 0; i < AlliesDieToday.Length; i++)
        {
            AlliesDieToday[i] = false;
        }

        AlliesRamaining = 0;
        
        //check from AlliesAliveStatus and AlliesDiePrev
        for (int i = 0;i < AlliesAliveStatus.Length; i++)
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

        AlliesDiePrev = AlliesDieToday;

        //for debug
        for (int i = 0; i < AlliesDieToday.Length; i++)
        {
            if (AlliesDieToday[i])
            {
                Debug.Log(AlliesName[i] + " Die to day ");
            }
            
        }

        //checked
        DayEndedCheck = false;

    }

    public void Button_stageChange(int NextState)
    {
        if((GameState)NextState != GameState.Wait)
        {
            ContinueGame();
        }

        if((GameState)NextState == GameState.Day)
        {
            SceneManager.LoadScene("Angled3D");
            
        }

        if ((GameState)NextState == GameState.Night)
        {
            //SceneManager.LoadScene("Angled3D");
        }

        CurrentState = (GameState)NextState;
    }

    public void Button_MainMenu()
    {
        ContinueGame();
        SceneManager.LoadScene("MainMenu");
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
