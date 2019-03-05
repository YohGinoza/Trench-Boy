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
   /*[System.NonSerialized]*/ public bool HasLeftSupport;
   /*[System.NonSerialized]*/ public bool HasRightSupport;


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
                    cargo.gameObject.layer = 0;

                    TopSlot.HasLeftSupport = true;
                    HasCargo = true;
                    break;

                case Slots.Right:
                    //change cargo parent to this
                    cargo.SetParent(this.transform);
                    cargo.localPosition = Vector3.zero;
                    cargo.gameObject.layer = 0;

                    TopSlot.HasRightSupport = true;
                    HasCargo = true;
                    break;

                case Slots.Top:
                    if (HasLeftSupport && HasRightSupport)
                    {
                        //change cargo parent to this
                        cargo.SetParent(this.transform);
                        cargo.localPosition = Vector3.zero;
                        cargo.gameObject.layer = 0;

                        HasCargo = true;
                    }
                    break;
            }
        }
        
    }

    public void TakeOffCargo(Transform lifter, Vector3 localHoldingPosition)
    {
        if (HasCargo)
        {
            HasCargo = false;

            switch (ThisSlot)
            {
                case Slots.Left:
                    if (!TopSlot.HasCargo)
                    {
                        //change parent to lifter
                        Transform childobj = this.transform.GetChild(0);
                        childobj.gameObject.layer = 10;
                        childobj.SetParent(lifter);
                        childobj.localPosition = localHoldingPosition;

                        TopSlot.HasLeftSupport = false;
                    }
                    break;

                case Slots.Right:
                    if (!TopSlot.HasCargo)
                    {
                        //change parent to lifter
                        Transform childobj = this.transform.GetChild(0);
                        childobj.gameObject.layer = 10;
                        childobj.SetParent(lifter);
                        childobj.localPosition = localHoldingPosition;

                        TopSlot.HasRightSupport = false;
                    }
                    break;

                case Slots.Top:
                    {
                        //change parent to lifter 
                        Transform childobj = this.transform.GetChild(0);
                        childobj.gameObject.layer = 10;
                        childobj.SetParent(lifter);
                        childobj.localPosition = localHoldingPosition;
                    }
                    break;
            }
        }
    }
}

