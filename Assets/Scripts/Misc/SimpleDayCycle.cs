using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDayCycle : MonoBehaviour
{
    [SerializeField] private float DayLength = 180;
    [Range(0, 1)] [SerializeField] private float NightTime = 0.5f;
    [Range(0, 1)] [SerializeField] private float NightEnd = 0.0f;
    [Range(0, 1)] public float CurrentTime = 0;
    private float offsetXangle;
    public bool nighting = false;
    [SerializeField] private Light sun;

    private GameObject[] nightlights;

    private void Start()
    {
        offsetXangle = sun.transform.rotation.eulerAngles.x;

        nightlights = GameObject.FindGameObjectsWithTag("NightLight");
        Debug.Log(nightlights.Length);
        foreach (GameObject light in nightlights)
        {
            //set up
            var em = light.GetComponent<ParticleSystem>().emission;
            em.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        CurrentTime += Time.fixedDeltaTime / DayLength;
        sun.transform.Rotate(new Vector3(-Time.fixedDeltaTime * (360 / DayLength), 0, 0), Space.World);
        //sun.transform.rotation = Quaternion.Euler((((-Time.time % DayLength) / DayLength) * 360) + offsetXangle, sun.transform.rotation.eulerAngles.y, sun.transform.rotation.eulerAngles.z);

        if(CurrentTime > 1)
        {
            CurrentTime = 0;
        }

        if (CurrentTime > NightTime && !nighting)
        {
            nighting = true;
            foreach (GameObject light in nightlights)
            {
                var em = light.GetComponent<ParticleSystem>().emission;
                em.enabled = true;
            }
        }
        else if (CurrentTime > NightEnd && CurrentTime < NightTime && nighting)
        {
            nighting = false;
            foreach (GameObject light in nightlights)
            {
                var em = light.GetComponent<ParticleSystem>().emission;
                em.enabled = false;
            }
        }
    }
}
