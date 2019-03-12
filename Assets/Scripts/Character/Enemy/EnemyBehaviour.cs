using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Personal Data")]

    [Range(0.5f, 2)] [SerializeField] private float ShotDelay;
    [Range(0, 1)] [SerializeField] private float Accuracy;
    [SerializeField] private float BulletLaunchForce;
    //[Range(0, 10)] [SerializeField] private float AimingPatience;
    //[Range(0, 1)] [SerializeField] private float SteadyHand;
    //[Range(0, 1)] [SerializeField] private float GunQuality;
    //[Range(0, 1)] [SerializeField] private float AimingPrioritizer;
    //[Tooltip("Time before this unit get back to shooting after medical request is ignnored")]

    [Header("General Setting")]
    [SerializeField] private float MaxTargetDistance = 15;
    [SerializeField] private float UrgentTargetZDistance = 3;
    [SerializeField] private LayerMask EnemyLayer;

    [SerializeField] private Transform Muzzle;
    [SerializeField] private int BulletPoolSize = 2;
    [SerializeField] private GameObject BulletPrefab;

    //background process variable
    private Transform CurrentTarget;
    //private float TargetDistance;
    private bool Shooting = false;
    private GameObject[] BulletPool;




    private void Start()
    {
        BulletPool = new GameObject[BulletPoolSize];
        for (int i = 0; i < BulletPoolSize; i++)
        {
            BulletPool[i] = Instantiate(BulletPrefab);
            BulletPool[i].SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        //if not shooting, find new target

        if (!Shooting)
        {
            StartCoroutine(Shoot());
        }
    }

    private void FindNewTarget()
    {
        //resets target
        CurrentTarget = null;

        //collect targets
        Collider[] Enemies = Physics.OverlapSphere(this.transform.position, MaxTargetDistance, EnemyLayer);

        //find target
        float ClosestPriorDistance = MaxTargetDistance;
        float ClosestLowPriorDistance = MaxTargetDistance;
        bool HasUrgentTarget = false; //target that is too close to trench line

        foreach (Collider enemy in Enemies)
        {
            float ZDistance = enemy.transform.position.z - this.transform.position.z;
            float Distance = (enemy.transform.position - this.transform.position).magnitude;

            if (ZDistance <= UrgentTargetZDistance)
            {
                HasUrgentTarget = true;
                if (Distance <= ClosestPriorDistance)
                {
                    ClosestPriorDistance = Distance;
                    CurrentTarget = enemy.transform;
                }

            }
            else if (!HasUrgentTarget)
            {
                if (Distance <= ClosestLowPriorDistance)
                {
                    ClosestLowPriorDistance = Distance;
                    CurrentTarget = enemy.transform;
                }
            }
        }
    }

    private IEnumerator Shoot()
    {
        Shooting = true;
        FindNewTarget();

        if (CurrentTarget != null)
        {
            //calculate
            //float distance = (CurrentTarget.position - this.transform.position).magnitude;
            Vector3 BulletForce = (CurrentTarget.transform.position - this.transform.position).normalized + new Vector3(Mathf.Tan(Mathf.Deg2Rad * (Random.Range(0, ((1 - Accuracy) * 5)))), Mathf.Tan(Mathf.Deg2Rad * Random.Range(0, ((1 - Accuracy) * 5))), 0);
            BulletForce.Normalize();
            BulletForce *= BulletLaunchForce;
            //aim
            yield return new WaitForSeconds(ShotDelay);
            //fire
            ShootBullet(BulletForce);
        }
        else
        {
            yield return null;
        }
        Shooting = false;
    }

    private void ShootBullet(Vector3 FiringDirection)
    {
        foreach (GameObject bullet in BulletPool)
        {
            if (!bullet.activeSelf)
            {
                bullet.SetActive(true);
                bullet.transform.position = Muzzle.position;
                bullet.GetComponent<Rigidbody>().AddForce(FiringDirection, ForceMode.Impulse);
                bullet.GetComponent<BulletBehaviour>().TargetLayer = EnemyLayer;
                break;
            }
        }
    }

    public void Shot()
    {
        Dead();
    }

    private void Dead()
    {
        //subject to change
        this.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine;
    }
}
