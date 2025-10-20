using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.Spawn;
using Player.Stats.Runtime;
using UnityEngine;


public class BossModel : MonoBehaviour, IDamageable
{
    [Header("Stats Config")]
    public BossStatsSO statsSO;

    public event Action<float> OnHealthChanged;
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public Vector3 Transform => transform.position;


    public event Action<BossModel> OnDeath;


    private BossView view;
    private BossController enemy;
    private OrbSpawner orbSpawner;

    [Header("Floating Damage Text Effect")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private float heightTextSpawn = 2f;
    [SerializeField] private GameObject jumpingCoinsTextPrefab;
    [SerializeField] private float heightCoinsTextSpawn = 2f;

    //[Header("Health bar")]
    //[SerializeField] private GameObject healthBarPrefab;
    //[SerializeField] private float heightBarSpawn = 2.5f;
    //private Transform healthBar;
    //private Transform healthFill;


    private void Start()
    {
        MaxHealth = statsSO.MaxHealth;
        CurrentHealth = MaxHealth;
 
        view = GetComponent<BossView>();
        enemy = GetComponent<BossController>();
        orbSpawner = GameManager.Instance.orbSpawner;


        OnHealthChanged?.Invoke(CurrentHealth);

    }

    public void PrintMessage(String text, float lifeTime)
    {
        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * heightTextSpawn;
            GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            textObj.GetComponent<FloatingDamageText>().Initialize(text, lifeTime);

        }
    }
    public void TakeDamage(float damageAmount)
    {
        //if (enemy.GetShield()) return;


        if (statsSO.RastroOrbOnHit && orbSpawner != null)
        {
            for (int i = 0; i < statsSO.numberOfOrbsOnHit; i++)
            {
                orbSpawner.SpawnHealingOrb(transform.position, transform.forward);
            }
        }

        float newHealth = CurrentHealth - damageAmount;
        CurrentHealth = GetCappedHealth(newHealth);

        OnHealthChanged?.Invoke(CurrentHealth);
      


        // Mostrar texto flotante
        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * heightTextSpawn; 
            GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            textObj.GetComponent<FloatingDamageText>().Initialize(damageAmount);
        }

        if (CurrentHealth <= 0) Die();
    }


    private float GetCappedHealth(float newHealth)
    {
        float healthPercent = CurrentHealth / MaxHealth;

        if (healthPercent > 0.66f)
            return Mathf.Max(newHealth, MaxHealth * 0.66f); 
        else if (healthPercent > 0.33f)
            return Mathf.Max(newHealth, MaxHealth * 0.33f);
        else
            return newHealth; 
    }




    public void Die()
    {
        if (statsSO.RastroOrbOnDeath && orbSpawner != null)
        {
            for (int i = 0; i < statsSO.numberOfOrbsOnDeath; i++)
            {
                orbSpawner.SpawnHealingOrb(transform.position, transform.forward);
            }
        }



        if (RunData.CurrentCurrency != null)
        {
            RunData.CurrentCurrency.AddCoins(statsSO.CoinsToDrop);

            //Mostrar texto flotante Coins
            if (jumpingCoinsTextPrefab != null)
            {
                Vector3 spawnPos = transform.position + Vector3.up * heightCoinsTextSpawn;
                GameObject textObj = Instantiate(jumpingCoinsTextPrefab, spawnPos, Quaternion.identity);
                textObj.GetComponent<JumpingCoinsText>().Initialize(statsSO.CoinsToDrop);
            }

        }
        OnDeath?.Invoke(this);
    }

    public void ApplyDebuffDoT(float dotDuration, float dps)
    {
        Debug.LogWarning("ApplyDebuffDoT called, but not implemented yet.");
    }
}

