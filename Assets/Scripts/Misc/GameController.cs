﻿using System.Collections;
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
    Cpt_William_Carter = 0,
    Sgt_Allen_Baker,
    Cpl_Ray_Garrett,
    Cpl_Darren_Reed,
    Pvt_Jason_Riley,
    Pvt_Roy_Watson,
    Pvt_Ellis_Anderson,
    Pvt_Andrew_Summer,
    Pvt_Tim_Berry,
    Pvt_Harper_Scott,
    Pvt_Dwayne_Manning,
    Lt_Henry_Mattis,
    Sgt_Becker_Samson
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

    //Sound
    public GameObject BGmusic;

    //camera
    private Transform Camera;
    [SerializeField] private Vector3 CamDayZoom;
    [SerializeField] private Vector3 CamNightZoom;
    [SerializeField] private float DayCamAngle;
    [SerializeField] private float NightCamAngle;

    void Start()
    {
        DayStart();
        for(int i = 0;i < AlliesAliveStatus.Length; i++)
        {
            AlliesAliveStatus[i] = true; 
        }

        Inventory_UI = GameObject.Find("InventoryBar");
        DayEnd_UI = GameObject.Find("DayEndUI");
        Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;

        BGmusic = GameObject.Find("BGmusic");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //advance daytime
        switch (CurrentState)
        {
            case GameState.Day:
                //set camera
                Camera.localPosition = CamDayZoom;
                Camera.localRotation = Quaternion.Euler(DayCamAngle, 0, 0);
                
                Inventory_UI.SetActive(true);
                DayEnd_UI.SetActive(false);
                BGmusic.SetActive(false);

                //check if any ally is alive
                DayEnded = true;
                for (int i = 1; i < AlliesAliveStatus.Length; i++)
                {
                    if (AlliesAliveStatus[i])
                    {
                        DayEnded = false;
                    }
                }

                if (!DayEnded)
                {
                    TimeOfDay += Time.fixedDeltaTime / DayLenght;
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

                break;

            case GameState.Stalling:
                DayEndedCheck = true;

                //check if there are any enemy left
                GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
                if (Enemies.Length > 0)
                {
                    DayEndedCheck = false;
                }

                //check if there are any downed ally left
                GameObject[] Allies = GameObject.FindGameObjectsWithTag("Ally");
                foreach (GameObject ally in Allies)
                {
                    if (ally.GetComponent<AllyBehaviour>().CurrentState == AllyBehaviour.State.Downed)
                    {
                        DayEndedCheck = false;
                    }
                }

                if (DayEndedCheck)
                {
                    StartNight();
                    CurrentState = GameState.Wait;
                }
                break;

            case GameState.Wait:
                Debug.Log("Wait");
                PauseGame();

                Inventory_UI.SetActive(false);
                DayEnd_UI.SetActive(true);
                BGmusic.SetActive(false);

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
                break;

            case GameState.Night:
                //set camera
                Camera.localPosition = CamNightZoom;
                Camera.localRotation = Quaternion.Euler(NightCamAngle, 0, 0);

                Inventory_UI.SetActive(false);
                DayEnd_UI.SetActive(false);
                BGmusic.SetActive(true);
                CaptainCall = NightTimeInteractCounter >= NightInteractionLimit;
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

        //switch all bahaviour to day and recover all ally
        GameObject[] Allies = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject ally in Allies)
        {
            AllyBehaviour DayBehaviour = ally.GetComponent<AllyBehaviour>();
            DialogueLoader NightBehaviour = ally.GetComponentInChildren<DialogueLoader>();

            //trigger day animation
            ally.GetComponentInChildren<Animator>().SetBool("Night", false);

            //switch behaviour to night
            if (DayBehaviour != null && DayBehaviour.CurrentState == AllyBehaviour.State.Healing)
            {
                DayBehaviour.CurrentState = AllyBehaviour.State.Shooting;
                DayBehaviour.EnteringNight = false;
            }

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
        //stop spawing enemy
        foreach (EnemySpawner spawner in EnemySpawners)
        {
            if (spawner.CoroutineRunning)
            {
                spawner.StopAllCoroutines();
            }
        }

        Debug.Log("DAY ENDED");
        CurrentState = GameState.Stalling;
    }

    void StartNight()
    {
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
                    DayBehaviour.enabled = false;
                }

                if (NightBehaviour != null)
                {
                    NightBehaviour.enabled = true;
                }
            }
        }

        //clear array 
        for (int i = 0; i < AlliesDieToday.Length; i++)
        {
            AlliesDieToday[i] = false;
        }

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
            StartNight();
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
