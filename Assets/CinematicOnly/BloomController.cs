using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BloomController : MonoBehaviour
{
    [SerializeField] private Bloom bloom;
    [SerializeField] private float IntensityStart;
    [SerializeField] private float IntensityEnd;
    [SerializeField] private float TransitionTime;
    float timer = 0;

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime / TransitionTime;
        bloom.intensity.value = Mathf.Lerp(IntensityStart, IntensityEnd, timer);
    }
}
