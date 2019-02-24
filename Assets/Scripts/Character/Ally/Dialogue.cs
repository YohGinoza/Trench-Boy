using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    TrenchBoyController player;    
    public Text speech;    
    private int i = 0;    

    void Update()
    {
        player = GetComponent<TrenchBoyController>();        
    }

    [TextArea(3,10)]
    public string[] dialogue;

    public void converse()
    {
        speech.enabled = true;
        speech.text = dialogue[i];
        speech.enabled = false;
        if (dialogue[i + 1] != null)
        {
            i++;
        }               
    }

}
