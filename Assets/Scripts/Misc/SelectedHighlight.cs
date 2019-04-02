using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedHighlight : MonoBehaviour
{
    [SerializeField] private Color SelectedEmisionColor = new Color(0.3f, 0.3f, 0.3f);

    private Renderer[] ChildrenRenderers;

    private void Start()
    {
        ChildrenRenderers = GetComponentsInChildren<Renderer>();
    }

    public void Select()
    {
        foreach(Renderer renderer in ChildrenRenderers)
        {
            renderer.material.SetColor("_EmissionColor", SelectedEmisionColor);
            renderer.material.EnableKeyword("_EMISSION");
        }
    }

    public void UnSelect()
    {
        foreach (Renderer renderer in ChildrenRenderers)
        {
            renderer.material.SetColor("_EmissionColor", Color.black);
            renderer.material.DisableKeyword("_EMISSION");
        }
    }
}
