using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampLight : MonoBehaviour
{
    [SerializeField] private float flameSpeed = 0.05f;
    [Range(0, 0.5f)] [SerializeField] private float flameBrightnessSway = 0.5f;

    Light light = null;
    float px = 0;
    float py = 0;

    private void Start()
    {
        light = this.GetComponent<Light>();
        px = Random.Range(0.0f, 50.0f);
        py = Random.Range(0.0f, 50.0f);
    }

    private void FixedUpdate()
    {
        px += flameSpeed;
        py += flameSpeed;

        if (light != null)
        {
            light.intensity = 0.5f + (0.5f - flameBrightnessSway) + (flameBrightnessSway * Mathf.PerlinNoise(px, py));
        }
    }
}
