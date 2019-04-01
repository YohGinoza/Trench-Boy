using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [Range(0.5f, 2)] [SerializeField] private float ShotDelay;
    [Range(0, 1)] [SerializeField] private float Accuracy;
    [Range(0, 10)] [SerializeField] private float Aggressiveness;
    [SerializeField] private float AwareBoxSize = 2.5f;
    [SerializeField] private int GrenadeLimit = 1;
    [Range(0, 1)] [SerializeField] private float GrenadesThrowChance = 0.25f;
    //[Range(0, 1)] [SerializeField] private float ThrowingAccuracy;
    [SerializeField] private float ThrowingTime = 0.5f;
    [SerializeField] private float GrenadeArriveTime = 3;

    const float BulletLaunchForce = 4;
    [SerializeField] private float MaxTargetDistance = 15;
    [SerializeField] private float UrgentTargetZDistance = 3;
    [SerializeField] private LayerMask TargetLayer;

    [SerializeField] private LayerMask BulletLayer;

    [SerializeField] private Transform Muzzle;
    [SerializeField] private int BulletPoolSize = 2;
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private GameObject GrenadePrefab;

    [SerializeField] private float CoverIgnoreDistance = 1.5f;
    [SerializeField] private float ArrivingDistance = 2;

    //background process variable
    private NavMeshAgent Agent;
    private Transform CurrentTarget;
    private bool Shooting = false;

    private GameObject[] BulletPool;
    private GameObject[] GrenadePool;

    public int GrenadeCount = 0;

    public float SuppressedTimer = 0;
    public GameObject MovingTarget;
    bool ThrowingDecided = false;
    bool WillThrowGrenade = false;

    bool ReachedBarbedWire = false;

    private Animator animator;

    private void Start()
    {
        BulletPool = new GameObject[BulletPoolSize];
        for (int i = 0; i < BulletPoolSize; i++)
        {
            BulletPool[i] = Instantiate(BulletPrefab);
            BulletPool[i].SetActive(false);
        }

        GrenadePool = new GameObject[GrenadeLimit];
        for (int i = 0; i < GrenadeLimit; i++)
        {
            GrenadePool[i] = Instantiate(GrenadePrefab);
            GrenadePool[i].SetActive(false);
        }

        animator = this.GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        Agent = this.GetComponent<NavMeshAgent>();
        Agent.enabled = false;
        GrenadeCount = GrenadeLimit;
        MovingTarget = this.gameObject;
    }

    private void FixedUpdate()
    {
        //if not shooting, find new target
        if (!ReachedBarbedWire)
        {
            if (SuppressedTimer > (10 - Aggressiveness))
            {
                //take action
                Action();
                //Debug.Log("actions");
            }
            else
            {
                //hold position and fire
                Agent.enabled = false;
                if (!Shooting)
                {
                    StartCoroutine(Shoot());
                }
            }
        }

        //suppressed
        Collider[] Bullets = Physics.OverlapBox(this.transform.position + Vector3.back * AwareBoxSize / 2, new Vector3(AwareBoxSize / 2, AwareBoxSize / 2, AwareBoxSize / 2), this.transform.rotation, BulletLayer);
        foreach(Collider bullet in Bullets)
        {
            if(bullet.GetComponent<Rigidbody>().velocity.z > 0)
            {
                SuppressedTimer = 0;
                break;
            }
        }

        SuppressedTimer += Time.fixedDeltaTime;
    }

    private void Action()
    {
        //stop shooting
        StopCoroutine(Shoot());

        //pop out
        if (!ThrowingDecided)
        {
            if (GrenadeCount > 0 && Random.Range(0, 1) < GrenadesThrowChance)
            {
                WillThrowGrenade = true;
                StartCoroutine(ThrowGrenade());
            }
            else
            {
                WillThrowGrenade = false;
            }
            ThrowingDecided = true;
        }

        if(ThrowingDecided && !WillThrowGrenade)
        {
            if (!Agent.enabled)
            {
                Agent.enabled = true;
                AdvancePosition();
                Agent.SetDestination(MovingTarget.transform.position);

                //animation
                animator.SetBool("Running", true);
            }
            else if ((this.transform.position - Agent.destination).magnitude < ArrivingDistance)
            {
                ReachedBarbedWire = MovingTarget.CompareTag("BarbedWire");
                animator.SetBool("ReachedBarbedWire", ReachedBarbedWire);

                Agent.SetDestination(this.transform.position);
                ThrowingDecided = false;
                SuppressedTimer = 0;
                Agent.enabled = false;
                //Debug.Log("arrived");

                //animation
                animator.SetBool("Running", false);
            }
        }
        //print("this " + this.transform.position + "target " + Agent.destination + "distance" + (this.transform.position - Agent.destination).magnitude);
    }

    private IEnumerator ThrowGrenade()
    {
        FindNewTarget();

        //throws at target
        if (CurrentTarget != null)
        {
            foreach (GameObject grenade in GrenadePool)
            {
                if (!grenade.activeSelf)
                {
                    //animation
                    animator.SetTrigger("Grenade");
                    yield return new WaitForSeconds(ThrowingTime);

                    grenade.transform.position = Muzzle.position;
                    grenade.SetActive(true);
                    //change later
                    

                    float Xdiff = CurrentTarget.position.x - this.transform.position.x;
                    float zdiff = CurrentTarget.position.z - this.transform.position.z;
                    Vector2 Direction = new Vector2(Xdiff, zdiff);
                    float heightDiff = CurrentTarget.position.y - this.transform.position.y;
                    float mass = grenade.GetComponent<Rigidbody>().mass;
                    float Ux = Direction.magnitude / GrenadeArriveTime;
                    float Uy = heightDiff - (0.5f * Physics.gravity.y * GrenadeArriveTime);

                    Vector3 ThrowForce = new Vector3((Direction.normalized * Ux).x, Uy, (Direction.normalized * Ux).y) * mass;
                    grenade.GetComponent<Rigidbody>().AddForce(ThrowForce, ForceMode.Impulse);
                    GrenadeCount--;
                    break;
                }
            }
        }

        WillThrowGrenade = false;
    }

    private void AdvancePosition()
    {
        float closestDistance = 100;
        GameObject ClosestPoint = null;
        GameObject[] Covers = GameObject.FindGameObjectsWithTag("EnemyCover");
        GameObject[] BarbedWires = GameObject.FindGameObjectsWithTag("BarbedWire");

        //Debug.Log(Covers.Length);
        foreach (GameObject BarbedWire in BarbedWires)
        {
            if ((BarbedWire.transform.position.z < this.transform.position.z) && ((BarbedWire.transform.position - this.transform.position).magnitude > CoverIgnoreDistance) && ((BarbedWire.transform.position - this.transform.position).magnitude < closestDistance))
            {
                closestDistance = (BarbedWire.transform.position - this.transform.position).magnitude;
                ClosestPoint = BarbedWire;
            }
        }

        foreach (GameObject Cover in Covers)
        {
            if ((Cover.transform.position.z < this.transform.position.z) && ((Cover.transform.position - this.transform.position).magnitude > CoverIgnoreDistance) && ((Cover.transform.position - this.transform.position).magnitude < closestDistance))
            {
                closestDistance = (Cover.transform.position - this.transform.position).magnitude;
                ClosestPoint = Cover;
            }
        }

        if(ClosestPoint != null)
        {
            //Debug.Log(ClosestPoint);
            MovingTarget = ClosestPoint;
        }
        else
        {
            MovingTarget = this.gameObject;
        }
    }

    private void FindNewTarget()
    {
        //resets target
        CurrentTarget = null;

        //collect targets
        Collider[] Targets = Physics.OverlapSphere(this.transform.position, MaxTargetDistance, TargetLayer);

        //find target
        float ClosestPriorDistance = MaxTargetDistance;
        float ClosestLowPriorDistance = MaxTargetDistance;
        bool HasUrgentTarget = false; //target that is too close to trench line

        foreach (Collider target in Targets)
        {
            float Distance = (target.transform.position - this.transform.position).magnitude;

            if (Distance <= ClosestLowPriorDistance)
            {
                ClosestLowPriorDistance = Distance;
                CurrentTarget = target.transform;
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
            //animation
            animator.SetTrigger("Fire");
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
                bullet.transform.position = Muzzle.position;
                bullet.SetActive(true);
                bullet.GetComponent<Rigidbody>().AddForce(FiringDirection, ForceMode.Impulse);
                bullet.GetComponent<BulletBehaviour>().TargetLayer = TargetLayer;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(this.transform.position + Vector3.back * AwareBoxSize / 2, new Vector3(AwareBoxSize, AwareBoxSize, AwareBoxSize));
        Gizmos.DrawWireSphere(this.transform.position, MaxTargetDistance);
    }

    public IEnumerator cutBarbedWire()
    {
        yield return new WaitForSeconds(BarbedWire.CUTTING_TIME);
        GameController.DayEnded = true;
        GameController.BarbedWireDestroyed = true;
    }
}
