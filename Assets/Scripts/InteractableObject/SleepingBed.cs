using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingBed : MonoBehaviour
{
    CameraController camControl;
    bool slept = false;

    private void Start()
    {
        camControl = FindObjectOfType<CameraController>();
    }

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

    public IEnumerator Sleep()
    {
        GameController controller = FindObjectOfType<GameController>();
        if (controller.CurrentState == GameState.Night && !slept)
        {
            slept = true;

            //fade to black
            camControl.StartCoroutine(camControl.FadeInOut(true));
            yield return new WaitForSecondsRealtime(camControl.FadeTime + 0.5f);

            //move to next day
            controller.CurrentDay++;
            //start day
            controller.DayStart();
            controller.CurrentState = GameState.Day;

            //fade to normal
            camControl.StartCoroutine(camControl.FadeInOut(false)) ;
            yield return new WaitForSecondsRealtime(camControl.FadeTime);

            slept = false;
        }

    }
}
