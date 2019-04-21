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

    public AudioClip[] grenadeSFX = new AudioClip[2];

    public bool Call = true;

    private void FixedUpdate()
    {
        CallOut();

        ExplodeTimer += Time.fixedDeltaTime;

        if (ExplodeTimer >= ExplodeTime)
        {
            ExplodeTimer = 0;
            StartCoroutine(Explode());
        }
    }

    private void Awake()
    {
        this.GetComponent<MeshRenderer>().enabled = true;

        GameObject[] Allies = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject Ally in Allies)
        {
            Ally.GetComponent<AllyBehaviour>().callGrenade = true;
        }
    }

    IEnumerator  Explode()
    {
        Collider[] Victims = Physics.OverlapSphere(this.transform.position, ExplosionRadius, VictimLayers);
        foreach (Collider victim in Victims)
        {
            victim.GetComponent<AllyBehaviour>().Shot();

            if (Random.Range(0.0f, 1.0f) < DownChance)
            {
                victim.GetComponent<AllyBehaviour>().Shot();
            }
        }

        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<MeshRenderer>().enabled = false;

        int index = Random.Range(0, grenadeSFX.Length);
        this.GetComponent<AudioSource>().clip = grenadeSFX[index];
        this.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(2.0f);
        this.gameObject.SetActive(false);
        
    }

    private void CallOut()
    {
        Collider[] Victims = Physics.OverlapSphere(this.transform.position, ExplosionRadius, VictimLayers);

        foreach (Collider victim in Victims)
        {
            if (victim.GetComponent<AllyBehaviour>().callGrenade) {

                victim.GetComponent<AllyBehaviour>().callSource.clip = victim.GetComponent<AllyBehaviour>().grenadeCall;
                victim.GetComponent<AllyBehaviour>().callSource.Play();

                victim.GetComponent<AllyBehaviour>().callGrenade = false;

                print("grenade call");
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(this.transform.position, ExplosionRadius);
    }
}
