using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepOrientation : MonoBehaviour
{
    [SerializeField] private Vector3 OriginalEulerAngle;
    [SerializeField] private Vector3 OriginalScale;
    [SerializeField] private Vector3 OrginalChildPos;
    Transform parent;
    [SerializeField] private bool YOnly = true;

    private void Start()
    {
        OriginalEulerAngle = this.transform.eulerAngles;
        OriginalScale = this.transform.lossyScale;
        OrginalChildPos = this.transform.localPosition;
        parent = this.transform.parent;
    }

    private void FixedUpdate()
    {
        if (YOnly)
        {
            this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x, OriginalEulerAngle.y, this.transform.rotation.eulerAngles.z);
        }
        else
        {
            this.transform.rotation = Quaternion.Euler(OriginalEulerAngle);
        }
        //this.transform.parent = null;
        //this.transform.localScale = OriginalScale;
        //this.transform.parent = parent;
        //this.transform.localPosition = OrginalChildPos;
    }
}
