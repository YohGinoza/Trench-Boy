using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTravel : MonoBehaviour
{
    [SerializeField] private KeyCode ActivationKey;
    [SerializeField] private GameController gameController;

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(ActivationKey))
        {
            gameController.TimeOfDay = 0.99f;
        }
    }
}
