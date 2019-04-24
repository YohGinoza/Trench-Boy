using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject EnemyPrefab;
    [SerializeField] private float AreaCovered = 5;
    [SerializeField] private float SecondsBetweenSpawns = 5;
    [SerializeField] private int PoolSize = 10;
    private GameObject[] EnemyPool;

    public bool CoroutineRunning = false;

    private void Start()
    {
        EnemyPool = new GameObject[PoolSize];
        for (int i = 0; i < PoolSize; i++)
        {
            EnemyPool[i] = Instantiate(EnemyPrefab);
            EnemyPool[i].SetActive(false);
        }
    }

    public IEnumerator SpawnEnemy()
    {
        CoroutineRunning = true;

        while (true)
        {
            yield return new WaitForSeconds(SecondsBetweenSpawns);

            Vector3 SpawnPosition = this.transform.position + (Vector3.left * (Random.Range(-AreaCovered / 2, AreaCovered / 2)));

            foreach (GameObject enemy in EnemyPool)
            {
                if (enemy.activeSelf == false)
                {
                    Debug.Log("Spawned");
                    enemy.SetActive(true);
                    enemy.transform.position = SpawnPosition;
                    break;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(this.transform.position + (Vector3.left * (AreaCovered / 2)), this.transform.position + (Vector3.right * (AreaCovered / 2)));
    }
}
