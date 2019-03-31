using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideChild : MonoBehaviour
{
    Renderer[] renderers;

    private void OnTransformChildrenChanged()
    {
        if (this.transform.childCount > 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
        }
        else
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
            }
        }
    }
}
