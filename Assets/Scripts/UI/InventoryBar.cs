using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryBar : MonoBehaviour
{   // Start is called before the first frame update

    private Image[] med = new Image[3];
    private Image[] ammo = new Image[3];
    private Image[] HL = new Image[3];

    private int SelectedItem;

    private ItemType[] ItemInventory = new ItemType[2];

    void Start()
    {
        //med1 = GetComponentInChildren<Cell_1_Med>().GetComponent<Image>();
        med[0] = GameObject.Find("Cell_1_Med").GetComponent<Image>();
        med[1] = GameObject.Find("Cell_2_Med").GetComponent<Image>();

        ammo[0] = GameObject.Find("Cell_1_Ammo").GetComponent<Image>();
        ammo[1] = GameObject.Find("Cell_2_Ammo").GetComponent<Image>();

        HL[0] = GameObject.Find("Cell_1_HL").GetComponent<Image>();
        HL[1] = GameObject.Find("Cell_2_HL").GetComponent<Image>();


    }
   // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < ItemInventory.Length; i++)
        {
            ItemInventory[i] = GameObject.Find("Player").GetComponent<InventorySystem>().ItemInventory[i];
        }
            
        SelectedItem = GameObject.Find("Player").GetComponent<InventorySystem>().SelectedItem;

        switch (SelectedItem)
        {
            case 0:
                HL[0].fillAmount = 1;
                HL[1].fillAmount = 0;
                break;
            case 1:
                HL[0].fillAmount = 0;
                HL[1].fillAmount = 1;
                break;
            case 2:
                HL[0].fillAmount = 0;
                HL[1].fillAmount = 0;
                break;
        }

        for(int i = 0; i < ItemInventory.Length; i++)
        {
            switch (ItemInventory[i]){
                case ItemType.Ammo:
                    ammo[i].fillAmount = 1;
                    med[i].fillAmount = 0;
                    break;
                case ItemType.Med:
                    ammo[i].fillAmount = 0;
                    med[i].fillAmount = 1;
                    break;
                case ItemType.None:
                    ammo[i].fillAmount = 0;
                    med[i].fillAmount = 0;
                    break;
            }

        }
        

    }
}
