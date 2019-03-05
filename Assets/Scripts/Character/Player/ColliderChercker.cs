using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChercker : MonoBehaviour {

    private Collider[] Triggerers;
    public Transform ClosestTrigerrer;
    [SerializeField] private float TriggererRadius = 3;
    [SerializeField] private LayerMask TrigererLayer;

    [SerializeField] private Transform Carrier;

    private void FixedUpdate()
    {
        Triggerers = Physics.OverlapSphere(this.transform.position, TriggererRadius, TrigererLayer);

        ClosestTrigerrer = null;
        if (Triggerers.Length > 0)
        {
            float FarthestDistance = 100;
            foreach (Collider Triggerer in Triggerers)
            {
                float distance = (Triggerer.transform.position - this.transform.position).magnitude;
                if (Triggerer.transform.parent != Carrier && distance <= FarthestDistance)
                {
                    FarthestDistance = distance;
                    ClosestTrigerrer = Triggerer.transform;
                }
            }
        }
    }

    /*private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.CompareTag("CargoSlot") || other.CompareTag("Crate"))
        {
            isTriggered = true;
            Triggerer = other.transform;
        }
    }

    private void OnTriggerExit(UnityEngine.Collider other)
    {
        if (other.CompareTag("CargoSlot") || other.CompareTag("Crate"))
        {
            isTriggered = false;
            Triggerer = null;
        }
    }*/

    public void childTransfer(Transform newparent)
    {
        ClosestTrigerrer.parent = newparent;
        ClosestTrigerrer.localPosition = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, TriggererRadius);
    }
}
