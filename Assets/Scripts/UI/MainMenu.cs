using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    GameObject Credit_UI;

    public bool ui_up = false;
    public bool ui_down = false;

    public float UI_speed;

    void Start()
    {
        Credit_UI = GameObject.Find("CreditUI");
    }

    void Update()
    {
        if (ui_up)
        {
            Credit_UI.transform.position = new Vector3(Credit_UI.transform.position.x, Credit_UI.transform.position.y + UI_speed, Credit_UI.transform.position.z);

            if (Credit_UI.transform.localPosition.y >= -300.0f)
            {
                if (Credit_UI.transform.localPosition.y > -300.0f)
                {
                    Credit_UI.transform.localPosition = new Vector3(Credit_UI.transform.localPosition.x, -300.0f, Credit_UI.transform.position.z);
                }

                ui_up = false;
            }
        }

        if (ui_down)
        {
            Credit_UI.transform.position = new Vector3(Credit_UI.transform.position.x, Credit_UI.transform.position.y - UI_speed, Credit_UI.transform.position.z);

            if (Credit_UI.transform.localPosition.y <= -1500.0f)
            {
                if (Credit_UI.transform.localPosition.y < -1500.0f)
                {
                    Credit_UI.transform.localPosition = new Vector3(Credit_UI.transform.localPosition.x, -1500.0f, Credit_UI.transform.position.z);
                }

                //Credit_UI.SetActive(false);
                ui_down = false;
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Angled3D");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Credit()
    {
        ui_up = true;
    }

    public void CloseCredit()
    {
        ui_down = true;
    }


}
