using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
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

    void checkRemainingAllies()
    {
        for(int i = 0; i < alliesAmount; i++)
        {
            if(allies[i].isDead)
            {
                dead[i] = true;
            }
        }
    }

}
