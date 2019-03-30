using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastChecker : MonoBehaviour
{
    RaycastHit hit;
    TrenchBoyController player;
    float raycastDistance;
    
    void Start()
    {
        raycastDistance = 1.20f; 
        player = this.GetComponentInParent<TrenchBoyController>();
    }
    
    void Update()
    {
        if (Physics.Raycast(transform.position, player.facing, out hit, raycastDistance))
        {
            //Debug.Log("found collider");
            player.ColliderInfront = true;
        }
        else
        {
            player.ColliderInfront = false;
        }
    }
}
