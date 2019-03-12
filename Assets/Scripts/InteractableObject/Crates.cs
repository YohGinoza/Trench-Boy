using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crates : MonoBehaviour
{
    public ItemType Type;
    [SerializeField] private int MaxAmount = 10;
    public int Amount = 10;

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
        if (Amount <= 0)
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

    public void Refill()
    {
        Amount = MaxAmount;
    }
}
