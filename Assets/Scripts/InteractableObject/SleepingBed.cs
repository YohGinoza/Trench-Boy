using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingBed : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TrenchBoyController player = other.GetComponent<TrenchBoyController>();
            player.Bed = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TrenchBoyController player = other.GetComponent<TrenchBoyController>();
            player.Bed = null;
        }
    }

    public void Sleep()
    {
        GameController controller = FindObjectOfType<GameController>();
        if (controller.CurrentState == GameState.Night)
        {
            controller.CurrentState = GameState.Day;
        }
    }
}
