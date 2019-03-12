using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepOrientation : MonoBehaviour
{
    public Vector3 OriginalEulerAngle;

    private void Start()
    {
        OriginalEulerAngle = this.transform.eulerAngles;
    }

    private void FixedUpdate()
    {
        this.transform.rotation = Quaternion.Euler(OriginalEulerAngle);        
    }
}
