using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public static bool[] AlliesAliveStatus = new bool[20];

    //time
    private GameState CurrentState = GameState.Day;
    private Day CurrentDay = Day.DAY_1;
    [Range(0, 1)] [SerializeField] private float TimeOfDay = 0;
    [SerializeField] private float DayLenght = 180;

    //enemy
    [SerializeField] private EnemySpawner[] EnemySpawners = new EnemySpawner[5];
    [SerializeField] private float EnemySpawnRate;

    public bool DayEnded = false;

    //night
    public int NightInteractionLimit = 6;
    public int NightTimeInteractCounter = 0;
    public bool CaptainCall = false;

    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //advance daytime
        switch (CurrentState)
        {
            case GameState.Day:
                TimeOfDay += Time.fixedDeltaTime / DayLenght;

                if (TimeOfDay >= 1)
                {
                    CurrentState = GameState.Wait;
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

                //check if any ally is alive
                DayEnded = true;
                foreach(bool alive in AlliesAliveStatus)
                {
                    if (alive)
                    {
                        DayEnded = false;
                    }
                }

                //if barbed wire is destroyed
                //dayended = true;

                break;
            case GameState.Night:
                CaptainCall = NightTimeInteractCounter >= NightInteractionLimit;
                break;
            case GameState.Wait:
                
                break;
        }
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
