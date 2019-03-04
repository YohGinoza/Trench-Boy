using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    TrenchBoyController player;
    GameController gc;
    public Text speech;    
    private int i = 0;

    private void Start()
    {
        gc = GetComponent<GameController>();
    }

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
        if (dialogue[i + 1] != null)
        {
            i++;
        }
    }

}
