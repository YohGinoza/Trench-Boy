using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChercker : MonoBehaviour {

    public bool isInteractable;
    Transform crate;

    private void OnTriggerEnter(UnityEngine.Collider other)
    {        
        if (other.CompareTag("Crate"))
        {
            isInteractable = true;
            crate = other.transform;
        }

    }

    private void OnTriggerExit(UnityEngine.Collider other)
    {
        if (other.CompareTag("Crate"))
        {
            isInteractable = false;
            crate = null;
        }
    }

    public void childTransfer(Transform newparent)
    {
        crate.parent = newparent;
        crate.localPosition = new Vector3(0, 0, 0);        
    }

}
