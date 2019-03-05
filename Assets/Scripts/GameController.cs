using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    enum DAY_NO
    {
        DAY_1 = 0,
        DAY_2,
        DAY_3,
        DAY_4,
        DAY_5,
        DAY_6,
        DAY_7,
    }

    enum ALLIES
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
    }

    public int day = 0;
    public AllyTempBehaviour[] allies;
    public bool[] dead;
    public int alliesAmount = 0;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < alliesAmount; i++)
        {
            allies[i] = GetComponent<AllyTempBehaviour>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
