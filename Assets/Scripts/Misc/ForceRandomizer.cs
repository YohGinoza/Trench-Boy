using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceRandomizer : MonoBehaviour
{
    [SerializeField] private ConstantForce constantForce;
    [SerializeField] private float minForce;
    [SerializeField] private float maxForce;
    [SerializeField] private float TimeBetweenRandomize = 10;
    private float timer = 0;

    private void FixedUpdate()
    {
        if (constantForce != null)
        {
            timer += Time.fixedDeltaTime;
            if (timer > TimeBetweenRandomize)
            {
                //random
                constantForce.force = new Vector3(0, 0, Random.Range(minForce, maxForce));
                timer = 0;
            }
        }
    }
}
