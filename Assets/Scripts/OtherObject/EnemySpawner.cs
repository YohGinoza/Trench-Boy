using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameController gameController;

    [SerializeField] private GameObject EnemyPrefab;
    [Tooltip("Enemy randomly spawn within these line")]
    [SerializeField] private float AreaCovered = 5;
    [Tooltip("Maximum number of enemy FROM THIS SPAWNER alive at the same time")]
    [SerializeField] private int PoolSize = 10;
    private GameObject[] EnemyPool;

    [Space]
    [Header("Day Modifier Attributes")]
    [Tooltip("Set to enable this spawner in each day")]
    [SerializeField] private bool[] Enable;
    [SerializeField] private float[] SecondsBetweenSpawns;

    [Tooltip("The more aggressive they are, the less time they spend behind the cover when not suppressed. (10 - aggressiveness sec)")]
    [Range(0, 10)] [SerializeField] private float[] Aggressiveness;

    [Header("Grenade")]
    [Tooltip("Calculate each time they get out of cover")]
    [Range(0, 1)] [SerializeField] private float[] GrenadeThrowingChance;
    [Tooltip("Distance that the enemy start calculate chance of throwing grenade")]
    [SerializeField] private float[] GrenadeThrowingDistance;
    [Tooltip("How many times enemy can throw grenade")]
    [SerializeField] private int[] GrenadeNumber;

    [Header("Firing")]
    [Tooltip("Distance that the enemy start shooting")]
    [SerializeField] private float[] FiringDistance;
    [Tooltip("Interval time in second")]
    [Range(0.2f, 2.0f)] [SerializeField] private float[] FiringInterval;
    [Tooltip("maximum bullet spread (degree)")]
    [Range(0, 5)] [SerializeField] private float[] AimmingError;

    public bool CoroutineRunning = false;

    private void Start()
    {
        if (gameController == null)
        {
            gameController = FindObjectOfType<GameController>();

        }

        EnemyPool = new GameObject[PoolSize];
        for (int i = 0; i < PoolSize; i++)
        {
            EnemyPool[i] = Instantiate(EnemyPrefab);
            EnemyPool[i].SetActive(false);
        }
    }

    void SetupEnemy(GameObject enemy, int day)
    {
        EnemyBehaviour e = enemy.GetComponent<EnemyBehaviour>();
        e.ShotDelay = FiringInterval[day];
        e.AimmingError = AimmingError[day];
        e.Aggressiveness = Aggressiveness[day];
        e.MaxFiringDistance = FiringDistance[day];

        e.GrenadeLimit = GrenadeNumber[day];
        e.GrenadesThrowChance = GrenadeThrowingChance[day];
        e.GrenadeThrowingDistance = GrenadeThrowingDistance[day];
    }

    public IEnumerator SpawnEnemy()
    {
        CoroutineRunning = true;

        //take current day
        int currentDay = (int)gameController.CurrentDay;

        while (true)
        {
            yield return new WaitForSeconds(SecondsBetweenSpawns[currentDay]);

            Vector3 SpawnPosition = this.transform.position + (Vector3.left * (Random.Range(-AreaCovered / 2, AreaCovered / 2)));

            foreach (GameObject enemy in EnemyPool)
            {
                if (enemy.activeSelf == false)
                {
                    //Debug.Log("Spawned");
                    SetupEnemy(enemy, currentDay);
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
