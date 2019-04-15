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
            Material newMat = renderer.material;
            newMat.SetColor("_EmissionColor", SelectedEmisionColor);
            newMat.EnableKeyword("_EMISSION");
            renderer.material = newMat;
        }
    }

    public void UnSelect()
    {
        foreach (Renderer renderer in ChildrenRenderers)
        {
            Material newMat = renderer.material;
            newMat.SetColor("_EmissionColor", Color.black);
            newMat.DisableKeyword("_EMISSION");
            renderer.material = newMat;
        }
    }
}
