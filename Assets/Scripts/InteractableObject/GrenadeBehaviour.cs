using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBehaviour : MonoBehaviour
{
    [SerializeField] private float ExplodeTime = 5;
    [SerializeField] private float ExplosionRadius = 3;
    [Range(0, 1)] [SerializeField] private float DownChance = 0.75f;
    [SerializeField] private LayerMask VictimLayers;

    private float ExplodeTimer;

    public AudioClip grenadeSFX;

    private void FixedUpdate()
    {
        ExplodeTimer += Time.fixedDeltaTime;

        if (ExplodeTimer >= ExplodeTime)
        {
            ExplodeTimer = 0;
            Explode();
        }
    }

    private void Awake()
    {
        this.GetComponent<MeshRenderer>().enabled = true;
    }

    private void Explode()
    {
        Collider[] Victims = Physics.OverlapSphere(this.transform.position, ExplosionRadius, VictimLayers);
        foreach (Collider victim in Victims)
        {
            victim.GetComponent<AllyBehaviour>().Shot();

            if (Random.Range(0, 1) < DownChance)
            {
                victim.GetComponent<AllyBehaviour>().Shot();
            }
        }

        this.GetComponent<AudioSource>().Play();
        StartCoroutine(Disable());

        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.gameObject.SetActive(false);
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(this.transform.position, ExplosionRadius);
    }

    IEnumerator Disable()
    {
        yield return new WaitForSeconds(0.4f);
        this.gameObject.SetActive(false);
    }


}
