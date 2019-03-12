using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
//THIS CODE NEEDS A TOTAL REWORK
//
public class FakeAssScoutZone : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float defaultCamSize = 7;
    [SerializeField] private float ScoutCamSize = 20;
    [SerializeField] private float defaultCamZ = -17;
    [SerializeField] private float ScoutCamZ = 4;
    [SerializeField] private float ZoomTime = 1;
    private float reff = 0;
    bool scouting = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            scouting = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            scouting = false;
        }
    }

    private void FixedUpdate()
    {
        if (scouting)
        {
            cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, Mathf.SmoothDamp(cam.transform.localPosition.z, ScoutCamZ, ref reff, ZoomTime));
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, ScoutCamSize, ref reff, ZoomTime);
        }
        else
        {
            cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, Mathf.SmoothDamp(cam.transform.localPosition.z, defaultCamZ, ref reff, ZoomTime));
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, defaultCamSize, ref reff, ZoomTime);
        }
    }
}
