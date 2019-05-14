using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryAmmo : MonoBehaviour
{
    [SerializeField] private float lifeTime = 10;

    private void OnEnable()
    {
        StartCoroutine(DestroySelf());   
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(lifeTime);
        this.gameObject.SetActive(false);
    }
}
