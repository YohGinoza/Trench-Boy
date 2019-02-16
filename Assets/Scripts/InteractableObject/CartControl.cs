using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartControl : MonoBehaviour
{
    [SerializeField] private float MaxSpeed;
    [SerializeField] private float TimeToReachMaxSpeed;
    [SerializeField] private float TimeToCompletelyStop;
    [SerializeField] private Transform Rail;
    [SerializeField] private Transform StartingEnd;
    [SerializeField] private Transform EndingEnd;

    private Rigidbody rigidbody;

    private float RailDistance;
    private Vector3 RailDirection;

    private Vector3 refVelocity = Vector3.zero;

    bool isMoving = false;

    private void Awake()
    {
        this.transform.SetParent(Rail);
        this.transform.localPosition = StartingEnd.localPosition + (Vector3.up * 0.5f);

        RailDirection = EndingEnd.position - StartingEnd.position;
        RailDistance = (EndingEnd.position - StartingEnd.position).magnitude;

        rigidbody = this.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //if player not pushing and still on track slow down until speed reaches 0
        if ((Vector3.Angle(rigidbody.velocity, RailDirection) < 90 && Vector3.Angle(rigidbody.velocity, EndingEnd.position - this.transform.position) < 90) || (Vector3.Angle(rigidbody.velocity, RailDirection) >= 90 && Vector3.Angle(rigidbody.velocity, StartingEnd.position - this.transform.position) < 90))
        {
            rigidbody.velocity = Vector3.SmoothDamp(rigidbody.velocity, Vector3.zero, ref refVelocity, TimeToCompletelyStop);
        }
        else //out of track, stop immediatly
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.isKinematic = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Vector3.Angle((this.transform.position - other.transform.position), RailDirection) < 90)
            {
                //move to ending end
                MoveCart(RailDirection);
            }
            else
            {
                //move to starting end
                MoveCart(-RailDirection);
            }
        }
    }
    private void OntriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isMoving = false;
        }
    }

    private void MoveCart(Vector3 movingDirection)
    {
        //if cart is on track accelerate until reaches max speed
        if ((Vector3.Angle(movingDirection, RailDirection) < 90 && Vector3.Angle(movingDirection, EndingEnd.position - this.transform.position) < 90)|| (Vector3.Angle(movingDirection, RailDirection) >= 90 && Vector3.Angle(movingDirection, StartingEnd.position - this.transform.position) < 90))
        {
            rigidbody.isKinematic = false;
            rigidbody.velocity = Vector3.SmoothDamp(rigidbody.velocity, movingDirection.normalized * MaxSpeed, ref refVelocity, TimeToReachMaxSpeed);
        }
        else//out of track, stop immediatly
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.isKinematic = true;
        }
    }
}
