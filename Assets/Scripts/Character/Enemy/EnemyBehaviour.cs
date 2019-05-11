using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("General")]
    [Range(0, 10)] public float Aggressiveness;
    [SerializeField] private float AwareBoxSize = 2.5f;

    [Header("Firing")]
    [Range(0.2f, 2)] public float ShotDelay;
    [Range(0.001f, 5)] public float AimmingError;
    public float MaxFiringDistance = 15;

    [Header("Grenade")]
    public int GrenadeLimit = 1;
    [Range(0, 1)] public float GrenadesThrowChance = 0.25f;
    public float GrenadeThrowingDistance = 7.5f;
    //[Range(0, 1)] [SerializeField] private float ThrowingAccuracy;
    [SerializeField] private float ThrowingTime = 0.5f;
    [SerializeField] private float GrenadeArriveTime = 3;

    const float BulletLaunchForce = 4;
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

    private int GrenadeCount = 0;

    public float SuppressedTimer = 0;
    public GameObject MovingTarget;
    bool ThrowingDecided = false;
    bool WillThrowGrenade = false;

    bool ReachedBarbedWire = false;

    private Animator animator;

    public AudioClip BarbedWireCut;
    public AudioSource BarbedWireSource;
    private bool call = false;

    public AudioClip DownSFX;
    public AudioSource audioSource;

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
    }

    private void Awake()
    {
        Agent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        Debug.Log("enabled");
        //reenable mesh renderee
        this.GetComponentInChildren<MeshRenderer>().enabled = true;
        //reset a unit
        ReachedBarbedWire = false;
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
                    Debug.Log("Aiming");
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
                if (ReachedBarbedWire)
                {
                    BarbedWireSource.clip = BarbedWireCut;
                    BarbedWireSource.Play();
                }

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
        FindNewTarget(GrenadeThrowingDistance);

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

    private void FindNewTarget(float distance)
    {
        //resets target
        CurrentTarget = null;

        //collect targets
        Collider[] Targets = Physics.OverlapSphere(this.transform.position, distance, TargetLayer);

        //find target
        float ClosestDistance = distance;
        bool HasUrgentTarget = false; //target that is too close to trench line

        foreach (Collider target in Targets)
        {
            float Distance = (target.transform.position - this.transform.position).magnitude;

            if (Distance <= ClosestDistance)
            {
                ClosestDistance = Distance;
                CurrentTarget = target.transform;
            }
        }
    }

    private IEnumerator Shoot()
    {
        Shooting = true;
        FindNewTarget(MaxFiringDistance);

        if (CurrentTarget != null)
        {
            //calculate
            //float distance = (CurrentTarget.position - this.transform.position).magnitude;
            Vector3 BulletForce = ((CurrentTarget.transform.position + (Vector3.up * CurrentTarget.transform.lossyScale.y / 2)) - Muzzle.position).normalized + new Vector3(Mathf.Tan(Mathf.Deg2Rad * (Random.Range(0, AimmingError))), Mathf.Tan(Mathf.Deg2Rad * Random.Range(0, AimmingError)), 0);
            BulletForce.Normalize();
            BulletForce *= BulletLaunchForce;
            //aim
            yield return new WaitForSeconds(ShotDelay);
            //fire
            ShootBullet(BulletForce);
            //animation
            animator.SetTrigger("Fire");
            //Debug.Log("fire");
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
                //Debug.Log("sdfsdfsdfdfdf");
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
        StartCoroutine(Dead());
    }

    private IEnumerator Dead()
    {

        this.GetComponentInChildren<MeshRenderer>().enabled = false;
        if (!call) {
            call = true;
            audioSource.clip = DownSFX;
            audioSource.Play();
        }
        yield return new WaitForSeconds(1.3f);
        //subject to change
        BarbedWireSource.Stop();
        this.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(this.transform.position + Vector3.back * AwareBoxSize / 2, new Vector3(AwareBoxSize, AwareBoxSize, AwareBoxSize));
        Gizmos.DrawWireSphere(this.transform.position, MaxFiringDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, GrenadeThrowingDistance);
    }

    public IEnumerator cutBarbedWire()
    {
        yield return new WaitForSeconds(BarbedWire.CUTTING_TIME);
        GameController.DayEnded = true;
        GameController.loseCondition = LoseCondition.BarbedWire;
    }
}
