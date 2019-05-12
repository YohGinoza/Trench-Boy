using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBehaviour : MonoBehaviour
{
    [SerializeField] private float ExplodeTime = 5;
    [SerializeField] private float ExplosionRadius = 3;
    [Range(0, 1)] [SerializeField] private float DownChance = 0.75f;
    [SerializeField] private LayerMask VictimLayers;
    [SerializeField] private float ExplosionForce = 10;
    [SerializeField] private float UpwardForceModifier = 1;
    [SerializeField] private MeshRenderer[] meshes;
    private bool exploded = false;
    private float ExplodeTimer;

    public AudioClip[] grenadeSFX = new AudioClip[2];

    public bool Call = true;

    private void FixedUpdate()
    {
        CallOut();

        ExplodeTimer += Time.fixedDeltaTime;

        if (!exploded && ExplodeTimer >= ExplodeTime)
        {
            StartCoroutine(Explode());
        }
    }

    private void Awake()
    {
        GameObject[] Allies = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject Ally in Allies)
        {
            Ally.GetComponent<AllyBehaviour>().callGrenade = true;
        }
    }

    private void OnEnable()
    {
        ExplodeTimer = 0;
        exploded = false;

        foreach (MeshRenderer childmesh in meshes)
        {
            childmesh.enabled = true;
        }
    }

    IEnumerator  Explode()
    {
        exploded = true;
        //display effect
        if(this.transform.childCount > 0)
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
        }
        //damage ally
        Collider[] Victims = Physics.OverlapSphere(this.transform.position, ExplosionRadius, VictimLayers);
        foreach (Collider victim in Victims)
        {
            victim.GetComponent<AllyBehaviour>().Shot();

            if (Random.Range(0.0f, 1.0f) < DownChance)
            {
                victim.GetComponent<AllyBehaviour>().Shot();
            }
        }
        //apply force to player
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().AddExplosionForce(ExplosionForce, this.transform.position, ExplosionRadius, UpwardForceModifier, ForceMode.Impulse);
        }
        Debug.Log("boom");

        //reset grenade
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //this.GetComponent<MeshRenderer>().enabled = false;

        foreach (MeshRenderer childmesh in meshes)
        {
            childmesh.enabled = false;
        }

        int index = Random.Range(0, grenadeSFX.Length);
        this.GetComponent<AudioSource>().clip = grenadeSFX[index];
        this.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(3.0f);
        //turnoff effect
        if (this.transform.childCount > 0)
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
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
