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

    private void FixedUpdate()
    {

    }

    public void Refill()
    {
        Amount = MaxAmount;

        if (NotEmpty != null)
        {
            //GetComponent<Renderer>().material.color = NormalColor;
            TopSide.material = NotEmpty;
        }
    }

    public void PickOut()
    {
        if (Amount > 0)
        {
            this.GetComponent<AudioSource>().Play();
            Amount--;
        }

        if (Amount <= 0)
        {
            if (Empty != null)
            {
                //GetComponent<Renderer>().material.color = Color.white;
                TopSide.material = Empty;
            }
        }
    }
}
