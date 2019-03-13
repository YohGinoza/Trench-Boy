using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllyBehaviour : MonoBehaviour
{
    [Header("Personal Data")]
    [SerializeField] private Ally Identity;

    [Range(0.5f, 2)] [SerializeField] private float ShotDelay;
    [Range(0, 1)] [SerializeField] private float Accuracy;
    //[Range(0, 10)] [SerializeField] private float AimingPatience;
    //[Range(0, 1)] [SerializeField] private float SteadyHand;
    //[Range(0, 1)] [SerializeField] private float GunQuality;
    //[Range(0, 1)] [SerializeField] private float AimingPrioritizer;
    //[Tooltip("Time before this unit get back to shooting after medical request is ignnored")]
    [Range(0, 30)] [SerializeField] private float WaitingPatience;

    [Header("General Setting")]
    const float BulletLaunchForce = 4;
    [SerializeField] private int MaxAmmo = 30;
    [SerializeField] private int AmmoCount = 10;
    [SerializeField] private bool Injured = false;

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

    private float WaitTimer = 0;

    [Header("Feedback and Animation")]
    [SerializeField] private Sprite AmmoRequest;
    [SerializeField] private Sprite HealRequest;
    [SerializeField] private Image RequestImage;

    [SerializeField] private Material FiringMaterial;
    [SerializeField] private Material WaitingMaterial;
    private Renderer renderer;

    private void Start()
    {
        renderer = this.GetComponentInChildren<MeshRenderer>();
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
        

        if(Injured && WaitTimer <= WaitingPatience)
        {
            WaitTimer += Time.fixedDeltaTime;
            //stop shooting
            StopCoroutine(Shoot());
        }
        else
        {
            if (!Shooting && AmmoCount > 0)
            {
                StartCoroutine(Shoot());
            }
        }

        //check availability
        if (AmmoCount > 0 && (!Injured || WaitTimer >= WaitingPatience))
        {
            RequestImage.enabled = false;
            renderer.material = FiringMaterial;

            //duck
            this.transform.position = new Vector3(this.transform.position.x, 1, this.transform.position.z);
        }
        else
        {
            RequestImage.enabled = true;
            renderer.material = WaitingMaterial;

            //popup
            this.transform.position = new Vector3(this.transform.position.x, 0.5f, this.transform.position.z);
        }

        //set request image
        if (Injured)
        {
            RequestImage.sprite = HealRequest;
        }else if (AmmoCount <= 0)
        {
            RequestImage.sprite = AmmoRequest;
        }
    }

    public bool HandItem(ItemType item)
    {
        switch (item)
        {
            case ItemType.Ammo:
                if (AmmoCount < (MaxAmmo - (int)ItemType.Ammo))
                {
                    AmmoCount += (int)ItemType.Ammo;
                    return true;
                }
                else
                {
                    print("Me pouches are too heavy mate.");
                    return false;
                }
            case ItemType.Med:
                if (Injured)
                {
                    Injured = false;
                    WaitTimer = 0;
                    return true;
                }
                else
                {
                    print("Na, I'm good.");
                    return false;
                }
        }
        Debug.Log("Wrong Item");
        return false;
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
        foreach(GameObject bullet in BulletPool)
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
        if (!Injured)
        {
            Injured = true;
            WaitTimer = 0;
        }
        else
        {
            Dead();
        }
    }

    private void Dead ()
    {
        //Collect deceased data
        GameController.AlliesAliveStatus[(int)Identity] = false;

        //subject to change
        this.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine;
    }
}
