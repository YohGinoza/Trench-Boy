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
                        this.transform.Find("Allen").gameObject.SetActive(false);
                        break;
                    case 2:
                        this.transform.Find("Ray").gameObject.SetActive(false);
                        break;
                    case 3:
                        this.transform.Find("Darren").gameObject.SetActive(false);
                        break;
                    case 4:
                        this.transform.Find("Jason").gameObject.SetActive(false);
                        break;
                    case 5:
                        this.transform.Find("Roy").gameObject.SetActive(false);
                        break;
                    case 6:
                        this.transform.Find("Ellis").gameObject.SetActive(false);
                        break;
                    case 7:
                        this.transform.Find("Andrew").gameObject.SetActive(false);
                        break;
                    case 8:
                        this.transform.Find("Tim").gameObject.SetActive(false);
                        break;
                    case 9:
                        this.transform.Find("Harper").gameObject.SetActive(false);
                        break;
                    case 10:
                        this.transform.Find("Dwayne").gameObject.SetActive(false);
                        break;
                    case 11:
                        this.transform.Find("Henry").gameObject.SetActive(false);
                        break;
                    case 12:
                        this.transform.Find("Becker").gameObject.SetActive(false);
                        break;
                }
            }
        }




    }
}
