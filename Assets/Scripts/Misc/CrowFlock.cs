using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowFlock : MonoBehaviour
{
    [SerializeField] private GameController gc;

    [SerializeField] float FlockSpeed;
    [SerializeField] float ReturnTime;
    [SerializeField] GameObject Crow;
    private GameObject[] Crows;
    [Range(5, 20)] [SerializeField] int FlockSize;
    [SerializeField] float FlockSpread;
    private float timer = 0;
    private Vector3 SpawnPos;

    private void Start()
    {
        Crows = new GameObject[FlockSize];
        for (int i = 0; i < FlockSize; i++)
        {
            GameObject c = Instantiate(Crow, this.transform);
            Crows[i] = c;
        }
        ResetFlock();
    }

    private void FixedUpdate()
    {
        if (gc.CurrentState == GameState.Day)
        {
            this.transform.position += this.transform.forward * FlockSpeed * Time.fixedDeltaTime;
            timer += Time.fixedDeltaTime;

            if (timer > ReturnTime)
            {
                ResetFlock();
                timer = 0;
            }
        }
    }

    private void ResetFlock()
    {
        if (Crows != null)
        {
            for (int i = 0; i < Crows.Length; i++)
            {
                Crows[i].SetActive(false);
            }
        }

        float size = Random.Range(FlockSize - 3, FlockSize);
        for(int i = 0; i < size; i++)
        {
            Crows[i].SetActive(true);
            //random position
            Vector3 pos = new Vector3(Random.Range(-FlockSpread, FlockSpread), Random.Range(-FlockSpread, FlockSpread), Random.Range(-FlockSpread, FlockSpread));
            Crows[i].transform.localPosition = pos;
            Crows[i].transform.localRotation = Quaternion.identity;
        }

        this.transform.Rotate(Vector3.up, Random.Range(0, 360), Space.World);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(this.transform.position, Vector3.one * FlockSpread);
    }
}
