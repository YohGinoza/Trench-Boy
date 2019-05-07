using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class photograph : MonoBehaviour
{
    GameController gc;

    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.Find("GameControl").GetComponent<GameController>();
    }
    
    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < GameController.AlliesAliveStatus.Length; i++){
            if (!GameController.AlliesAliveStatus[i])
            {
                switch(i){
                    case 1:
                        this.transform.Find("Ally1").gameObject.SetActive(false);
                        break;
                    case 2:
                        this.transform.Find("Ally2").gameObject.SetActive(false);
                        break;
                    case 3:
                        this.transform.Find("Ally3").gameObject.SetActive(false);
                        break;
                    case 4:
                        this.transform.Find("Ally4").gameObject.SetActive(false);
                        break;
                    case 5:
                        this.transform.Find("Ally5").gameObject.SetActive(false);
                        break;
                    case 6:
                        this.transform.Find("Ally6").gameObject.SetActive(false);
                        break;
                    case 7:
                        this.transform.Find("Ally7").gameObject.SetActive(false);
                        break;
                    case 8:
                        this.transform.Find("Ally8").gameObject.SetActive(false);
                        break;
                    case 9:
                        this.transform.Find("Ally9").gameObject.SetActive(false);
                        break;
                    case 10:
                        this.transform.Find("Ally10").gameObject.SetActive(false);
                        break;
                    case 11:
                        this.transform.Find("Ally11").gameObject.SetActive(false);
                        break;
                    case 12:
                        this.transform.Find("Ally12").gameObject.SetActive(false);
                        break;
                }
            }
        }




    }
}
