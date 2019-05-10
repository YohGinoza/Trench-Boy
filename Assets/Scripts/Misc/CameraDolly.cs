using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDolly : MonoBehaviour
{
    [SerializeField] private Transform Start;
    [SerializeField] private Transform End;
    [SerializeField] private float MoveTime;
    float timer = 0;

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime/MoveTime;
        transform.position = Vector3.Lerp(Start.position, End.position, timer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Start.position, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(End.position, 0.1f);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(Start.position, End.position);
    }
}
