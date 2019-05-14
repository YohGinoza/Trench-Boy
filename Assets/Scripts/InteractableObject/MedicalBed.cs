using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicalBed : MonoBehaviour
{
    AllyBehaviour Patient = null;

    private void FixedUpdate()
    {
        if (Patient != null && Patient.CurrentState != AllyBehaviour.State.Healing)
        {
            DissmissPatient();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TrenchBoyController player = other.GetComponent<TrenchBoyController>();
            player.MedBed = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TrenchBoyController player = other.GetComponent<TrenchBoyController>();
            player.MedBed = null;
        }
    }

    public void PutPatient(Transform patient)
    {
        if (patient.CompareTag("Ally"))
        {
            patient.transform.parent = this.transform;
            patient.transform.localPosition = Vector3.zero + (Vector3.up * 0.4f);
            Patient = patient.GetComponent<AllyBehaviour>();
            Patient.CurrentState = AllyBehaviour.State.Healing;
        }
    }

    public void DissmissPatient()
    {
        if (Patient != null)
        {
            Patient.transform.parent = null;
            Patient.transform.position += Vector3.forward;
            Patient = null;
        }
    }
}
