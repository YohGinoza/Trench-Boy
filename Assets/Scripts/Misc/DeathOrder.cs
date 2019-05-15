using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathOrder : MonoBehaviour
{
    [SerializeField] KeyCode DeathKey;
    [SerializeField] AllyBehaviour Soul;

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(DeathKey))
        {
            Soul.Shot();
        }
    }
}
