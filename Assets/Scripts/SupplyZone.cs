using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyZone : MonoBehaviour
{
    [SerializeField] private ItemType SupplyType;
    [SerializeField] GameObject Crate;
    [SerializeField] private int CrateNumberLimit = 15;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TrenchBoyController player = other.GetComponent<TrenchBoyController>();
            player.ReSupplyZone = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TrenchBoyController player = other.GetComponent<TrenchBoyController>();
            player.ReSupplyZone = null;
        }
    }

    public void SpawnNew(Transform Holder)
    {
        int CrateNumber = 0;
        GameObject[] crates = GameObject.FindGameObjectsWithTag("Crate");
        foreach(GameObject crate in crates)
        {
            if(crate.GetComponent<Crates>().Type == SupplyType)
            {
                CrateNumber++;
            }
        }

        if (CrateNumber <= CrateNumberLimit)
        {
            GameObject newCrate = Instantiate(Crate, Holder);
            newCrate.transform.localPosition = Vector3.zero;
        }
    }

    public void RefillCrate(Crates crate)
    {
        if (crate.Type == SupplyType)
        {
            crate.Refill();
        }
    }
}
