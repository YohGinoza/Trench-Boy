using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artillery : MonoBehaviour
{
    [SerializeField] private GameController gc;
    [SerializeField] private float FiringInterval;
    [SerializeField] private GameObject ArtilleryAmmo;
    [SerializeField] private int poolSize;
    [SerializeField] private GameObject[] Pool;

    [SerializeField] private Vector2 FireArea;
    public bool firing = false;

    private void Start()
    {
        Pool = new GameObject[poolSize];

        for(int i = 0; i < poolSize; i++)
        {
            GameObject ammo = Instantiate(ArtilleryAmmo, this.transform);
            Pool[i] = ammo;
            ammo.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if ((gc.CurrentState == GameState.Day || gc.CurrentState == GameState.Stalling) && !firing)
        {
            StartCoroutine(FireArtillery());
        }
        else if ((gc.CurrentState == GameState.Night || gc.CurrentState == GameState.Wait))
        {
            StopAllCoroutines();
        }
    }

    IEnumerator FireArtillery()
    {
        firing = true;
        yield return new WaitForSeconds(FiringInterval);
        for (int i = 0; i < Pool.Length; i++)
        {
            if (!Pool[i].activeSelf)
            {
                Pool[i].SetActive(true);
                Pool[i].transform.position = this.transform.position + new Vector3(Random.Range(-FireArea.x, FireArea.x), this.transform.position.y, Random.Range(-FireArea.y, FireArea.y));
                Debug.Log("Fuckn Boom");
                break;
            }
        }
        firing = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(this.transform.position, new Vector3(FireArea.x, 0.1f, FireArea.y));
    }

}
