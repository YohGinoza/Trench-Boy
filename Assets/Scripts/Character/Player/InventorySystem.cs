using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//soon to be moved to gamecontrol ==============
public enum ItemType { Ammo = 10, Med = 5, None = 0 };           //the number is amount per pouch
//==============================================

public class InventorySystem : MonoBehaviour
{
    /*[System.NonSerialized]*/ public ItemType[] ItemInventory = new ItemType[2];
    /*[System.NonSerialized]*/ public int SelectedItem;

    public bool Add(ItemType item)
    {
        bool Added = false;
        for (int i = 0; i < ItemInventory.Length; i++)
        {
            if(ItemInventory[i] == ItemType.None)
            {
                ItemInventory[i] = item;
                Added = true;
                break;
            }
        }
        return Added;
    }

    public bool Add(ItemType item, int slot)
    {
        bool Added = false;
        if ((slot >= 0 && slot < ItemInventory.Length) && ItemInventory[slot] == ItemType.None)
        {
            ItemInventory[slot] = item;
            Added = true;
        }
        return Added;
    }

    public void RemoveItem(int slot)
    {
        //clear item from inventory
        ItemInventory[slot] = ItemType.None;

        //move selctor to another available item;
        CycleInventory();
    }

    public void RemoveItem()
    {
        //clear item from inventory
        ItemInventory[SelectedItem] = ItemType.None;

        //move selctor to another available item;
        CycleInventory();
    }

    public void CycleInventory()
    {
        Debug.Log("Cycling Inventory...");
        int index = SelectedItem;

        if (isEmpty())
        {
            SelectedItem = 0;
        }
        else
        {
            for (int i = 0; i < ItemInventory.Length; i++)
            {
                SelectedItem++;
                if (SelectedItem >= ItemInventory.Length)
                {
                    SelectedItem = 0;
                }

                if (ItemInventory[SelectedItem] != ItemType.None)
                {
                    break;
                }
            }
        }
    }

    public bool HasEmptySlot()
    {
        for (int i = 0; i < ItemInventory.Length; i++)
        {
            if (ItemInventory[i] == ItemType.None)
            {
                return true;
            }
        }
        return false;
    }

    public bool isEmpty()
    {
        for (int i = 0; i < ItemInventory.Length; i++)
        {
            if (ItemInventory[i] != ItemType.None)
            {
                return false;
            }
        }
        return true;
    }
}
