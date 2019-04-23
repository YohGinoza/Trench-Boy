using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedHighlight : MonoBehaviour
{
    [SerializeField] private Color SelectedEmisionColor = new Color(0.3f, 0.3f, 0.3f);

    private Renderer[] ChildrenRenderers;

    [System.NonSerialized] public bool selected = false;

    private void Start()
    {
        ChildrenRenderers = GetComponentsInChildren<Renderer>();
    }

    public void Select()
    {
        selected = true;
        switch (this.tag)
        {
            case "Ally":
                transform.GetChild(transform.childCount - 1).GetComponent<Renderer>().enabled = true;
                break;
            default:
                foreach (Renderer renderer in ChildrenRenderers)
                {
                    //Material newMat = renderer.material;
                    renderer.material.SetColor("_EmissionColor", SelectedEmisionColor);
                    renderer.material.EnableKeyword("_EMISSION");
                    //renderer.material = newMat;
                }
                break;
        }
    }

    public void UnSelect()
    {
        selected = false;
        switch (this.tag)
        {
            case "Ally":
                transform.GetChild(transform.childCount - 1).GetComponent<Renderer>().enabled = false;
                break;
            default:
                foreach (Renderer renderer in ChildrenRenderers)
                {
                    //Material newMat = renderer.material;
                    renderer.material.SetColor("_EmissionColor", Color.black);
                    renderer.material.DisableKeyword("_EMISSION");
                    //renderer.material = newMat;
                }
                break;
        }
    }
}
