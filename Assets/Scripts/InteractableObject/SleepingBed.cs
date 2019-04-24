using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingBed : MonoBehaviour
{
    CameraController camControl;

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
        if (controller.CurrentState == GameState.Night)
        {
            //fade to black
            camControl.StartCoroutine("FadeInOut", true);
            yield return new WaitForSecondsRealtime(camControl.FadeTime);

            controller.DayStart();
            controller.CurrentState = GameState.Day;

            //fade to normal
            camControl.StartCoroutine("FadeInOut", false);
            yield return new WaitForSecondsRealtime(camControl.FadeTime);
        }
    }
}
