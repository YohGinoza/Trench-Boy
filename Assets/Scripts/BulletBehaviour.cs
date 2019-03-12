﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [System.NonSerialized] public LayerMask TargetLayer;
    [SerializeField] private float MaxLifeTime = 0.75f;

    private float LifeTime = 0;

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
        this.gameObject.SetActive(false);
    }
}
