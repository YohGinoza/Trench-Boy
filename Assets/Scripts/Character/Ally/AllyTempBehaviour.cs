using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllyTempBehaviour : MonoBehaviour
{
    [SerializeField] private int MaxAmmo = 30;
    [SerializeField] private int MaxHP = 10;
    [SerializeField] private int AmmoCount = 10;
    [SerializeField] private int CurrentHP;
    public int getCurrentHP() { return CurrentHP; }
    public int getAmmoCount() { return AmmoCount; }    
    public bool isDown = false;
    public bool isDead = false;
    
    [Header("Temp variables")]
    [SerializeField] private float MinAmmoDepletionRate = 5;
    [SerializeField] private float MaxAmmoDepletionRate = 10;
    private float AmmoDepletionTime;
    [SerializeField] private float MinHPDepletionRate = 20;
    [SerializeField] private float MaxHPDepletionRate = 30;
    private float HPDepletionTime;

    [SerializeField] private Sprite AmmoRequest;
    [SerializeField] private Sprite HealRequest;
    [SerializeField] private Image RequestImage;

    [SerializeField] private Material FiringMaterial;
    [SerializeField] private Material WaitingMaterial;
    private Renderer renderer;

    private bool Hrunning, Arunning;

    private void Awake()
    {
        //will not have physical contact with crate
        Physics.IgnoreLayerCollision(9, 10, true);

        CurrentHP = MaxHP;
        renderer = this.GetComponentInChildren<Renderer>();
    }

    private void FixedUpdate()
    {
        //hp check
        if (!Arunning && AmmoCount > 0 && CurrentHP > 0)
        {
            StartCoroutine(AmmoDeplete());
        }
        else if (CurrentHP < 0)
        {
            RequestImage.sprite = HealRequest;
        }

        //ammo check
        if (!Hrunning && CurrentHP > 0 && AmmoCount > 0)
        {
            StartCoroutine(HPDeplete());
        }
        else if (AmmoCount < 0)
        {
            RequestImage.sprite = AmmoRequest;
        }

        if(AmmoCount > 0 && CurrentHP > 0)
        {
            RequestImage.enabled = false;
            renderer.material = FiringMaterial;
        }
        else
        {
            RequestImage.enabled = true;
            renderer.material = WaitingMaterial;
        }
    }

    IEnumerator AmmoDeplete()
    {
        Arunning = true;
        AmmoDepletionTime = Random.Range(MinAmmoDepletionRate, MaxAmmoDepletionRate);
        yield return new WaitForSeconds(AmmoDepletionTime);
        AmmoCount--;
        Arunning = false;
    }

    IEnumerator HPDeplete()
    {
        Hrunning = true;
        HPDepletionTime = Random.Range(MinHPDepletionRate, MaxHPDepletionRate);
        yield return new WaitForSeconds(HPDepletionTime);
        CurrentHP--;
        Hrunning = false;
    }

    public void Heal(int Amount)
    {
        if (CurrentHP < (MaxHP - Amount))
        {
            CurrentHP += Amount;
        }
        else
        {
            print("Na, I'm good.");
        }
    }

    public void ReSupply(int Amount)
    {
        if (AmmoCount < (MaxAmmo - Amount))
        {
            AmmoCount += Amount;
        }
        else
        {
            print("Me pouches are too heavy mate.");
        }
    } 
}
