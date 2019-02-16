using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoSlot : MonoBehaviour
{
    public enum Slots { Left, Right, Top }
    
    public Slots ThisSlot;
    [SerializeField] private CargoSlot TopSlot;
    public bool HasCargo = false;

    //for top slot only
    [System.NonSerialized] public bool HasLeftSupport;
    [System.NonSerialized] public bool HasRightSupport;


    public void StoreCargo(Transform cargo)
    {
        if (!HasCargo)
        {
            switch (ThisSlot)
            {
                case Slots.Left:
                    //change cargo parent to this
                    cargo.SetParent(this.transform);
                    cargo.localPosition = Vector3.zero;

                    TopSlot.HasLeftSupport = true;
                    HasCargo = true;
                    break;

                case Slots.Right:
                    //change cargo parent to this
                    cargo.SetParent(this.transform);
                    cargo.localPosition = Vector3.zero;

                    TopSlot.HasLeftSupport = true;
                    HasCargo = true;
                    break;

                case Slots.Top:
                    if (HasLeftSupport && HasRightSupport)
                    {
                        //change cargo parent to this
                        cargo.SetParent(this.transform);
                        cargo.localPosition = Vector3.zero;

                        HasCargo = true;
                    }
                    break;
            }
        }
        
    }

    public void TakeOffCargo(Transform lifter, Vector3 holdingPosition)
    {
        if (HasCargo)
        {
            switch (ThisSlot)
            {
                case Slots.Left:
                    if (!TopSlot.HasCargo)
                    {
                        //change parent to lifter
                        this.transform.GetChild(0).SetParent(lifter);
                        this.transform.GetChild(0).localPosition = holdingPosition;

                        TopSlot.HasLeftSupport = false;
                        HasCargo = false;
                    }
                    break;

                case Slots.Right:
                    if (!TopSlot.HasCargo)
                    {
                        //change parent to lifter
                        this.transform.GetChild(0).SetParent(lifter);
                        this.transform.GetChild(0).localPosition = holdingPosition;

                        TopSlot.HasRightSupport = false;
                        HasCargo = false;
                    }
                    break;

                case Slots.Top:
                    //change parent to lifter 
                    this.transform.GetChild(0).SetParent(lifter);
                    this.transform.GetChild(0).localPosition = holdingPosition;

                    HasCargo = false;
                    break;
            }
        }
    }
}

