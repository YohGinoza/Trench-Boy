using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crates : MonoBehaviour
{
    public ItemType Type;
    public int amount = 10;

    [SerializeField] private Renderer TopSide = null;
    [SerializeField] private Material Empty;
    [SerializeField] private Material NotEmpty;
    //temporary var
    private Color NormalColor;

    private void Awake()
    {
        NormalColor = GetComponent<Renderer>().material.color;
    }

    private void FixedUpdate()
    {
        //if box is empty access will be false and box will change color to white
        if (amount <= 0)
        {
            //GetComponent<Renderer>().material.color = Color.white;
            TopSide.material = Empty;
        }
        else
        {
            //GetComponent<Renderer>().material.color = NormalColor;
            TopSide.material = NotEmpty;
        }
    }
}
