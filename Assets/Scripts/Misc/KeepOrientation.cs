using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepOrientation : MonoBehaviour
{
    public Vector3 OriginalEulerAngle;
    public Vector3 OriginalScale;
    public Vector3 OrginalChildPos;
    Transform parent;

    private void Start()
    {
        OriginalEulerAngle = this.transform.eulerAngles;
        OriginalScale = this.transform.lossyScale;
        OrginalChildPos = this.transform.localPosition;
        parent = this.transform.parent;
    }

    private void FixedUpdate()
    {
        this.transform.rotation = Quaternion.Euler(OriginalEulerAngle);
        this.transform.parent = null;
        this.transform.localScale = OriginalScale;
        this.transform.parent = parent;
        this.transform.localPosition = OrginalChildPos;
    }
}
