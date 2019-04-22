using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChercker : MonoBehaviour {

    private Collider[] Triggerers;
    public Transform ClosestTrigerrer;
    public SelectedHighlight Selected;
    [SerializeField] private float TriggererRadius = 3;
    [SerializeField] private LayerMask TrigererLayer;

    [SerializeField] private Transform Carrier;

    private void FixedUpdate()
    {
        Triggerers = Physics.OverlapSphere(this.transform.position, TriggererRadius, TrigererLayer);

        if (ClosestTrigerrer != null)
        {
            //unselect
            if (Selected != null && Selected.selected)
            {
                Selected.UnSelect();
            }

            ClosestTrigerrer = null;
            Selected = null;
        }

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

            //trigger selected
            if (ClosestTrigerrer != null)
            {
                //find in child
                Selected = ClosestTrigerrer.GetComponentInChildren<SelectedHighlight>();
                //if not found find in the object it self
                if (Selected == null)
                {
                    Selected = ClosestTrigerrer.GetComponent<SelectedHighlight>();
                }

                if (Selected != null && !Selected.selected)
                {
                    Selected.Select();
                    Debug.Log("Found");
                }
                else
                {
                    Debug.Log("Not Forund");
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
