using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [System.NonSerialized] public LayerMask TargetLayer;
    [SerializeField] private float MaxLifeTime = 0.75f;

    private float LifeTime = 0;

    private void Start()
    {
        //Ignore Bullet/BarbedWire
        Physics.IgnoreLayerCollision(13, 14);
    }

    private void OnEnable()
    {
        this.GetComponent<TrailRenderer>().enabled = true;
    }

    private void FixedUpdate()
    {
        if (this.gameObject.activeSelf)
        {
            LifeTime += Time.fixedDeltaTime;
        }

        if (LifeTime >= MaxLifeTime)
        {
            LifeTime = 0;
            Disable();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (TargetLayer == (TargetLayer | 1 << collision.gameObject.layer))
        {
            switch (collision.gameObject.layer)
            {
                case 9: //ally
                    collision.transform.GetComponent<AllyBehaviour>().Shot();
                    break;
                case 12: //enemy
                    collision.transform.GetComponent<EnemyBehaviour>().Shot();
                    break;
            }
        }
        Disable();
    }

    private void Disable()
    {
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<TrailRenderer>().enabled = false;
        this.gameObject.SetActive(false);
    }
}
